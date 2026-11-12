using Common.Application;
using Common.Application.Validation;
using Common.Application.Validation.FluentValidations;
using FluentValidation;
using MediatR;
using Shop.Domain.OrderAgg;
using Shop.Domain.OrderAgg.Repository;
using Shop.Domain.OrderAgg.ValueObjects;
using Shop.Domain.SiteEntities.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.Orders.Checkout
{
    public class CheckoutOrderCommand : IBaseCommand
    {
        public CheckoutOrderCommand(long userId, string shire, string city, string postalCode, string postalAddress,
            string phoneNumber, string name, string family, string nationalCode, long shippingMethodId)
        {
            UserId = userId;
            Shire = shire;
            City = city;
            PostalCode = postalCode;
            PostalAddress = postalAddress;
            PhoneNumber = phoneNumber;
            Name = name;
            Family = family;
            NationalCode = nationalCode;
            ShippingMethodId = shippingMethodId;
        }
        public long UserId { get; private set; }
        public string Shire { get; private set; }
        public string City { get; private set; }
        public string PostalCode { get; private set; }
        public string PostalAddress { get; private set; }
        public string PhoneNumber { get; private set; }
        public string Name { get; private set; }
        public string Family { get; private set; }
        public string NationalCode { get; private set; }
        public long ShippingMethodId { get; private set; }
    }
    public class CheckoutOrderCommandHandler : IBaseCommandHandler<CheckoutOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IShippingMethodRepository _shippingMethodRepository;

        public CheckoutOrderCommandHandler(IOrderRepository orderRepository, IShippingMethodRepository shippingMethodRepository)
        {
            _orderRepository = orderRepository;
            _shippingMethodRepository = shippingMethodRepository;
        }

        public async Task<OperationResult> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
        {
            var currentOrder = await _orderRepository.GetCurrentUserOrder(request.UserId);
            if (currentOrder == null)
                return OperationResult.NotFound();
            var address = new OrderAddress(request.Shire, request.City, request.PostalCode, request.PostalAddress,
                request.PhoneNumber, request.Name, request.Family, request.NationalCode);
            var shippingMethod = await _shippingMethodRepository.GetAsync(request.ShippingMethodId);
            if (shippingMethod == null)
                return OperationResult.Error();
            currentOrder.Checkout(address, new OrderShippingMethod(shippingMethod.Title, shippingMethod.Cost));
            await _orderRepository.Save();
            return OperationResult.Success();
        }
    }
    public class CheckoutOrderCommandValidator : AbstractValidator<CheckoutOrderCommand>
    {
        public CheckoutOrderCommandValidator()
        {
            RuleFor(f => f.Name)
                .NotNull()
                .NotEmpty()
                .WithMessage(ValidationMessages.required("نام"));

            RuleFor(f => f.Family)
               .NotNull()
               .NotEmpty()
               .WithMessage(ValidationMessages.required("نام خانوادگی"));

            RuleFor(f => f.City)
               .NotNull()
               .NotEmpty()
               .WithMessage(ValidationMessages.required("شهر"));

            RuleFor(f => f.Shire)
               .NotNull()
               .NotEmpty()
               .WithMessage(ValidationMessages.required("استان"));

            RuleFor(f => f.PostalAddress)
              .NotNull()
              .NotEmpty()
              .WithMessage(ValidationMessages.required("آدرس"));

            RuleFor(f => f.PostalCode)
             .NotNull()
             .NotEmpty()
             .WithMessage(ValidationMessages.required("کدپستی"));

            RuleFor(f => f.PhoneNumber)
              .NotNull()
              .NotEmpty()
              .WithMessage(ValidationMessages.required("شماره"))
              .MaximumLength(11).WithMessage("شماره موبایل نامعتبر است")
              .MinimumLength(11).WithMessage("شماره موبایل نامعتبر است");

            RuleFor(f => f.NationalCode)
             .NotNull()
             .NotEmpty()
             .WithMessage(ValidationMessages.required("کد ملی"))
             .MaximumLength(10).WithMessage(" کدملی نامعتبر است")
             .MinimumLength(10).WithMessage("کدملی نامعتبر است")
             .ValidNationalId();
        }
    }
}
