using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Domain.OrderAgg
{
    public enum OrderStatus
    {

        Pending,
        Finally,
        Shipping,
        Rejected
    }
}
