using EbayClone.API.Models;

namespace EbayClone.API.DTOs.Users;

public record AdminUserDto(
    int Id,
    string Email,
    string FullName,
    string Role,
    UserStatus Status,
    string ApprovalStatus,
    string? BannedReason,
    DateTime? ApprovedAt,
    DateTime? BannedAt);
