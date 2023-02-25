using DigitalLibary.Data.Data;
using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Common;
using DigitalLibary.Service.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DigitalLibary.Service.Repository.RepositoryIPL
{
    public class ICategorySupplierImpl : ICategorySupplier
    {
        private readonly DataContext _context;

        public ICategorySupplierImpl(DataContext context)
        {
            _context = context;
        }

        public bool CheckExitsSupplierCode(string supplieCode)
        {
            try
            {
                CategorySupplier categorySupplier = _context.CategorySupplier.Where(c => c.SupplierCode == supplieCode).FirstOrDefault();

                if (categorySupplier != null)
                {
                    return true;
                }
                else return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Response UpdateCategorySupplier(CategorySupplier categorySupplier)
        {
            Response response = new Response();
            try
            {
                CategorySupplier categorySupplier1 = new CategorySupplier();
                categorySupplier1 = _context.CategorySupplier.Where(e => e.Id == categorySupplier.Id).FirstOrDefault();

                if (categorySupplier.SupplierCode != categorySupplier1.SupplierCode)
                {
                    CategorySupplier categorySignTemp = _context.CategorySupplier.Where(e => e.IsDeleted == false
                    && e.SupplierCode == categorySupplier.SupplierCode).FirstOrDefault();

                    if (categorySignTemp != null)
                    {
                        response = new Response()
                        {
                            Success = false,
                            Fail = true,
                            Message = "Mã kí hiệu này đã tồn tại !"
                        };
                        return response;
                    }
                }

                if (categorySupplier1 != null)
                {
                    // define some col with data concrete
                    categorySupplier1.SupplierCode = String.IsNullOrEmpty(categorySupplier.SupplierCode) ? categorySupplier1.SupplierCode : categorySupplier.SupplierCode;
                    categorySupplier1.SupplierName = String.IsNullOrEmpty(categorySupplier.SupplierName) ? categorySupplier1.SupplierName : categorySupplier.SupplierName;
                    categorySupplier1.TaxCode = String.IsNullOrEmpty(categorySupplier.TaxCode) ? categorySupplier1.TaxCode : categorySupplier.TaxCode;
                    categorySupplier1.Address = String.IsNullOrEmpty(categorySupplier.Address) ? categorySupplier1.Address : categorySupplier.Address;
                    categorySupplier1.Note = String.IsNullOrEmpty(categorySupplier.Note) ? categorySupplier1.Note : categorySupplier.Note;


                    _context.CategorySupplier.Update(categorySupplier1);
                    _context.SaveChanges();

                    response = new Response()
                    {
                        Success = true,
                        Fail = false,
                        Message = "Cập nhật thành công !"
                    };
                    return response;
                }
                else
                {
                    response = new Response()
                    {
                        Success = false,
                        Fail = true,
                        Message = "Cập nhật không thành công !"
                    };
                    return response;
                }
            }
            catch (Exception)
            {
                response = new Response()
                {
                    Success = false,
                    Fail = true,
                    Message = "Cập nhật không thành công !"
                };
                return response;
            }
        }

        List<CategorySupplier> ICategorySupplier.SearchByName(string SearchString, int pageNumber, int pageSize)
        {

            var category = from c in _context.CategorySupplier
                           where EF.Functions.Like(c.SupplierName, "%" + SearchString + "%")
                           select c;
            List<CategorySupplier> returnList = category.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            if (category == null)
            {
                return null;
            }

            return returnList;
        }
    }
}
