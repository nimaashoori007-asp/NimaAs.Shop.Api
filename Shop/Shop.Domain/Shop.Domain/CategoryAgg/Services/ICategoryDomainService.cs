using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Domain.CategoryAgg.Services
{
    public interface ICategoryDomainService
    {
        public bool IsSlugExist(string slug);
    }
}
