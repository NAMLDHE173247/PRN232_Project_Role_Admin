namespace EbayClone.API.DTOs.Reviews;

public record PagedReviewResultDto(
    int Page,
    int PageSize,
    int Total,
    IReadOnlyList<AdminReviewDto> Items);
