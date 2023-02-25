using AutoMapper;
using DigitalLibary.Data.Data;
using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Common;
using DigitalLibary.Service.Dto;
using DigitalLibary.Service.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DigitalLibary.Service.Repository.RepositoryIPL
{
    public class SlideRepository : ISlideRepository
    {
        #region Variables

        private readonly IMapper _mapper;
        public DataContext _DbContext;
        #endregion

        #region Constructors
        public SlideRepository(DataContext DbContext, IMapper mapper)
        {
            _DbContext = DbContext;
            _mapper = mapper;
        }

        #endregion

        #region Method
        public List<SlideDto> GetAllSlideClient(int pageNumber, int pageSize)
        {
            try
            {
                List<Slide> slideLEntity = new List<Slide>();
                if(pageNumber != 0 && pageSize != 0)
                {
                    slideLEntity = _DbContext.Slide.Where(e => e.IsDelete == false && e.IsHide == false)
                    .OrderByDescending(e => e.CreatedDate)
                    .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }
                else
                {
                    slideLEntity = _DbContext.Slide.Where(e => e.IsDelete == false && e.IsHide == false)
                    .OrderByDescending(e => e.CreatedDate)
                    .ToList();
                }

                List<SlideDto> slideList = new List<SlideDto>();
                slideList = _mapper.Map<List<SlideDto>>(slideLEntity);

                return slideList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<SlideDto> GetAllSlideAdmin(int pageNumber, int pageSize)
        {
            try
            {
                List<Slide> slideLEntity = new List<Slide>();
                if (pageNumber != 0 && pageSize != 0)
                {
                    slideLEntity = _DbContext.Slide.Where(e => e.IsDelete == false)
                    .OrderByDescending(e => e.CreatedDate)
                    .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }
                else
                {
                    slideLEntity = _DbContext.Slide.Where(e => e.IsDelete == false)
                    .OrderByDescending(e => e.CreatedDate)
                    .ToList();
                }
                

                List<SlideDto> slideList = new List<SlideDto>();
                slideList = _mapper.Map<List<SlideDto>>(slideLEntity);

                return slideList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Response DeleteSlide(Guid Id)
        {
            Response response = new Response();
            try
            {
                Slide slide = _DbContext.Slide.Where(x => x.Id == Id).FirstOrDefault();

                if (slide != null)
                {
                    slide.IsDelete = true;
                    _DbContext.Slide.Update(slide);
                    _DbContext.SaveChanges();

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

        public Response HideSlide(Guid Id, bool check)
        {
            Response response = new Response();
            try
            {
                Slide slide = _DbContext.Slide.Where(x => x.Id == Id).FirstOrDefault();

                if (slide != null)
                {
                    slide.IsHide = check;
                    _DbContext.Slide.Update(slide);
                    _DbContext.SaveChanges();

                    response = new Response()
                    {
                        Success = true,
                        Fail = false,
                        Message = "Ẩn thành công !"
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
                    Message = "Ẩn không thành công !"
                };
                return response;
            }
        }

        public Response InsertSlide(SlideDto slideDto)
        {
            Response response = new Response();
            try
            {
                Slide slide = new Slide();
                slide = _mapper.Map<Slide>(slideDto);

                _DbContext.Slide.Add(slide);
                _DbContext.SaveChanges();

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

        public Response UpdateSlide(SlideDto slideDto)
        {
            Response response = new Response();
            try
            {
                Slide slide = new Slide();
                slide = _DbContext.Slide.Where(e => e.Id == slideDto.Id).FirstOrDefault();
                if (slide != null)
                {
                    // define some col with data concrete
                    slide.FileName = String.IsNullOrEmpty(slideDto.FileName) ? slide.FileName : slideDto.FileName;
                    slide.FileNameExtention = String.IsNullOrEmpty(slideDto.FileNameExtention) ? slide.FileNameExtention : slideDto.FileNameExtention;
                    slide.FilePath = String.IsNullOrEmpty(slideDto.FilePath) ? slide.FilePath : slideDto.FilePath;
                    slide.Description = String.IsNullOrEmpty(slideDto.Description) ? slide.Description : slideDto.Description;
                    slide.Title = String.IsNullOrEmpty(slideDto.Title) ? slide.Title : slideDto.Title;

                    _DbContext.Slide.Update(slide);
                    _DbContext.SaveChanges();

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

        public SlideDto getSlideById(Guid Id)
        {
            try
            {
                Slide slide = _DbContext.Slide.
                Where(e => e.Id == Id).FirstOrDefault();

                SlideDto slideDto = new SlideDto();
                slideDto = _mapper.Map<SlideDto>(slide);
                return slideDto;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
