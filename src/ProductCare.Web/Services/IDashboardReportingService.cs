using ProductCare.Web.Models.Entities;
using ProductCare.Web.ViewModels.Admin;
using ProductCare.Web.ViewModels.Customer;

namespace ProductCare.Web.Services;

public interface IDashboardReportingService
{
    Task<CustomerDashboardViewModel> GetCustomerDashboardAsync(string userId, CancellationToken cancellationToken = default);

    Task<AdminDashboardViewModel> GetAdminDashboardAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ServiceTicketStatisticsDto>> GetServiceTicketStatisticsAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductWarrantySummaryView>> GetWarrantySummaryReportAsync(CancellationToken cancellationToken = default);

    Task<AdminReportsViewModel> GetFullReportsAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default);
}
