// Vị trí: Bookstore.Core/Models/NV3_Cart/Decorators.cs
using Bookstore.Core.Interfaces;

namespace Bookstore.Core.Models.NV3_Cart
{
    // Lớp Decorator nền tảng (Base Decorator)
    public abstract class CartDecorator : ICart
    {
        protected readonly ICart _innerCart;

        protected CartDecorator(ICart cart)
        {
            _innerCart = cart;
        }

        public virtual decimal CalculateTotal() => _innerCart.CalculateTotal();
    }

    // Concrete Decorator 1: Tính thêm thuế VAT (Ví dụ: +10%)
    public class VatTaxDecorator : CartDecorator
    {
        public VatTaxDecorator(ICart cart) : base(cart) { }

        public override decimal CalculateTotal()
        {
            decimal baseTotal = base.CalculateTotal();
            return baseTotal + (baseTotal * 0.10m); // Cộng thêm 10% thuế
        }
    }

    // Concrete Decorator 2: Dịch vụ gói quà tặng (+30,000đ)
    public class GiftWrappingDecorator : CartDecorator
    {
        public GiftWrappingDecorator(ICart cart) : base(cart) { }

        public override decimal CalculateTotal()
        {
            return base.CalculateTotal() + 30000m; // Cộng thêm phí gói quà
        }
    }
}