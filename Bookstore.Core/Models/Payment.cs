// vị trí: Bookstore.Core/Models/Payment.cs
namespace Bookstore.Core.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        
        // Tên phương thức thanh toán: "Momo", "VNPay", "COD"
        public string PaymentMethod { get; set; } = string.Empty; 
        
        public bool IsSuccess { get; set; } = false;
    }
}