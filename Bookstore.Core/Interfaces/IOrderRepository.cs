using System.Collections.Generic;
using Bookstore.Core.Models.NV4_Order;
namespace Bookstore.Core.Interfaces {
    public interface IOrderRepository {
        Order? GetById(int id);
        List<Order> GetAll();
        void Add(Order order);
        void Update(Order order);
    }
}