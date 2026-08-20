using EbayClone.API.DTOs.Feedbacks;
using EbayClone.API.Models;
using EbayClone.API.Repositories;

namespace EbayClone.API.Services;

public class AdminFeedbackService(IFeedbackRepository feedbackRepository) : IAdminFeedbackService
{
    public async Task<PagedFeedbackResultDto> GetFeedbacksAsync(
        int? sellerId,
        decimal? minRating,
        decimal? maxRating,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);
        if (minRating.HasValue && maxRating.HasValue && minRating > maxRating)
            throw new ArgumentException("minRating must be less than or equal to maxRating.");

        var result = await feedbackRepository.GetPageAsync(
            sellerId, minRating, maxRating, page, pageSize, cancellationToken);

        return new PagedFeedbackResultDto(page, pageSize, result.Total, result.Items.Select(Map).ToList());
    }

    public async Task<AdminFeedbackDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var feedback = await feedbackRepository.GetByIdAsync(id, cancellationToken);
        return feedback is null ? null : Map(feedback);
    }

    private static AdminFeedbackDto Map(Feedback feedback) =>
        new(feedback.Id, feedback.SellerId, feedback.AverageRating, feedback.TotalReviews, feedback.PositiveRate);
}
