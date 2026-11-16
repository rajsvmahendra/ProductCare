using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProductCare.Web.Data;
using ProductCare.Web.Middleware;
using ProductCare.Web.Models.Entities;
using ProductCare.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Configure Entity Framework Core with Microsoft SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=(localdb)\\MSSQLLocalDB;Database=ProductCareDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// Configure Data Protection to store cryptographic keys inside local App_Data directory
var keysFolder = Path.Combine(builder.Environment.ContentRootPath, "App_Data", "DataProtection-Keys");
Directory.CreateDirectory(keysFolder);
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysFolder))
    .SetApplicationName("ProductCare");

// 2. Configure ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Practical security settings suitable for enterprise demonstration
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure Cookie Authentication for MVC
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.Name = "ProductCare.AuthCookie";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;

    // Return 401/403 for API routes instead of HTML redirect
    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }
        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        }
        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
});

// 3. Configure JWT Bearer Authentication for REST APIs
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? "ProductCare_Development_Secret_Key_Minimum_32_Characters_Long_For_HmacSha256!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "ProductCare.Web";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "ProductCare.ApiConsumers";

builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

// 4. Register Domain Services with Dependency Injection
builder.Services.AddSingleton<IWarrantyCalculatorService, WarrantyCalculatorService>();
builder.Services.AddScoped<IServiceRequestWorkflowService, ServiceRequestWorkflowService>();
builder.Services.AddScoped<IInvoiceStorageService, InvoiceStorageService>();
builder.Services.AddScoped<IDashboardReportingService, DashboardReportingService>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IDbInitializerService, DbInitializerService>();

// 5. Register MVC Controllers, Razor Views, and API Explorers
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();

// 6. Configure Swagger/OpenAPI with Bearer Authentication support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ProductCare REST API",
        Version = "v1",
        Description = "Product Warranty and Service Management Portal API. Supports both Session Cookies and JWT Bearer Tokens."
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Check for explicit admin provisioning CLI switch
if (args.Contains("--provision-admin") || args.Contains("--seed-admin"))
{
    using var scope = app.Services.CreateScope();
    var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializerService>();
    try
    {
        var passwordOrStatus = await initializer.ProvisionAdminUserAsync();
        Console.WriteLine("=================================================================");
        Console.WriteLine(" PRODUCTCARE: Administrator Provisioning Completed Successfully");
        Console.WriteLine($" Status / Password: {passwordOrStatus}");
        Console.WriteLine("=================================================================");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERROR] Administrator provisioning failed: {ex.Message}");
    }
    return;
}

// Check for explicit demo data seeding CLI switch
if (args.Contains("--seed-demo-data"))
{
    using var scope = app.Services.CreateScope();
    var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializerService>();
    try
    {
        await initializer.SeedDemoDataAsync();
        Console.WriteLine("=================================================================");
        Console.WriteLine(" PRODUCTCARE: Demo Data Seeding Completed Successfully");
        Console.WriteLine(" Sample categories, customers, products, and service tickets seeded.");
        Console.WriteLine("=================================================================");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERROR] Demo data seeding failed: {ex.Message}");
    }
    return;
}

// 7. Request Pipeline Configuration
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Custom diagnostic and security middleware
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();

// Swagger API documentation
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProductCare API v1");
    c.RoutePrefix = "swagger";
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Route configuration
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
