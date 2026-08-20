namespace EbayClone.API.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    public UserStatus Status { get; set; } = UserStatus.Active;
    public string ApprovalStatus { get; set; } = "Approved";
    public int? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? BannedReason { get; set; }
    public int? BannedBy { get; set; }
    public DateTime? BannedAt { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<OrderTable> Orders { get; set; } = new List<OrderTable>();
}
