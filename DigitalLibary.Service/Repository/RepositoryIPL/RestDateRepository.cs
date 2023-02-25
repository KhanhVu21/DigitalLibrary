using AutoMapper;
using DigitalLibary.Data.Data;
using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Common;
using DigitalLibary.Service.Dto;
using DigitalLibary.Service.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLibary.Service.Repository.RepositoryIPL
{
    public class RestDateRepository : IRestDateRepository
    {
        #region Variables

        private readonly IMapper _mapper;
        public DataContext _DbContext;

        #endregion

        #region Constructors
        public RestDateRepository(DataContext DbContext, IMapper mapper)
        {
            _DbContext = DbContext;
            _mapper = mapper;
        }

        #endregion

        #region CRUD TABLE RESTDAY
        public Response ActiveYear(Guid Id, bool IsActive)
        {
            Response response = new Response();
            try
            {
                RestDate restDate = _DbContext.RestDate.Where(x => x.Id == Id).FirstOrDefault();

                if (restDate != null)
                {
                    restDate.IsActived = IsActive;
                    _DbContext.RestDate.Update(restDate);
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
        public async Task<Response> DeleteRestDay(Guid Id)
        {
            Response response = new Response();
            try
            {
                RestDate restDate = _DbContext.RestDate.Where(x => x.Id == Id).FirstOrDefault();

                if (restDate != null)
                {
                    restDate.IsDeleted = true;
                    _DbContext.RestDate.Update(restDate);
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
                        Message = "Không tìm thấy ngày nghỉ !"
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
        public RestDateDto getRestDateDto(Guid Id)
        {
            try
            {
                RestDate restDate = _DbContext.RestDate.
                Where(e => e.Id == Id).FirstOrDefault();

                RestDateDto restDateDto = new RestDateDto();
                restDateDto = _mapper.Map<RestDateDto>(restDate);
                return restDateDto;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<RestDateDto> getRestDay(int pageNumber, int pageSize)
        {
            try
            {
                List<RestDate> restDate = new List<RestDate>();
                if (pageNumber == 0 && pageSize == 0)
                {
                    restDate = _DbContext.RestDate.
                    Where(e => e.IsDeleted == false)
                    .OrderByDescending(e => e.CreatedDate)
                    .ToList();
                }
                else
                {
                    restDate = _DbContext.RestDate.
                    Where(e => e.IsDeleted == false)
                    .OrderByDescending(e => e.CreatedDate)
                    .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }

                List<RestDateDto> restDateList = new List<RestDateDto>();
                restDateList = _mapper.Map<List<RestDateDto>>(restDate);
                return restDateList;
            }
            catch(Exception)
            {
                throw;
            }
        }
        public async Task<Response> InsertRestDay(RestDateDto restDateDto)
        {
            Response response = new Response();
            try
            {
                RestDate restDate = new RestDate();
                restDate = _mapper.Map<RestDate>(restDateDto);

                _DbContext.RestDate.Add(restDate);
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
        public async Task<Response> UpdateRestDay(RestDateDto restDateDto)
        {
            Response response = new Response();
            try
            {
                RestDate restDate = new RestDate();
                restDate = _DbContext.RestDate.Where(e => e.Id == restDateDto.Id).FirstOrDefault();

                if(restDate != null)
                {
                    restDate.NameRestDate = String.IsNullOrEmpty(restDateDto.NameRestDate) ? restDate.NameRestDate : restDateDto.NameRestDate;
                    restDate.FromDate = restDateDto.FromDate.HasValue ? restDateDto.FromDate : restDate.FromDate;
                    restDate.ToDate = restDateDto.ToDate.HasValue ? restDateDto.ToDate : restDate.ToDate;
                    restDate.Status = restDateDto.Status.HasValue ? restDateDto.Status : restDate.Status;
                    restDate.CreatedBy = restDateDto.CreatedBy.HasValue ? restDateDto.CreatedBy : restDate.CreatedBy;
                    restDate.CreatedDate = restDateDto.CreatedDate.HasValue ? restDateDto.CreatedDate : restDate.CreatedDate;
                    restDate.Note = String.IsNullOrEmpty(restDateDto.Note) ? restDate.Note : restDateDto.Note;

                    _DbContext.RestDate.Update(restDate);
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
