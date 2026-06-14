// Vị trí: Bookstore.Core/Interfaces/IPaymentRepository.cs
using System.Collections.Generic;
using Bookstore.Core.Models;

namespace Bookstore.Core.Interfaces
{
    public interface IPaymentRepository
    {
        List<Payment> GetAll();
        void Add(Payment payment);
    }
}