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

namespace Shop.Application.Products.Edit
{
    public class EditProductCommand : IBaseCommand
    {
        public EditProductCommand(long productId, string title, IFormFile? imageFile, string description, long categoryId, long subCategoryId, long secondarySubCategoryId, string slug, SeoData seoData, Dictionary<string, string> specifications)
        {
            ProductId = productId;
            Title = title;
            ImageFile = imageFile;
            Description = description;
            CategoryId = categoryId;
            SubCategoryId = subCategoryId;
            SecondarySubCategoryId = secondarySubCategoryId;
            Slug = slug;
            SeoData = seoData;
            Specifications = specifications;
        }

        public long ProductId { get; private set; }
        public string Title { get; private set; }
        public IFormFile? ImageFile { get; private set; }
        public string Description { get; private set; }
        public long CategoryId { get; private set; }
        public long SubCategoryId { get; private set; }
        public long SecondarySubCategoryId { get; private set; }
        public string Slug { get; private set; }
        public SeoData SeoData { get; private set; }
        public Dictionary<string, string> Specifications { get; private set; }
    }
    internal class EditProductCommandHandler : IBaseCommandHandler<EditProductCommand>
    {
        private readonly IProductDomainService _domainService;
        private readonly IProductRepository _repository;
        private readonly IFileService _fileService;

        public EditProductCommandHandler(IProductDomainService domainService, IProductRepository repository, IFileService fileService)
        {
            _domainService = domainService;
            _repository = repository;
            _fileService = fileService;
        }
        public async Task<OperationResult> Handle(EditProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetTracking(request.ProductId);
            if (product == null)
                return OperationResult.NotFound();

            product.Edit(request.Title, request.Description, request.CategoryId, request.SubCategoryId,
                request.SecondarySubCategoryId, request.Slug, _domainService, request.SeoData);
            var oldImage = product.ImageName;
            if (request.ImageFile != null)
            {
                var imageName = await _fileService.SaveFileAndGenerateName(request.ImageFile, Directories.ProductImages);
                product.SetProductImage(imageName);
            }
            var specifications = new List<ProductSpecification>();
            request.Specifications.ToList().ForEach(specification =>
            {
                specifications.Add(new ProductSpecification(specification.Key, specification.Value));
            });
            product.SetSpecification(specifications);
            await _repository.Save();
            RemoveOldImage(request.ImageFile, oldImage);
            return OperationResult.Success();
        }
        private void RemoveOldImage(IFormFile imageFile, string oldImageName)
        {
            if(imageFile != null)
            {
                _fileService.DeleteFile(Directories.ProductImages, oldImageName);
            }

        }
    }
    public class EditProductCommandValidator : AbstractValidator<EditProductCommand>
    {
        public EditProductCommandValidator()
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
