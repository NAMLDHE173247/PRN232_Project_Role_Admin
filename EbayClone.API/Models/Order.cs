namespace EbayClone.API.Models;

public class Order
{
    public int Id { get; set; }
    public int BuyerId { get; set; }
    public User Buyer { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
