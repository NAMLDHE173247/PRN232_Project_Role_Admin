using EbayClone.API.DTOs.Reviews;
using EbayClone.API.Models;
using EbayClone.API.Repositories;

namespace EbayClone.API.Services;

public class AdminReviewService(IReviewRepository reviewRepository, IAuditRepository auditRepository) : IAdminReviewService
{
    public async Task<PagedReviewResultDto> GetReviewsAsync(
        ReviewStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var result = await reviewRepository.GetPageAsync(status, page, pageSize, cancellationToken);
        return new PagedReviewResultDto(page, pageSize, result.Total, result.Items.Select(Map).ToList());
    }

    public async Task<AdminReviewDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var review = await reviewRepository.GetByIdAsync(id, cancellationToken);
        return review is null ? null : Map(review);
    }

    public Task<AdminReviewDto?> HideAsync(int id, int adminId, CancellationToken cancellationToken = default) =>
        ChangeStatusAsync(id, adminId, ReviewStatus.Visible, ReviewStatus.Hidden, "HIDE_REVIEW", cancellationToken);

    public Task<AdminReviewDto?> UnhideAsync(int id, int adminId, CancellationToken cancellationToken = default) =>
        ChangeStatusAsync(id, adminId, ReviewStatus.Hidden, ReviewStatus.Visible, "UNHIDE_REVIEW", cancellationToken);

    private async Task<AdminReviewDto?> ChangeStatusAsync(
        int id,
        int adminId,
        ReviewStatus expectedStatus,
        ReviewStatus nextStatus,
        string auditAction,
        CancellationToken cancellationToken)
    {
        var review = await reviewRepository.GetByIdAsync(id, cancellationToken);
        if (review is null) return null;
        if (review.Status != expectedStatus)
            throw new InvalidOperationException($"Only {expectedStatus} reviews can be changed by this action.");

        review.Status = nextStatus;
        await reviewRepository.SaveChangesAsync(cancellationToken);
        await auditRepository.AddAsync(new AuditLog
        {
            ActorId = adminId,
            Action = auditAction,
            Resource = "REVIEW",
            ResourceId = review.Id,
            CreatedAtUtc = DateTime.UtcNow
        }, cancellationToken);

        return Map(review);
    }

    private static AdminReviewDto Map(Review review) =>
        new(review.Id, review.ProductId, review.ReviewerId, review.Rating, review.Comment, review.CreatedAt, review.Status);
}
