using DigitalLibary.Service.Common;
using DigitalLibary.Service.Dto;
using System;
using System.Collections.Generic;

namespace DigitalLibary.Service.Repository.IRepository
{
    public interface ICategorySign_V1Repository
    {
        List<CategorySign_V1Dto> getAllCategorySignV1(int pageNumber, int pageSize);
        List<CategorySign_V1Dto> getAllCategorySignV1();
        CategorySign_V1Dto getAllCategorySignV1ById(Guid Id);
        Response InsertCategorySignV1(CategorySign_V1Dto categorySign_V1);
        Response UpdateCategorySignV1(CategorySign_V1Dto categorySign_V1);
        Response DeleteCategorySignV1(Guid Id);
        Response HideCategorySignV1(Guid Id, bool check);
        Boolean CheckExitsCategorySignCode(string signCode);
    }
}
