// Vị trí: Bookstore.Web/Modules/NV3_Cart/Decorators/CartDecorator.cs
using Bookstore.Core.Models;

namespace Bookstore.Web.Modules.NV3_Cart.Decorators
{
    // Interface hoặc Lớp gốc cho Decorator bọc quanh (Thừa kế cấu trúc dữ liệu của Cart)
    public abstract class CartDecorator : Cart
    {
        protected Cart _wrappedCart;
        protected CartDecorator(Cart cart) { _wrappedCart = cart; }
    }

    // Decorator cụ thể: Thêm dịch vụ bọc hộp quà cao cấp
    public class PremiumPackagingDecorator : CartDecorator
    {
        public PremiumPackagingDecorator(Cart cart) : base(cart) { }

        // Bọc lại hàm tính tổng tiền để cộng thêm tiền hộp quà dịch vụ
        public decimal GetTotalCostWithService(decimal baseItemsTotal, decimal shippingFee)
        {
            decimal premiumBoxPrice = 25000; // Cộng thêm 25k tiền hộp quà bọc nhung
            Console.WriteLine("[ DECORATOR] Đã kích hoạt dịch vụ gói quà Premium (+25,000đ vào hóa đơn).");
            return baseItemsTotal + shippingFee + premiumBoxPrice;
        }
    }
}