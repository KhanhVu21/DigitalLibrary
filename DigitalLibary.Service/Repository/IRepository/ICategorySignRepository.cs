using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Common;
using DigitalLibary.Service.Dto;
using System;
using System.Collections.Generic;

namespace DigitalLibary.Service.Repository.IRepository
{
    public interface ICategorySignRepository
    {
        #region CRUD TABLE CATEGORYSIGN
        List<Category> getAllCategory();
        List<CategorySignDto> getAllCategorySign(int pageNumber, int pageSize);
        CategorySignDto getCategorySignById(Guid Id);
        Response InsertCategorySign(CategorySignDto categorySignDto);
        Response UpdateCategorySign(CategorySignDto categorySignDto);
        Response DeleteCategorySign(Guid Id);
        Response HideCategorySign(Guid Id, bool check);
        List<CategorySignDto> getAllCategorySign();
        List<CategorySignDto> SearchCategorySign(string code);
        Response RemoveCategorySign(Guid Id);
        public List<CategorySignDto> CategorySignByDocument(Guid IdDocument);
        List<Tuple<Guid, string,string>> GetIdAndSignCodeCategorySign();
        #endregion
    }
}
