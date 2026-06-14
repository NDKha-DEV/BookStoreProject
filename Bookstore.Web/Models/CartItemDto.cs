// vị trí: Bookstore.Web/Models/CartItemDto.cs
using System;

namespace Bookstore.Web.Models
{
    public class CartItemDto
    {
        public int ProductId { get; set; }
        public int? VariantId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? Title { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;
    }
}
