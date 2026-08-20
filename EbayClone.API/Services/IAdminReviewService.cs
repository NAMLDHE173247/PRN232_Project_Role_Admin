using EbayClone.API.DTOs.Reviews;
using EbayClone.API.Models;

namespace EbayClone.API.Services;

public interface IAdminReviewService
{
    Task<PagedReviewResultDto> GetReviewsAsync(
        ReviewStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<AdminReviewDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<AdminReviewDto?> HideAsync(int id, int adminId, CancellationToken cancellationToken = default);
    Task<AdminReviewDto?> UnhideAsync(int id, int adminId, CancellationToken cancellationToken = default);
}
