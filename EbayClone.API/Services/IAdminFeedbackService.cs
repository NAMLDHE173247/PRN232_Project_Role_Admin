using EbayClone.API.DTOs.Feedbacks;

namespace EbayClone.API.Services;

public interface IAdminFeedbackService
{
    Task<PagedFeedbackResultDto> GetFeedbacksAsync(
        int? sellerId,
        decimal? minRating,
        decimal? maxRating,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<AdminFeedbackDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
