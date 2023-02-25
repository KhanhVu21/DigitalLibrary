using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitalLibary.Data.Entity;


namespace DigitalLibary.Service.Repository.IRepository
{
    public interface CategoriesRepository
    {
        public List<Category> SearchByName(string SearchString, int pageNumber, int pageSize);
    }
}
