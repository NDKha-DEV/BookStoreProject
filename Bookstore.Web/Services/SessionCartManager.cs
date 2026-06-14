// vị trí: Bookstore.Web/Services/SessionCartManager.cs
using Bookstore.Web.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Bookstore.Web.Services
{
    public class SessionCartManager : ICartManager
    {
        private const string SessionKey = "Cart";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionCartManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
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
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId && i.VariantId == variantId);
            if (item == null)
            {
                item = new CartItemDto
                {
                    ProductId = productId,
                    VariantId = variantId,
                    Quantity = quantity,
                    UnitPrice = 0m,
                    Title = null
                };
                cart.Items.Add(item);
            }
            else
            {
                item.Quantity += quantity;
            }

            SaveCart(cart);
            return cart;
        }

        public CartDto UpdateQuantity(int productId, int quantity, int? variantId = null)
        {
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId && i.VariantId == variantId);
            if (item == null) return cart;

            if (quantity <= 0)
            {
                cart.Items.Remove(item);
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
}
