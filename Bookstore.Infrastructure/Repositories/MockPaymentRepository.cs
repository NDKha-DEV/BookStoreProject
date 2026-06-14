// Vị trí: Bookstore.Infrastructure/Repositories/MockPaymentRepository.cs
using System.Collections.Generic;
using Bookstore.Core.Interfaces;
using Bookstore.Core.Models;

namespace Bookstore.Infrastructure.Repositories
{
    public class MockPaymentRepository : IPaymentRepository
    {
        public List<Payment> GetAll() => MockDataStore.Payments;
        public void Add(Payment payment) => MockDataStore.Payments.Add(payment);
    }
}