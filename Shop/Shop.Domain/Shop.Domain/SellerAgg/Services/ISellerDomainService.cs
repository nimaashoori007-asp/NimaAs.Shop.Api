using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Domain.SellerAgg.Services
{
    public interface ISellerDomainService
    {
        bool IsValidSellerInformation(Seller seller);
        bool NationalCodeExistInDataBase(string nationalCode);
    }
}
