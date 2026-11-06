using Common.Application;
using Common.Application.Validation;
using Common.Domain.ValueObjects;
using FluentValidation;
using Shop.Domain.CategoryAgg;
using Shop.Domain.CategoryAgg.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.Categories.AddChild
{
    public record AddChildCategoryCommand(long ParentId, string Title, 
        string Slug, SeoData SeoData) : IBaseCommand<long>;

    public class AddChildCategoryCommandHandler : IBaseCommandHandler<AddChildCategoryCommand, long>
    {
        private readonly ICategoryRepository _repository;
        private readonly ICategoryDomainService _domainServicer;

        public AddChildCategoryCommandHandler(ICategoryRepository repository, ICategoryDomainService domainServicer)
        {
            _repository = repository;
            _domainServicer = domainServicer;
        }
        public async Task<OperationResult<long>> Handle(AddChildCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _repository.GetTracking(request.ParentId);
            if (category == null)
                return OperationResult<long>.NotFound();
            category.AddChild(request.Title, request.Slug, request.SeoData, _domainServicer);
            await _repository.Save();
            return OperationResult<long>.Success(category.Id);


        }
        public class AddChildCategoryCommandValidator : AbstractValidator<AddChildCategoryCommand>
        {
            public AddChildCategoryCommandValidator()
            {
                RuleFor(r => r.Title)
            .NotNull().NotEmpty().WithMessage(ValidationMessages.required("عنوان"));

                RuleFor(r => r.Slug)
                  .NotNull().NotEmpty().WithMessage(ValidationMessages.required("Slug"));
            }
        }

    }
}
