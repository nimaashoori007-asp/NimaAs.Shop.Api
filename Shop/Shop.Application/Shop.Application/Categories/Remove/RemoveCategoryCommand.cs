using Common.Application;
using Shop.Domain.CategoryAgg;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.Categories.Remove
{
    public record RemoveCategoryCommand(long CategoryId) : IBaseCommand;
    public class RemoveCategoryCommandHandler : IBaseCommandHandler<RemoveCategoryCommand>
    {
        private readonly ICategoryRepository _repository;

        public RemoveCategoryCommandHandler(ICategoryRepository categoryRepository)
        {
            _repository = categoryRepository;
        }
        public async Task<OperationResult> Handle(RemoveCategoryCommand request, CancellationToken cancellationToken)
        {
            var result=await _repository.DeleteCategory(request.CategoryId);
            if (result)
            {
               await _repository.Save();
                return OperationResult.Success();
            }
            return OperationResult.Error("امکان حذف این دسته بندی وجود ندارد");
        }
    }
}
