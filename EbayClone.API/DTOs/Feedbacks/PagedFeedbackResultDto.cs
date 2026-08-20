namespace EbayClone.API.DTOs.Feedbacks;

public record PagedFeedbackResultDto(
    int Page,
    int PageSize,
    int Total,
    IReadOnlyList<AdminFeedbackDto> Items);
