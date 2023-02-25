using DigitalLibary.Service.Common;
using DigitalLibary.Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLibary.Service.Repository.IRepository
{
    public interface ISchoolYearRepository
    {
        #region CRUD TABLE SCHOOLYEAR
        List<SchoolYearDto> getSchoolYear(int pageNumber, int pageSize);
        Task<Response> InsertSchoolYear(SchoolYearDto schoolYearDto);
        Task<Response> UpdateSchoolYear(SchoolYearDto schoolYearDto);
        Task<Response> DeleteSchoolYear(Guid Id);
        SchoolYearDto getSchoolYear(Guid Id);
        SchoolYearDto getSchoolYear();
        Response ActiveYear(Guid Id, bool IsActive);
        #endregion
    }
}
