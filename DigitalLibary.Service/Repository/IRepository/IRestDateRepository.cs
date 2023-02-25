using DigitalLibary.Service.Common;
using DigitalLibary.Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLibary.Service.Repository.IRepository
{
    public interface IRestDateRepository
    {
        #region CRUD TABLE RESTDAY
        List<RestDateDto> getRestDay(int pageNumber, int pageSize);
        Task<Response> InsertRestDay(RestDateDto restDateDto);
        Task<Response> UpdateRestDay(RestDateDto restDateDto);
        Task<Response> DeleteRestDay(Guid Id);
        RestDateDto getRestDateDto(Guid Id);
        Response ActiveYear(Guid Id, bool IsActive);
        #endregion
    }
}
