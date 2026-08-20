namespace EbayClone.API.DTOs.Feedbacks;

public record AdminFeedbackDto(
    int Id,
    int? SellerId,
    decimal? AverageRating,
    int? TotalReviews,
    decimal? PositiveRate);
