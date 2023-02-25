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
    public class ContactAndIntroductionRepository : IContactAndIntroductionRepository
    {
        #region Variables

        private readonly IMapper _mapper;
        public DataContext _DbContext;

        #endregion

        #region Constructors
        public ContactAndIntroductionRepository(DataContext DbContext, IMapper mapper)
        {
            _DbContext = DbContext;
            _mapper = mapper;
        }
        #endregion

        #region CRUD TABLE CONTACTANDINTRODUCTION WITH TYPE  = 3
        public Response DeleteRule(Guid Id)
        {
            Response response = new Response();
            try
            {
                ContactAndIntroduction contactAndIntroduction = _DbContext.ContactAndIntroduction.Where(x => x.Id == Id).FirstOrDefault();

                if (contactAndIntroduction != null)
                {
                    contactAndIntroduction.IsDeleted = true;
                    _DbContext.ContactAndIntroduction.Update(contactAndIntroduction);
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
                        Success = true,
                        Fail = false,
                        Message = "Không tìm thấy nội quy !"
                    };
                return response;
            }
            catch (Exception)
            {
                response = new Response()
                {
                    Success = true,
                    Fail = false,
                    Message = "Xóa không thành công !"
                };
                return response;
            }
        }
        public List<ContactAndIntroductionDto> getAllRule(int pageNumber, int pageSize, int type)
        {
            try
            {
                List<ContactAndIntroduction> ruleAll = null;
                if (pageNumber == 0 && pageSize == 0)
                {
                    ruleAll = _DbContext.ContactAndIntroduction.
                    Where(e => e.IsDeleted == false && e.Type == type)
                    .OrderByDescending(e => e.CreateDate)
                    .ToList();
                }
                else
                {
                    ruleAll = _DbContext.ContactAndIntroduction.
                    Where(e => e.IsDeleted == false && e.Type == type)
                    .OrderByDescending(e => e.CreateDate)
                    .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }

                List<ContactAndIntroductionDto> ruleDto = new List<ContactAndIntroductionDto>();
                ruleDto = _mapper.Map<List<ContactAndIntroductionDto>>(ruleAll);
                return ruleDto;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ContactAndIntroductionDto getRuleClient(Guid Id)
        {
            try
            {
                ContactAndIntroduction rule = _DbContext.ContactAndIntroduction.
                Where(e => e.IsActived == true && e.IsDeleted == false && e.Id == Id).FirstOrDefault();

                ContactAndIntroductionDto ruleDto = new ContactAndIntroductionDto();
                ruleDto = _mapper.Map<ContactAndIntroductionDto>(rule);
                return ruleDto;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Response InsertRule(ContactAndIntroductionDto contactAndIntroductionDto)
        {
            Response response = new Response();
            try
            {
                ContactAndIntroduction contactAndIntroduction = new ContactAndIntroduction();
                contactAndIntroduction = _mapper.Map<ContactAndIntroduction>(contactAndIntroductionDto);

                _DbContext.ContactAndIntroduction.Add(contactAndIntroduction);
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
                    Success = true,
                    Fail = false,
                    Message = "Thêm mới không thành công !"
                };
                return response;
            }
        }
        public Response UpdateRule(ContactAndIntroductionDto contactAndIntroductionDto)
        {
            Response response = new Response();
            try
            {
                ContactAndIntroduction rule = new ContactAndIntroduction();
                rule = _DbContext.ContactAndIntroduction.Where(e => e.Id == contactAndIntroductionDto.Id).FirstOrDefault();

                if (rule != null)
                {
                    // define some col with data concrete
                    rule.col = String.IsNullOrEmpty(contactAndIntroductionDto.col) ? rule.col : contactAndIntroductionDto.col;
                    rule.col1 = String.IsNullOrEmpty(contactAndIntroductionDto.col1) ? rule.col1 : contactAndIntroductionDto.col1;
                    rule.col2 = String.IsNullOrEmpty(contactAndIntroductionDto.col2) ? rule.col2 : contactAndIntroductionDto.col2;
                    rule.col3 = String.IsNullOrEmpty(contactAndIntroductionDto.col3) ? rule.col3 : contactAndIntroductionDto.col3;
                    rule.col4 = String.IsNullOrEmpty(contactAndIntroductionDto.col4) ? rule.col4 : contactAndIntroductionDto.col4;
                    rule.col5 = String.IsNullOrEmpty(contactAndIntroductionDto.col5) ? rule.col5 : contactAndIntroductionDto.col5;
                    rule.col6 = String.IsNullOrEmpty(contactAndIntroductionDto.col6) ? rule.col6 : contactAndIntroductionDto.col6;
                    rule.col7 = String.IsNullOrEmpty(contactAndIntroductionDto.col7) ? rule.col7 : contactAndIntroductionDto.col7;
                    rule.col8 = String.IsNullOrEmpty(contactAndIntroductionDto.col8) ? rule.col8 : contactAndIntroductionDto.col8;
                    rule.col9 = String.IsNullOrEmpty(contactAndIntroductionDto.col9) ? rule.col9 : contactAndIntroductionDto.col9;
                    rule.col10 = String.IsNullOrEmpty(contactAndIntroductionDto.col10) ? rule.col10 : contactAndIntroductionDto.col10;
                    rule.Type = contactAndIntroductionDto.Type.HasValue ? contactAndIntroductionDto.Type : rule.Type;

                    _DbContext.ContactAndIntroduction.Update(rule);
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
                        Success = true,
                        Fail = false,
                        Message = "Cập nhật không thành công !"
                    };
                    return response;
                }
            }
            catch (Exception)
            {
                response = new Response()
                {
                    Success = true,
                    Fail = false,
                    Message = "Cập nhật không thành công !"
                };
                return response;
            }
        }
        #endregion
    }
}
