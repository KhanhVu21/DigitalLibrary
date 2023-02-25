using DigitalLibary.Service.Common;
using DigitalLibary.Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLibary.Service.Repository.IRepository
{
    public interface IAuditorListRepository
    {
        IEnumerable<AuditorListDto> GetAllAuditorList(int pageNumber, int pageSize);
        AuditorListDto GetAllAuditorListById(Guid IdAuditorList);
        Response InsertAuditorList(AuditorListDto AuditorListDto);
        Response UpdateAuditorList(Guid IdAuditorList, AuditorListDto AuditorListDto);
        Response DeleteAuditorList(Guid IdAuditorList);
        Response DeleteAuditorListByList(List<Guid> IdAuditorList);
    }
}
