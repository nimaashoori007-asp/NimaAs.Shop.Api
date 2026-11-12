using Common.Domain;
using Common.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Domain.OrderAgg
{
    public class OrderItem:BaseEntity
    {
        public OrderItem(long inventoryId, long count, long price)
        {
            PriceGuard(price);
            CountGuard(count);

            InventoryId = inventoryId;
            Count = count;
            Price = price;
        }

        public long OrderId { get; internal set; }
        public long InventoryId { get; private set; }
        public long Count { get; private set; }
        public long Price { get; private set; }
        public long TotalPrice => Price * Count;
        public void IncreaseCount(long count)
        {
            Count += count;
        }
        public void DecreaseCount(long count)
        {
            if (Count == 1)
                return;
            if(Count-count<=0) 
                return;
            Count -= count;
        }
        public void ChangeCount(long newCount)
        {
            CountGuard(newCount);
            Count = newCount;
        }
        public void SetPrice(long newPrice)
        {
            PriceGuard(newPrice);
            Price = newPrice;
        }
        public void PriceGuard(long newPrice)
        {
            if(newPrice<1)
                throw new InvalidDomainDataException("مبلغ کالا نامعتبر است");
        }
        public void CountGuard(long count)
        {
            if (count < 1)
                throw new InvalidDomainDataException();
        }
    }
}
