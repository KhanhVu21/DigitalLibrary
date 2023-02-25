using DigitalLibary.Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLibary.Service.Repository.IRepository
{
    public interface IStatusBookRepository
    {
        IEnumerable<StatusBookDto> GetAllStatusBook(int pageNumber, int pageSize, int Type);
    }
}
