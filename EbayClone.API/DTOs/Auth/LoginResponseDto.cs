namespace EbayClone.API.DTOs.Auth;

public record LoginResponseDto(string Token, int UserId, string Email, string Role);
