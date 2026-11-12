using Common.Application;
using FluentValidation;
using Shop.Domain.OrderAgg;
using Shop.Domain.OrderAgg.Repository;
using Shop.Domain.SellerAgg.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.Orders.AddItem
{
    public class AddOrderItemCommand:IBaseCommand
    {
        public AddOrderItemCommand(long inventoryId, long count, long userId)
        {
            InventoryId = inventoryId;
            Count = count;
            UserId = userId;
        }

        public long InventoryId { get; private set; }
        public long Count { get; private set; }
        public long UserId { get; private set; }
    }
    public class AddOrderItemCommandHandler : IBaseCommandHandler<AddOrderItemCommand>
    {
        private readonly IOrderRepository _repository;
        private readonly ISellerRepository _sellerRepository;

        public AddOrderItemCommandHandler(IOrderRepository repository, ISellerRepository sellerRepository)
        {
            _repository = repository;
            _sellerRepository = sellerRepository;
        }
        public async Task<OperationResult> Handle(AddOrderItemCommand request, CancellationToken cancellationToken)
        {
           var inventory=await _sellerRepository.GetInventoryById(request.InventoryId);
            if (inventory == null)
                return OperationResult.NotFound();
            if(inventory.Count<request.Count)
                return OperationResult.Error("تعداد محصولات موجود کمتر از حد درخواستی است.");
            var order = await _repository.GetCurrentUserOrder(request.UserId);
            if(order == null)
            {
                order = new Order(request.UserId);
                order.AddItem(new OrderItem(request.InventoryId,request.Count, inventory.Price));
                _repository.Add(order);
            }
            else
            {
                order.AddItem(new OrderItem(request.InventoryId, request.Count, inventory.Price));
            }
            if(ItemCountBeggerThanInventoryCount(inventory,order))
                return OperationResult.Error("تعداد محصولات موجود کمتر از حد درخواستی است.");
          await  _repository.Save();
            return OperationResult.Success();

        }
        private bool ItemCountBeggerThanInventoryCount(InventoryResult inventory, Order order)
        {
            var orderItem=order.Items.First(f=>f.InventoryId==inventory.Id);
            if (orderItem.Count > inventory.Count)
                return true;
            return false;
        }
    }
    public class AddOrderItemCommandValidator : AbstractValidator<AddOrderItemCommand>
    {
        public AddOrderItemCommandValidator()
        {
            RuleFor(x => x.Count)
            .GreaterThanOrEqualTo(1).WithMessage("تعداد باید بیشتر از 0 باشد");
        }
    }
}
