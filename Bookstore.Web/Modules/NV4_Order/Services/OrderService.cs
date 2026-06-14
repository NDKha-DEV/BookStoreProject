// Vị trí: Bookstore.Web/Modules/NV4_Order/Services/OrderService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Bookstore.Core.Interfaces;
using Bookstore.Core.Models.NV4_Order;
using Bookstore.Core.Models.NV4_Order.Interfaces;
using Bookstore.Core.Models.NV3_Cart;
using Bookstore.Web.Modules.NV1_Account.Services;
using Bookstore.Web.Modules.NV4_Order.States;
using Bookstore.Web.Modules.NV4_Order.Observers;

namespace Bookstore.Web.Modules.NV4_Order.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBookRepository _bookRepository;

        public OrderService(
            IOrderRepository orderRepository, 
            ICartRepository cartRepository, 
            IUserRepository userRepository,
            IBookRepository bookRepository)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _userRepository = userRepository;
            _bookRepository = bookRepository;
        }

        public Order CreateOrder(decimal dynamicTotalAmount)
        {
            var authUser = AuthService.Instance.CurrentLoggedInUser; 
            int currentUserId;

            if (authUser != null)
            {
                currentUserId = authUser.Id;
            }
            else
            {
                var lastUser = _userRepository.GetAll().LastOrDefault();
                currentUserId = lastUser != null ? lastUser.Id : 3;
            }

            var currentCart = _cartRepository.GetByUserId(currentUserId);
            if (currentCart == null)
            {
                currentCart = new Cart();
                _cartRepository.Save(currentUserId, currentCart);
            }

            var newOrder = new Order
            {
                Id = _orderRepository.GetAll().Count + 1,
                UserId = currentUserId,
                TotalAmount = dynamicTotalAmount,
                CreatedDate = DateTime.Now,
                PaymentMethod = "PENDING",
                OrderItems = new List<CartItem>(currentCart.Items) 
            };

            // Tiêm các cổng phụ thuộc vào hệ thống Observer tại Runtime
            newOrder.RegisterObserver(new CustomerPointsObserver(_orderRepository, _userRepository));
            newOrder.RegisterObserver(new InventoryObserver(_orderRepository, _bookRepository));

            newOrder.PaymentStatus = "Unpaid";
            newOrder.ShippingStatus = "NotShipped";
            newOrder.CurrentState = new AwaitingPaymentState();

            _orderRepository.Add(newOrder);
            return newOrder;
        }

        public void ChangeOrderStatus(int orderId, string action)
        {
            var order = _orderRepository.GetById(orderId);
            if (order == null) throw new Exception("Không tìm thấy mã đơn hàng cần duyệt!");

            if (action.Equals("proceed", StringComparison.CurrentCultureIgnoreCase))
            {
                order.Proceed(); 
                _orderRepository.Update(order);
            }
            else if (action.Equals("cancel", StringComparison.CurrentCultureIgnoreCase))
            {
                order.Cancel(); 
                _orderRepository.Update(order);
            }
        }

        public Order GetOrderDetails(int orderId)
        {
            return _orderRepository.GetById(orderId) 
                   ?? throw new Exception("Không tìm thấy đơn hàng trong kho lưu trữ dữ liệu!");
        }
    }
}