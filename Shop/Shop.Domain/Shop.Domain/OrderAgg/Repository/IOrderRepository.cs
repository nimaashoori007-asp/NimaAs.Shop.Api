using Common.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Domain.OrderAgg.Repository
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        Task<Order> GetCurrentUserOrder(long userId);
    }
}
