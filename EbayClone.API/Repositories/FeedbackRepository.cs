using EbayClone.API.Data;
using EbayClone.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EbayClone.API.Repositories;

public class FeedbackRepository(AppDbContext dbContext) : IFeedbackRepository
{
    public async Task<(int Total, IReadOnlyList<Feedback> Items)> GetPageAsync(
        int? sellerId,
        decimal? minRating,
        decimal? maxRating,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Feedbacks.AsNoTracking().AsQueryable();

        if (sellerId.HasValue)
            query = query.Where(feedback => feedback.SellerId == sellerId.Value);
        if (minRating.HasValue)
            query = query.Where(feedback => feedback.AverageRating >= minRating.Value);
        if (maxRating.HasValue)
            query = query.Where(feedback => feedback.AverageRating <= maxRating.Value);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(feedback => feedback.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (total, items);
    }

    public Task<Feedback?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        dbContext.Feedbacks.AsNoTracking().SingleOrDefaultAsync(feedback => feedback.Id == id, cancellationToken);
}
