using AutoMapper;
using DigitalLibary.Data.Data;
using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Common;
using DigitalLibary.Service.Dto;
using DigitalLibary.Service.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DigitalLibary.Service.Repository.RepositoryIPL
{
    public class CategorySignRepository: ICategorySignRepository
    {
        #region Variables

        private readonly IMapper _mapper;
        public DataContext _DbContext;

        #endregion

        #region Constructors
        public CategorySignRepository(DataContext DbContext, IMapper mapper)
        {
            _DbContext = DbContext;
            _mapper = mapper;
        }
        #endregion

        #region CRUD TABLE CATEGORYSIGN
        public List<CategorySignDto> CategorySignByDocument(Guid IdDocument)
        {
            try
            {
                List<CategorySignDto> categorySignDto = new List<CategorySignDto>();
                IndividualSample individualSample = _DbContext.IndividualSample.AsNoTracking()
                .Where(x => x.IdDocument == IdDocument).FirstOrDefault();

                if (individualSample == null)
                {
                    List<CategorySign> categorySign = _DbContext.CategorySign.AsNoTracking()
                    .Where(e => e.IsDeleted == false && e.IsHided == false
                    ).ToList();

                    categorySignDto = _mapper.Map<List<CategorySignDto>>(categorySign);
                }
                else
                {
                    var IdArray = individualSample.NumIndividual.Split('/');

                    CategorySign categorySign = _DbContext.CategorySign.AsNoTracking()
                    .Where(e => e.IsDeleted == false && e.IsHided == false && e.Id == new Guid(IdArray[1]))
                    .FirstOrDefault();

                    CategorySignDto dto = _mapper.Map<CategorySignDto>(categorySign);
                    categorySignDto.Add(dto);
                }
                return categorySignDto;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Response DeleteCategorySign(Guid Id)
        {
            Response response = new Response();
            try
            {
                CategorySign categorySign = _DbContext.CategorySign.Where(x => x.Id == Id).FirstOrDefault();

                if (categorySign is not null)
                {
                    IndividualSample individualSample = _DbContext.IndividualSample
                        .Where(e => e.NumIndividual == $"{categorySign.SignCode}1/{categorySign.Id}").FirstOrDefault();
                    if (individualSample is not null)
                    {
                        response = new Response()
                        {
                            Success = false,
                            Fail = true,
                            Message = "Mã cá biệt này đã đăng kí cho sách không thể xóa !"
                        };
                        return response;
                    }
                }

                if (categorySign != null)
                {
                    categorySign.IsDeleted = true;
                    _DbContext.CategorySign.Update(categorySign);
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

        public List<Category> getAllCategory()
        {
            try
            {
                List<Category> categories = _DbContext.Category.
                Where(e => e.IsDeleted == false)
                .ToList();

                return categories;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<CategorySignDto> getAllCategorySign()
        {
            try
            {
                List<CategorySign> categorySigns = _DbContext.CategorySign.
                Where(e => e.IsDeleted == false)
                .OrderByDescending(e => e.CreatedDate)
                .ToList();

                List<CategorySignDto> categorySignDtos = new List<CategorySignDto>();
                categorySignDtos = _mapper.Map<List<CategorySignDto>>(categorySigns);
                return categorySignDtos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<CategorySignDto> getAllCategorySign(int pageNumber, int pageSize)
        {
            try
            {
                if(pageNumber == 0 && pageSize == 0)
                {
                    pageNumber = 1;
                    pageSize = 10;
                }
                List<CategorySign> categorySigns = _DbContext.CategorySign.
                Where(e => e.IsDeleted == false )
                .OrderByDescending(e => e.CreatedDate)
                .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                List<CategorySignDto> categorySignDtos = new List<CategorySignDto>();
                categorySignDtos = _mapper.Map<List<CategorySignDto>>(categorySigns);
                return categorySignDtos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public CategorySignDto getCategorySignById(Guid Id)
        {
            try
            {
                CategorySign categorySigns = _DbContext.CategorySign.
                Where(e => e.Id == Id).FirstOrDefault();

                CategorySignDto categorySignDtos = new CategorySignDto();
                categorySignDtos = _mapper.Map<CategorySignDto>(categorySigns);
                return categorySignDtos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Response HideCategorySign(Guid Id, bool check)
        {
            Response response = new Response();
            try
            {
                CategorySign categorySign = _DbContext.CategorySign.Where(x => x.Id == Id).FirstOrDefault();

                if (categorySign is not null)
                {
                    IndividualSample individualSample = _DbContext.IndividualSample
                        .Where(e => e.NumIndividual == $"{categorySign.SignCode}1/{categorySign.Id}").FirstOrDefault();
                    if (individualSample is not null)
                    {
                        response = new Response()
                        {
                            Success = false,
                            Fail = true,
                            Message = "Mã cá biệt này đã đăng kí cho sách không thể ẩn !"
                        };
                        return response;
                    }
                }


                if (categorySign != null)
                {
                    categorySign.IsHided = check;
                    _DbContext.CategorySign.Update(categorySign);
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
                    Message = "Xóa không thành công !"
                };
                return response;
            }
        }

        public Response InsertCategorySign(CategorySignDto categorySignDto)
        {
            Response response = new Response();
            try
            {
                CategorySign categorySign = new CategorySign();
                categorySign = _mapper.Map<CategorySign>(categorySignDto);

                _DbContext.CategorySign.Add(categorySign);
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

        public Response RemoveCategorySign(Guid Id)
        {
            Response response = new Response();
            try
            {
                CategorySign categorySign = _DbContext.CategorySign.Where(x => x.Id == Id).FirstOrDefault();

                if(categorySign is not null)
                {
                    IndividualSample individualSample = _DbContext.IndividualSample
                        .Where(e => e.NumIndividual == $"{categorySign.SignCode}1/{categorySign.Id}").FirstOrDefault();
                    if(individualSample is not null)
                    {
                        response = new Response()
                        {
                            Success = false,
                            Fail = true,
                            Message = "Mã cá biệt này đã đăng kí cho sách không thể xóa !"
                        };
                        return response;
                    }
                }

                if (categorySign != null)
                {
                    _DbContext.CategorySign.Remove(categorySign);
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

        public List<CategorySignDto> SearchCategorySign(string code)
        {
            try
            {
                List<CategorySign> categorySigns = _DbContext.CategorySign.
                Where(e => e.IsDeleted == false && e.IsHided == false && e.SignName.ToLower().Contains(code.ToLower()))
                .ToList();

                if(categorySigns.Count == 0)
                {
                    categorySigns = _DbContext.CategorySign.
                    Where(e => e.IsDeleted == false && e.IsHided == false && e.SignCode.ToLower().Contains(code.ToLower()))
                    .ToList();
                }

                List<CategorySignDto> categorySignDtos = new List<CategorySignDto>();
                categorySignDtos = _mapper.Map<List<CategorySignDto>>(categorySigns);
                return categorySignDtos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Response UpdateCategorySign(CategorySignDto categorySignDto)
        {
            Response response = new Response();
            try
            {
                CategorySign categorySign = new CategorySign();
                categorySign = _DbContext.CategorySign.Where(e => e.Id == categorySignDto.Id).FirstOrDefault();
                if (categorySign != null)
                {
                    // define some col with data concrete
                    categorySign.SignName = String.IsNullOrEmpty(categorySignDto.SignName) ? categorySign.SignName : categorySignDto.SignName;
                    categorySign.SignCode = String.IsNullOrEmpty(categorySignDto.SignCode) ? categorySign.SignCode : categorySignDto.SignCode;
                    categorySign.Status = categorySignDto.Status.HasValue ? categorySignDto.Status : categorySign.Status;
                    categorySign.CreatedDate = categorySignDto.CreatedDate.HasValue ? categorySignDto.CreatedDate : categorySign.CreatedDate;
                    categorySign.CreatedBy = categorySignDto.CreatedBy.HasValue ? categorySignDto.CreatedBy : categorySign.CreatedBy;
                    categorySign.IdCategory = categorySignDto.IdCategory.HasValue ? categorySignDto.IdCategory : categorySign.IdCategory;

                    _DbContext.CategorySign.Update(categorySign);
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
        public List<Tuple<Guid, string, string>> GetIdAndSignCodeCategorySign()
        {
            try
            {
                var result = _DbContext.CategorySign.Where(e => e.IsDeleted == false && e.IsHided == false).Select(e => new Tuple<Guid, string, string>(e.Id, e.SignName, e.SignCode)).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
