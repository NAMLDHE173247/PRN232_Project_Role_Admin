using EbayClone.API.DTOs.Dashboard;

namespace EbayClone.API.Services;

public interface IAdminDashboardService
{
    Task<DashboardDto> GetAsync(CancellationToken cancellationToken = default);
}
