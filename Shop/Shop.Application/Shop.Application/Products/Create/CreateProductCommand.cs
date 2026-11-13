using Common.Application;
using Common.Application.FileUtil.Interfaces;
using Common.Application.Validation;
using Common.Application.Validation.FluentValidations;
using Common.Domain.ValueObjects;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Shop.Application._Utilities;
using Shop.Domain.ProductAgg;
using Shop.Domain.ProductAgg.Repository;
using Shop.Domain.ProductAgg.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.Products.Create
{
    public class CreateProductCommand : IBaseCommand
    {
        public string Title { get; set; }
        public IFormFile ImageFile { get; set; }
        public string Description { get; set; }
        public long CategoryId { get; set; }
        public long SubCategoryId { get; set; }
        public long SecondarySubCategoryId { get; set; }
        public string Slug { get; set; }
        public SeoData SeoData { get; set; }
        public Dictionary<string, string> Specifications { get; set; }
    }
    internal class CreateProductCommandHandler : IBaseCommandHandler<CreateProductCommand>
    {
        private readonly IProductDomainService _domainService;
        private readonly IProductRepository _repository;
        private readonly IFileService _fileService;

        public CreateProductCommandHandler(IProductDomainService domainService, IProductRepository repository, IFileService fileService)
        {
            _domainService = domainService;
            _repository = repository;
            _fileService = fileService;
        }
        public async Task<OperationResult> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var imageName = await _fileService.SaveFileAndGenerateName(request.ImageFile, Directories.ProductImages);
            var product = new Product(request.Title, imageName, request.Description, request.CategoryId,
               request.SubCategoryId, request.SecondarySubCategoryId, _domainService, request.Slug,
               request.SeoData);
            _repository.Add(product);
            var specifications = new List<ProductSpecification>();
            request.Specifications.ToList().ForEach(specification =>
            {
                specifications.Add(new ProductSpecification(specification.Key, specification.Value));
            });
            product.SetSpecification(specifications);
            await _repository.Save();
            return OperationResult.Success();
        }
    }
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(r => r.Title)
                .NotEmpty().WithMessage(ValidationMessages.required("عنوان"));

            RuleFor(r => r.Slug)
               .NotEmpty().WithMessage(ValidationMessages.required("Slug"));

            RuleFor(r => r.Description)
               .NotEmpty().WithMessage(ValidationMessages.required("توضیحات"));

            RuleFor(r => r.ImageFile)
               .JustImageFile();
        }
    }
}
