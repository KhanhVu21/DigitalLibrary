using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Common;
using System;
using System.Collections.Generic;

namespace DigitalLibary.Service.Repository.IRepository
{
    public interface ICategorySupplier
    {
        public List<CategorySupplier> SearchByName(String SearchString, int pageNumber, int pageSize);
        Boolean CheckExitsSupplierCode(string supplieCode);

        Response UpdateCategorySupplier(CategorySupplier categorySupplier);
    }
}
