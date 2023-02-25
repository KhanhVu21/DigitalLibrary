using AutoMapper;
using DigitalLibary.Data.Data;
using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Common;
using DigitalLibary.Service.Dto;
using DigitalLibary.Service.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalLibary.Service.Repository.RepositoryIPL
{
    public class SchoolYearRepository : ISchoolYearRepository
    {
        #region Variables

        private readonly IMapper _mapper;
        public DataContext _DbContext;

        #endregion

        #region Constructors
        public SchoolYearRepository(DataContext DbContext, IMapper mapper)
        {
            _DbContext = DbContext;
            _mapper = mapper;
        }
        #endregion

        #region CRUD TABLE SCHOOLYEAR
        public async Task<Response> DeleteSchoolYear(Guid Id)
        {
            Response response = new Response();
            try
            {
                SchoolYear schoolYear  = _DbContext.SchoolYear.Where(x => x.Id == Id).FirstOrDefault();

                if (schoolYear != null)
                {
                    _DbContext.SchoolYear.Remove(schoolYear);
                    await _DbContext.SaveChangesAsync();

                    response = new Response()
                    {
                        Success = true,
                        Fail = false,
                        Message = "Xóa thành công !"
                    };
                    return response;
                }
                else
                    response = new Response()
                    {
                        Success = false,
                        Fail = true,
                        Message = "Không tìm thấy kết quả !"
                    };
                return response;
            }
            catch (Exception)
            {
                response = new Response()
                {
                    Success = false,
                    Fail = true,
                    Message = "Xóa không thành công !"
                };
                return response;
            }
        }
        public List<SchoolYearDto> getSchoolYear(int pageNumber, int pageSize)
        {
            try
            {
                List<SchoolYear> schoolYears = new List<SchoolYear>();
                if (pageNumber == 0 && pageSize == 0)
                {
                    schoolYears = _DbContext.SchoolYear.
                    Where(e => e.Id != Guid.Empty)
                    .OrderByDescending(e => e.CreatedDate)
                    .ToList();
                }
                else
                {
                    schoolYears = _DbContext.SchoolYear.
                    Where(e => e.Id != Guid.Empty)
                    .OrderByDescending(e => e.CreatedDate)
                    .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }

                List<SchoolYearDto> schoolYearsLst = new List<SchoolYearDto>();
                schoolYearsLst = _mapper.Map<List<SchoolYearDto>>(schoolYears);
                return schoolYearsLst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public SchoolYearDto getSchoolYear(Guid Id)
        {
            try
            {
                SchoolYear schoolYear = _DbContext.SchoolYear.
                Where(e => e.Id == Id).FirstOrDefault();

                SchoolYearDto schoolYearDto = new SchoolYearDto();
                schoolYearDto = _mapper.Map<SchoolYearDto>(schoolYear);
                return schoolYearDto;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public SchoolYearDto getSchoolYear()
        {
            try
            {
                SchoolYear schoolYear = _DbContext.SchoolYear.
                Where(e => e.IsActived == true)
                .OrderByDescending(e => e.CreatedDate)
                .FirstOrDefault();

                SchoolYearDto schoolYearDto = new SchoolYearDto();
                schoolYearDto = _mapper.Map<SchoolYearDto>(schoolYear);
                return schoolYearDto;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Response ActiveYear(Guid Id, bool IsActive)
        {
            Response response = new Response();
            try
            {
                SchoolYear schoolYear = _DbContext.SchoolYear.Where(x => x.Id == Id).FirstOrDefault();

                if (schoolYear != null)
                {
                    schoolYear.IsActived = IsActive;
                    _DbContext.SchoolYear.Update(schoolYear);
                    _DbContext.SaveChanges();

                    response = new Response()
                    {
                        Success = true,
                        Fail = false,
                        Message = "Kích hoạt thành công !"
                    };
                    return response;
                }
                else
                    response = new Response()
                    {
                        Success = false,
                        Fail = true,
                        Message = "Không tìm thấy kết quả !"
                    };
                return response;
            }
            catch (Exception)
            {
                response = new Response()
                {
                    Success = false,
                    Fail = true,
                    Message = "Kích hoạt không thành công !"
                };
                return response;
            }
        }
        public async Task<Response> InsertSchoolYear(SchoolYearDto schoolYearDto)
        {
            Response response = new Response();
            try
            {
                SchoolYear schoolYear = new SchoolYear();
                schoolYear = _mapper.Map<SchoolYear>(schoolYearDto);

                _DbContext.SchoolYear.Add(schoolYear);
                await _DbContext.SaveChangesAsync();

                response = new Response()
                {
                    Success = true,
                    Fail = false,
                    Message = "Thêm mới thành công !"
                };
                return response;
            }
            catch (Exception)
            {
                response = new Response()
                {
                    Success = false,
                    Fail = true,
                    Message = "Thêm mới không thành công !"
                };
                return response;
            }
        }
        public async Task<Response> UpdateSchoolYear(SchoolYearDto schoolYearDto)
        {
            Response response = new Response();
            try
            {
                SchoolYear schoolYear = new SchoolYear();
                schoolYear = _DbContext.SchoolYear.Where(e => e.Id == schoolYearDto.Id).FirstOrDefault();

                if (schoolYear != null)
                {
                    schoolYear.StartSemesterI = schoolYearDto.StartSemesterI.HasValue ? schoolYearDto.StartSemesterI : schoolYear.StartSemesterI;
                    schoolYear.StartSemesterII = schoolYearDto.StartSemesterII.HasValue ? schoolYearDto.StartSemesterII : schoolYear.StartSemesterII;
                    schoolYear.EndAllSemester = schoolYearDto.EndAllSemester.HasValue ? schoolYearDto.EndAllSemester : schoolYear.EndAllSemester;
                    schoolYear.IsActived = schoolYearDto.IsActived.HasValue ? schoolYearDto.IsActived : schoolYear.IsActived;
                    schoolYear.Status = schoolYearDto.Status.HasValue ? schoolYearDto.Status : schoolYear.Status;
                    schoolYear.CreatedBy = schoolYearDto.CreatedBy.HasValue ? schoolYearDto.CreatedBy : schoolYear.CreatedBy;
                    schoolYear.CreatedDate = schoolYearDto.CreatedDate.HasValue ? schoolYearDto.CreatedDate : schoolYear.CreatedDate;
                    schoolYear.FromYear = schoolYear.StartSemesterI;
                    schoolYear.ToYear = schoolYear.EndAllSemester;

                    _DbContext.SchoolYear.Update(schoolYear);
                    await _DbContext.SaveChangesAsync();

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
        #endregion
    }
}
