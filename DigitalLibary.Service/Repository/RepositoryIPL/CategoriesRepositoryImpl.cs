using System;
using System.Collections.Generic;
using System.Linq;
using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Repository.IRepository;
using DigitalLibary.Data.Data;
using Microsoft.EntityFrameworkCore;



namespace DigitalLibary.Service.Repository.RepositoryIPL
{
    public class CategoriesRepositoryImpl : CategoriesRepository
    {
        private readonly DataContext _context;

        public CategoriesRepositoryImpl(DataContext context)
        {
            _context = context;
        }

       public List<Category>  SearchByName(string SearchString, int pageNumber, int pageSize)
        {

            var category = from c in _context.Category
                           where EF.Functions.Like(c.CategoryName, "%"+SearchString+"%")
                           select c;
            List<Category> returnList = category.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            if (category == null)
             {
                 return null;
             }

            return returnList;
        }
    }
}
