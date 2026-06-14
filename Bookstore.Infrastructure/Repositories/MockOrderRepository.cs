using System.Collections.Generic;
using System.Linq;
using Bookstore.Core.Interfaces;
using Bookstore.Core.Models;
using Bookstore.Core.Models.NV4_Order;

namespace Bookstore.Infrastructure.Repositories {
    public class MockOrderRepository : IOrderRepository {
        public Order? GetById(int id) => MockDataStore.Orders.FirstOrDefault(o => o.Id == id);
        public List<Order> GetAll() => MockDataStore.Orders;
        public void Add(Order order) => MockDataStore.Orders.Add(order);
        public void Update(Order order) { /* RAM tự cập nhật theo tham chiếu object */ }
    }
}