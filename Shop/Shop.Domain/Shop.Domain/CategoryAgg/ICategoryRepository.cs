using Common.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Domain.CategoryAgg
{
    public interface ICategoryRepository:IBaseRepository<Category>
    {
        Task<bool> DeleteCategory(long categoryId);
    }
}
