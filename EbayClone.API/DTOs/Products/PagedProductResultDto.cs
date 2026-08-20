namespace EbayClone.API.DTOs.Products;

public record PagedProductResultDto<T>(
    int Page,
    int PageSize,
    int Total,
    IReadOnlyList<T> Items);
