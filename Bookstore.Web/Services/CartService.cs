// vị trí: Bookstore.Web/Services/CartService.cs
using Bookstore.Web.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Bookstore.Web.Services
{
    public class CartService
    {
        private const string SessionKey = "Cart";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IInventoryService _inventory;

        public CartService(IHttpContextAccessor httpContextAccessor, IInventoryService inventory)
        {
            _httpContextAccessor = httpContextAccessor;
            _inventory = inventory;
        }

        private ISession Session => _httpContextAccessor.HttpContext!.Session;

        public CartDto GetCart()
        {
            var c = Session.GetObject<CartDto>(SessionKey);
            if (c == null)
            {
                c = new CartDto();
                SaveCart(c);
            }
            return c;
        }

        public void SaveCart(CartDto cart)
        {
            Session.SetObject(SessionKey, cart);
        }

        public CartDto AddToCart(int productId, int quantity, int? variantId = null)
        {
            if (quantity <= 0) quantity = 1;
            var stock = _inventory.GetStock(productId, variantId);
            if (stock == 0) throw new CartException("OutOfStock");

            var cart = GetCart();
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

            SaveCart(cart);
            return cart;
        }

        public CartDto UpdateQuantity(int productId, int quantity, int? variantId = null)
        {
            if (quantity <= 0)
            {
                // treat as removal
                return RemoveItem(productId, variantId);
            }

            var stock = _inventory.GetStock(productId, variantId);
            var cart = GetCart();
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

            SaveCart(cart);
            return cart;
        }

        public CartDto RemoveItem(int productId, int? variantId = null)
        {
            var cart = GetCart();
            cart.Items.RemoveAll(i => i.ProductId == productId && i.VariantId == variantId);
            SaveCart(cart);
            return cart;
        }
    }

    public class CartException : System.Exception
    {
        public string Code { get; }
        public CartException(string code) : base(code) { Code = code; }
    }
}
