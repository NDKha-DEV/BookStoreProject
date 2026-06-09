// Trong file: Bookstore.Core/Models/Cart.cs
using Bookstore.Core.Interfaces;

namespace Bookstore.Core.Models
{
    // Tạo Class CartItem trước để chứa thông tin dòng sản phẩm
    public class CartItem
    {
        public Book Product { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal TotalPrice => Product.BasePrice * Quantity;
    }

    // Class Cart chính thức kế thừa từ ICart để chuẩn hóa Decorator Pattern
    public class Cart : ICart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        
        // Khoảng cách giao hàng (Dùng cho Strategy Pattern tính phí vận chuyển ở NV3)
        public decimal DeliveryDistanceInKm { get; set; } 

        public decimal CalculateTotal()
        {
            // Tính tổng tiền các item trong giỏ đơn thuần (Chưa thuế, chưa gói quà)
            decimal sum = 0;
            foreach (var item in Items)
            {
                sum += item.TotalPrice;
            }
            return sum;
        }
    }
}