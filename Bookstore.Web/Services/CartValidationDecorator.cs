// vị trí: Bookstore.Web/Services/CartValidationDecorator.cs
using Bookstore.Web.Models;

namespace Bookstore.Web.Services
{
    public class CartValidationDecorator : ICartManager
    {
        private readonly ICartManager _inner;
        private readonly IInventoryStrategy _inventory;

        public CartValidationDecorator(SessionCartManager inner, IInventoryStrategy inventory)
        {
            _inner = inner;
            _inventory = inventory;
        }

        public CartDto GetCart() => _inner.GetCart();

        public void SaveCart(CartDto cart) => _inner.SaveCart(cart);

        public CartDto AddToCart(int productId, int quantity, int? variantId = null)
        {
            if (quantity <= 0) quantity = 1;
            var stock = _inventory.GetStock(productId, variantId);
            if (stock == 0) throw new CartException("OutOfStock");

            var cart = _inner.GetCart();
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId && i.VariantId == variantId);
            if (item == null)
            {
                if (quantity > stock) throw new CartException("NotEnoughStock");
                item = new CartItemDto
                {
                    ProductId = productId,
                    VariantId = variantId,
                    Quantity = quantity,
                    UnitPrice = _inventory.GetPrice(productId, variantId),
                    Title = _inventory.GetTitle(productId)
                };
                cart.Items.Add(item);
            }
            else
            {
                var newQty = item.Quantity + quantity;
                if (newQty > stock) throw new CartException("NotEnoughStock");
                item.Quantity = newQty;
            }

            _inner.SaveCart(cart);
            return cart;
        }

        public CartDto UpdateQuantity(int productId, int quantity, int? variantId = null)
        {
            var stock = _inventory.GetStock(productId, variantId);
            if (quantity <= 0)
            {
                return RemoveItem(productId, variantId);
            }

            var cart = _inner.GetCart();
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId && i.VariantId == variantId);
            if (item == null) return cart;

            if (quantity > stock)
            {
                item.Quantity = stock;
            }
            else
            {
                item.Quantity = quantity;
            }

            _inner.SaveCart(cart);
            return cart;
        }

        public CartDto RemoveItem(int productId, int? variantId = null)
        {
            var cart = _inner.GetCart();
            cart.Items.RemoveAll(i => i.ProductId == productId && i.VariantId == variantId);
            _inner.SaveCart(cart);
            return cart;
        }
    }
}
