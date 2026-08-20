namespace EbayClone.API.DTOs.Disputes;

public record PagedDisputeResultDto(
    int Page,
    int PageSize,
    int Total,
    IReadOnlyList<DisputeDto> Items);
