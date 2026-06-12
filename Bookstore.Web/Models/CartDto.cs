using System.Collections.Generic;
using System.Linq;

namespace Bookstore.Web.Models
{
    public class CartDto
    {
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();

        public decimal Total => Items.Sum(i => i.LineTotal);

        public int TotalQuantity => Items.Sum(i => i.Quantity);
    }
}
