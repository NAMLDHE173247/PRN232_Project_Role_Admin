namespace EbayClone.API.DTOs.Orders;

public record PagedOrderResultDto(
    int Page,
    int PageSize,
    int Total,
    IReadOnlyList<OrderAdminDto> Items);
