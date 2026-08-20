using EbayClone.API.Models;

namespace EbayClone.API.DTOs.Reviews;

public record AdminReviewDto(
    int Id,
    int? ProductId,
    int? ReviewerId,
    int? Rating,
    string? Comment,
    DateTime? CreatedAt,
    ReviewStatus Status);
