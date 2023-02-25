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
    public class AuditBookListRepository: IAuditBookListRepository
    {
        #region Variables
        private readonly IMapper _mapper;
        public DataContext _DbContext;
        #endregion

        #region Constructors
        public AuditBookListRepository(DataContext DbContext, IMapper mapper)
        {
            _DbContext = DbContext;
            _mapper = mapper;
        }
        #endregion

        #region METHOD
        public Response DeleteAuditBookList(Guid IdAuditBookList)
        {
            var AuditBookList = _DbContext.AuditBookList.Find(IdAuditBookList);
            if (AuditBookList == null)
            {
                return new Response()
                {
                    Success = false,
                    Fail = true,
                    Message = "Không tìm thấy dữ liệu cần xóa !"
                };
            }

            _DbContext.AuditBookList.Remove(AuditBookList);
            _DbContext.SaveChanges();

            return new Response() { Success = true, Fail = false, Message = "Xóa thành công !" };
        }
        public Response DeleteAuditBookListByList(List<Guid> IdAuditBookList)
        {
            var AuditBookLists = _DbContext.AuditBookList.Where(ar => IdAuditBookList.Contains(ar.Id)).ToList();
            if (!AuditBookLists.Any())
            {
                return new Response()
                {
                    Success = false,
                    Fail = true,
                    Message = "Không tìm thấy dữ liệu cần xóa !"
                };
            }

            _DbContext.AuditBookList.RemoveRange(AuditBookLists);
            _DbContext.SaveChanges();

            return new Response() { Success = true, Fail = false, Message = "Xóa thành công !" };
        }
        public IEnumerable<AuditBookListDto> GetAllAuditBookList(int pageNumber, int pageSize)
        {
            var AuditBookLists = new List<AuditBookList>();
            AuditBookLists = _DbContext.AuditBookList.OrderByDescending(e => e.CreatedDate).ToList();

            if (pageNumber != 0 && pageSize != 0)
            {
                if (pageNumber < 0) { pageNumber = 1; }
                AuditBookLists = AuditBookLists.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            }

            var result = new List<AuditBookListDto>();
            result = _mapper.Map<List<AuditBookListDto>>(AuditBookLists);

            return result;
        }
        public AuditBookListDto GetAllAuditBookListById(Guid IdAuditBookList)
        {
            var AuditBookLists = new AuditBookList();
            AuditBookLists = _DbContext.AuditBookList.Where(e => e.Id == IdAuditBookList).FirstOrDefault();

            var result = new AuditBookListDto();
            result = _mapper.Map<AuditBookListDto>(AuditBookLists);

            return result;
        }
        public Response InsertAuditBookList(AuditBookListDto AuditBookListDto)
        {
            var AuditBookList = _mapper.Map<AuditBookList>(AuditBookListDto);
            AuditBookList.CreatedDate = DateTime.Now;
            AuditBookList.Status = 0;
            AuditBookList.Id = Guid.NewGuid();

            _DbContext.AuditBookList.Add(AuditBookList);
            _DbContext.SaveChanges();

            return new Response()
            {
                Success = true,
                Fail = false,
                Message = "Thêm mới thành công !"
            };
        }
        public Response UpdateAuditBookList(Guid IdAuditBookList, AuditBookListDto AuditBookListDto)
        {
            var AuditBookList = _DbContext.AuditBookList.Find(IdAuditBookList);
            if (AuditBookList == null)
            {
                return new Response()
                {
                    Success = false,
                    Fail = true,
                    Message = "Không tìm thấy phiếu kiểm cần cập nhật !"
                };
            }

            _DbContext.AuditBookList.Update(AuditBookList);
            _DbContext.SaveChanges();

            return new Response() { Success = true, Fail = false, Message = "Cập nhật thành công !" };
        }
        #endregion
    }
}
