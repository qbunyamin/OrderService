using System.ComponentModel.DataAnnotations;

public class OrderRequest
{
    [Required]
    public string UserId { get; set; } = null!;

    [Required]
    public string ProductId { get; set; } = null!;

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }

    [Required]
    [RegularExpression("CreditCard|BankTransfer", ErrorMessage = "Invalid payment method")]
    public string PaymentMethod { get; set; } = null!;
}
