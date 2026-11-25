using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCare.Web.Data;
using ProductCare.Web.Models.Entities;
using ProductCare.Web.Models.Enums;
using ProductCare.Web.Services;
using ProductCare.Web.ViewModels.Api;
using ProductCare.Web.ViewModels.Common;

namespace ProductCare.Web.Controllers.Api;

[ApiController]
[Route("api/products")]
[Authorize(AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},Identity.Application")]
[Produces("application/json")]
public class ProductsApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IWarrantyCalculatorService _warrantyCalculator;
    private readonly ILogger<ProductsApiController> _logger;

    public ProductsApiController(
        ApplicationDbContext context,
        IWarrantyCalculatorService warrantyCalculator,
        ILogger<ProductsApiController> logger)
    {
        _context = context;
        _warrantyCalculator = warrantyCalculator;
        _logger = logger;
    }

    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private bool IsAdmin => User.IsInRole(DbInitializerService.RoleAdmin);

    /// <summary>
    /// Retrieves all products owned by the authenticated customer (or all products if Admin).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<ProductDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] ProductCategory? category, [FromQuery] WarrantyStatus? status)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var query = _context.Products.AsNoTracking().AsQueryable();

        if (!IsAdmin)
        {
            query = query.Where(p => p.UserId == CurrentUserId);
        }

        if (category.HasValue)
        {
            query = query.Where(p => p.Category == category.Value);
        }

        var products = await query.OrderBy(p => p.WarrantyExpiryDate).ToListAsync();
        var resultList = new List<ProductDto>();

        foreach (var p in products)
        {
            var warranty = _warrantyCalculator.GetWarrantyDetails(p.WarrantyExpiryDate, today);

            if (status.HasValue && warranty.Status != status.Value)
            {
                continue;
            }

            resultList.Add(MapToDto(p, warranty));
        }

        return Ok(ApiResponse<List<ProductDto>>.Ok(resultList));
    }

    /// <summary>
    /// Retrieves product details by ID, enforcing customer ownership verification.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null || (!IsAdmin && product.UserId != CurrentUserId))
        {
            return NotFound(ApiResponse<object>.Fail($"Product #{id} was not found."));
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var warranty = _warrantyCalculator.GetWarrantyDetails(product.WarrantyExpiryDate, today);

        return Ok(ApiResponse<ProductDto>.Ok(MapToDto(product, warranty)));
    }

    /// <summary>
    /// Creates and registers a new product for the authenticated user.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.Fail("Validation failed.", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (dto.PurchaseDate > today)
        {
            return BadRequest(ApiResponse<object>.Fail("Purchase date cannot be in the future."));
        }

        var duplicateSerial = await _context.Products.AnyAsync(p =>
            p.UserId == CurrentUserId &&
            p.SerialNumber == dto.SerialNumber.Trim());

        if (duplicateSerial)
        {
            return BadRequest(ApiResponse<object>.Fail("A product with this serial number is already registered under your account."));
        }

        var expiryDate = _warrantyCalculator.CalculateExpiryDate(dto.PurchaseDate, dto.WarrantyDurationMonths);

        var product = new Product
        {
            UserId = CurrentUserId,
            Name = dto.Name.Trim(),
            Brand = dto.Brand.Trim(),
            ModelNumber = dto.ModelNumber.Trim(),
            SerialNumber = dto.SerialNumber.Trim(),
            Category = dto.Category,
            PurchaseDate = dto.PurchaseDate,
            WarrantyDurationMonths = dto.WarrantyDurationMonths,
            WarrantyExpiryDate = expiryDate,
            PurchaseStore = dto.PurchaseStore?.Trim(),
            PurchasePrice = dto.PurchasePrice,
            Notes = dto.Notes?.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var warranty = _warrantyCalculator.GetWarrantyDetails(expiryDate, today);
        var responseDto = MapToDto(product, warranty);

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, ApiResponse<ProductDto>.Ok(responseDto, "Product registered successfully."));
    }

    /// <summary>
    /// Updates an existing registered product.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.Fail("Validation failed.", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
        }

        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null || (!IsAdmin && product.UserId != CurrentUserId))
        {
            return NotFound(ApiResponse<object>.Fail($"Product #{id} was not found."));
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (dto.PurchaseDate > today)
        {
            return BadRequest(ApiResponse<object>.Fail("Purchase date cannot be in the future."));
        }

        var duplicateSerial = await _context.Products.AnyAsync(p =>
            p.UserId == product.UserId &&
            p.SerialNumber == dto.SerialNumber.Trim() &&
            p.Id != id);

        if (duplicateSerial)
        {
            return BadRequest(ApiResponse<object>.Fail("Another registered product already has this serial number."));
        }

        product.Name = dto.Name.Trim();
        product.Brand = dto.Brand.Trim();
        product.ModelNumber = dto.ModelNumber.Trim();
        product.SerialNumber = dto.SerialNumber.Trim();
        product.Category = dto.Category;
        product.PurchaseDate = dto.PurchaseDate;
        product.WarrantyDurationMonths = dto.WarrantyDurationMonths;
        product.WarrantyExpiryDate = _warrantyCalculator.CalculateExpiryDate(dto.PurchaseDate, dto.WarrantyDurationMonths);
        product.PurchaseStore = dto.PurchaseStore?.Trim();
        product.PurchasePrice = dto.PurchasePrice;
        product.Notes = dto.Notes?.Trim();
        product.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var warranty = _warrantyCalculator.GetWarrantyDetails(product.WarrantyExpiryDate, today);
        return Ok(ApiResponse<ProductDto>.Ok(MapToDto(product, warranty), "Product updated successfully."));
    }

    /// <summary>
    /// Deletes a registered product.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products
            .Include(p => p.ServiceRequests)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null || (!IsAdmin && product.UserId != CurrentUserId))
        {
            return NotFound(ApiResponse<object>.Fail($"Product #{id} was not found."));
        }

        if (product.ServiceRequests.Any())
        {
            return BadRequest(ApiResponse<object>.Fail("Cannot delete this product because it has associated service requests in history."));
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<object>.Ok(new { DeletedProductId = id }, "Product deleted successfully."));
    }

    private static ProductDto MapToDto(Product p, WarrantyDetails warranty)
    {
        return new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Brand = p.Brand,
            ModelNumber = p.ModelNumber,
            SerialNumber = p.SerialNumber,
            Category = p.Category,
            CategoryName = p.Category.ToString(),
            PurchaseDate = p.PurchaseDate,
            WarrantyDurationMonths = p.WarrantyDurationMonths,
            WarrantyExpiryDate = p.WarrantyExpiryDate,
            RemainingDays = warranty.RemainingDays,
            WarrantyStatus = warranty.Status,
            WarrantyStatusText = warranty.StatusDisplayText,
            PurchaseStore = p.PurchaseStore,
            PurchasePrice = p.PurchasePrice,
            HasInvoice = !string.IsNullOrEmpty(p.InvoiceFileName),
            InvoiceOriginalFileName = p.InvoiceOriginalFileName,
            Notes = p.Notes,
            CreatedAtUtc = p.CreatedAtUtc
        };
    }
}
