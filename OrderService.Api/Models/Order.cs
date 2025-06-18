namespace OrderService.Api.Models;

    public class Order
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = null!;
    public string ProductId { get; set; } = null!;
    public int Quantity { get; set; }
    public string PaymentMethod { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}