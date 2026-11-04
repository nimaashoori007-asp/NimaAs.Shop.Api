using Common.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Domain.SiteEntities.Repositories
{
    public interface IBannerRepository : IBaseRepository<Banner>
    {
        void Delete(Banner banner);
    }
}
