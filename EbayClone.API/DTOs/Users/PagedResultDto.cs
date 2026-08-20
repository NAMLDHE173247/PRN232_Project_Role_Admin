namespace EbayClone.API.DTOs.Users;

public record PagedResultDto<T>(int Page, int PageSize, int Total, IReadOnlyList<T> Items);
