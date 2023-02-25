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
    public class AuditorListRepository: IAuditorListRepository
    {
        #region Variables
        private readonly IMapper _mapper;
        public DataContext _DbContext;
        #endregion

        #region Constructors
        public AuditorListRepository(DataContext DbContext, IMapper mapper)
        {
            _DbContext = DbContext;
            _mapper = mapper;
        }
        #endregion

        #region METHOD
        public Response DeleteAuditorList(Guid IdAuditorList)
        {
            var AuditorList = _DbContext.AuditorList.Find(IdAuditorList);
            if (AuditorList == null)
            {
                return new Response()
                {
                    Success = false,
                    Fail = true,
                    Message = "Không tìm thấy dữ liệu cần xóa !"
                };
            }

            _DbContext.AuditorList.Remove(AuditorList);
            _DbContext.SaveChanges();

            return new Response() { Success = true, Fail = false, Message = "Xóa thành công !" };
        }
        public Response DeleteAuditorListByList(List<Guid> IdAuditorList)
        {
            var AuditorLists = _DbContext.AuditorList.Where(ar => IdAuditorList.Contains(ar.Id)).ToList();
            if (!AuditorLists.Any())
            {
                return new Response()
                {
                    Success = false,
                    Fail = true,
                    Message = "Không tìm thấy dữ liệu cần xóa !"
                };
            }

            _DbContext.AuditorList.RemoveRange(AuditorLists);
            _DbContext.SaveChanges();

            return new Response() { Success = true, Fail = false, Message = "Xóa thành công !" };
        }
        public IEnumerable<AuditorListDto> GetAllAuditorList(int pageNumber, int pageSize)
        {
            var AuditorLists = new List<AuditorList>();
            AuditorLists = _DbContext.AuditorList.OrderByDescending(e => e.CreatedDate).ToList();

            if (pageNumber != 0 && pageSize != 0)
            {
                if (pageNumber < 0) { pageNumber = 1; }
                AuditorLists = AuditorLists.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            }

            var result = new List<AuditorListDto>();
            result = _mapper.Map<List<AuditorListDto>>(AuditorLists);

            return result;
        }
        public AuditorListDto GetAllAuditorListById(Guid IdAuditorList)
        {
            var AuditorLists = new AuditorList();
            AuditorLists = _DbContext.AuditorList.Where(e => e.Id == IdAuditorList).FirstOrDefault();

            var result = new AuditorListDto();
            result = _mapper.Map<AuditorListDto>(AuditorLists);

            return result;
        }
        public Response InsertAuditorList(AuditorListDto AuditorListDto)
        {
            var AuditorList = _mapper.Map<AuditorList>(AuditorListDto);
            AuditorList.CreatedDate = DateTime.Now;
            AuditorList.Status = 0;
            AuditorList.Id = Guid.NewGuid();

            _DbContext.AuditorList.Add(AuditorList);
            _DbContext.SaveChanges();

            return new Response()
            {
                Success = true,
                Fail = false,
                Message = "Thêm mới thành công !"
            };
        }
        public Response UpdateAuditorList(Guid IdAuditorList, AuditorListDto AuditorListDto)
        {
            var AuditorList = _DbContext.AuditorList.Find(IdAuditorList);
            if (AuditorList == null)
            {
                return new Response()
                {
                    Success = false,
                    Fail = true,
                    Message = "Không tìm thấy phiếu kiểm cần cập nhật !"
                };
            }

            AuditorList.IdUser = AuditorListDto.IdUser;
            AuditorList.DescriptionRole = AuditorListDto.DescriptionRole;
            AuditorList.IdAuditReceipt = AuditorListDto.IdAuditReceipt;

            _DbContext.AuditorList.Update(AuditorList);
            _DbContext.SaveChanges();

            return new Response() { Success = true, Fail = false, Message = "Cập nhật thành công !" };
        }
        #endregion
    }
}
