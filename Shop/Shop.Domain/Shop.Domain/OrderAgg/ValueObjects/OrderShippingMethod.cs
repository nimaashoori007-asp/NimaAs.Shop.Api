using Common.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Domain.OrderAgg.ValueObjects
{
    public class OrderShippingMethod:ValueObject
    {
        public OrderShippingMethod(string shippingType, int shippingCost)
        {
            ShippingType = shippingType;
            ShippingCost = shippingCost;
        }

        public string ShippingType { get; private set; }
        public int ShippingCost { get; private set; }
        
    }
}
