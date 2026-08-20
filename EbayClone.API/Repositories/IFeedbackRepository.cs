using EbayClone.API.Models;

namespace EbayClone.API.Repositories;

public interface IFeedbackRepository
{
    Task<(int Total, IReadOnlyList<Feedback> Items)> GetPageAsync(
        int? sellerId,
        decimal? minRating,
        decimal? maxRating,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<Feedback?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
