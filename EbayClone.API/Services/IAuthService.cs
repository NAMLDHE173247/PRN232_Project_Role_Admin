using EbayClone.API.DTOs.Auth;

namespace EbayClone.API.Services;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
}
