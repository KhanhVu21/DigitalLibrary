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
using System.Text;
using System.Threading.Tasks;


namespace DigitalLibary.Service.Repository.RepositoryIPL
{
    public class CategoryPublisherRepositoryImpl : ICategoryPublisherRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public CategoryPublisherRepositoryImpl(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public bool CheckPublisherCodeUpdate(CategoryPublisher categoryPublisherModel)
        {
            try
            {

                CategoryPublisher categoryPublisher = _context.CategoryPublisher
                .Where(e => e.Id == categoryPublisherModel.Id).FirstOrDefault();

                if (categoryPublisherModel.PublisherCode != categoryPublisher.PublisherCode)
                {
                    CategoryPublisher categoryPublisherTemp = _context.CategoryPublisher
                    .Where(e => e.PublisherCode == categoryPublisherModel.PublisherCode).FirstOrDefault();

                    if (categoryPublisherTemp != null)
                    {
                        return true;
                    }
                    else return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool CheckPublisherCode(string PublishCode)
        {
            try
            {
                CategoryPublisher categoryPublisher = _context.CategoryPublisher.Where(e => e.PublisherCode == PublishCode).FirstOrDefault();

                if (categoryPublisher != null)
                {
                    return true;
                }
                else return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Response InsertCategoryPublisher(CategoryPublisherDto categoryPublisherDto)
        {
            Response response = new Response();
            try
            {
                CategoryPublisher categoryPublisher = new CategoryPublisher();
                categoryPublisher = _mapper.Map<CategoryPublisher>(categoryPublisherDto);

                _context.CategoryPublisher.Add(categoryPublisher);
                _context.SaveChanges();

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

        List<CategoryPublisher> ICategoryPublisherRepository.SearchByName(string SearchString, int pageNumber, int pageSize)
        {
            var category = from c in _context.CategoryPublisher
                           where EF.Functions.Like(c.PublisherName, "%" + SearchString + "%")
                           select c;
            List<CategoryPublisher> ReturnList = category.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            if (category == null)
            {
                return null;
            }

            return ReturnList;
        }

        public CategoryPublisher GetPublishById(Guid id)
        {
            try
            {
                CategoryPublisher categoryPublisher = _context.CategoryPublisher.
                Where(e => e.Id == id).FirstOrDefault();

                return categoryPublisher;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Response UpdateCategoryPublisher(CategoryPublisher categoryPublisher)
        {
            Response response = new Response();
            try
            {
                CategoryPublisher categoryPublisher1 = new CategoryPublisher();
                categoryPublisher1 = _context.CategoryPublisher.Where(e => e.Id == categoryPublisher.Id).FirstOrDefault();

                if (categoryPublisher.PublisherCode != categoryPublisher1.PublisherCode)
                {
                    CategoryPublisher categoryPublisher2 = new CategoryPublisher();
                    categoryPublisher2 = _context.CategoryPublisher.Where(e => e.PublisherCode == categoryPublisher.PublisherCode
                    && e.IsDeleted == false).FirstOrDefault();

                    if (categoryPublisher2 != null)
                    {
                        response = new Response()
                        {
                            Success = false,
                            Fail = true,
                            Message = "Mã kí hiệu này đã tồn tại !"
                        };
                        return response;
                    }
                }

                if (categoryPublisher1 != null)
                {
                    // define some col with data concrete
                    categoryPublisher1.IdCategory = categoryPublisher.IdCategory.HasValue ? categoryPublisher.IdCategory : categoryPublisher1.IdCategory;
                    categoryPublisher1.PublisherCode = String.IsNullOrEmpty(categoryPublisher.PublisherCode) ? categoryPublisher1.PublisherCode : categoryPublisher.PublisherCode;
                    categoryPublisher1.PublisherName = String.IsNullOrEmpty(categoryPublisher.PublisherName) ? categoryPublisher1.PublisherName : categoryPublisher.PublisherName;
                    categoryPublisher1.Address = String.IsNullOrEmpty(categoryPublisher.Address) ? categoryPublisher1.Address : categoryPublisher.Address;
                    categoryPublisher1.Note = String.IsNullOrEmpty(categoryPublisher.Note) ? categoryPublisher1.Note : categoryPublisher.Note;

                    _context.CategoryPublisher.Update(categoryPublisher1);
                    _context.SaveChanges();

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

        public List<Tuple<string, string>> GetAllPublisher()
        {
            try
            {
                var result = _context.CategoryPublisher.Select(e => new Tuple<string, string>(e.PublisherName, e.PublisherCode)).ToList();
                return result;
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}
