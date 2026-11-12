using Common.Application;
using Shop.Domain.OrderAgg.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.Orders.Finally
{
    public record OrderFinallyCommand(long OrderId) : IBaseCommand;
    public class OrderFinallyCommandHandler : IBaseCommandHandler<OrderFinallyCommand>
    {
        private readonly IOrderRepository _repository;
        public OrderFinallyCommandHandler(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<OperationResult> Handle(OrderFinallyCommand request, CancellationToken cancellationToken)
        {
            var order = await _repository.GetTracking(request.OrderId);
            if (order == null)
                return OperationResult.NotFound();

            order.Finally();
            await _repository.Save();
            return OperationResult.Success();
        }
    }
}
