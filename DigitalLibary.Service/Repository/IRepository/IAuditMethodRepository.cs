using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalLibary.Service.Repository.IRepository
{
    public interface IAuditMethodRepository
    {
        IEnumerable<AuditMethodDto> GetAllAuditMethod(int pageNumber, int pageSize);
    }
}
