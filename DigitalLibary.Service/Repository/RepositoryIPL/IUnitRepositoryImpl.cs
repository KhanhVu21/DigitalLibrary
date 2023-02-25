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
    public class IUnitRepositoryImpl : IUnitRepository
    {
        #region Variable
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        #endregion

        #region Contructor
        public IUnitRepositoryImpl(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        #endregion


        Unit IUnitRepository.LoadUnitByID(Guid ID)
        {
            
            Unit unit = (from c in _context.Unit
                         where c.Id == ID
                         select c).FirstOrDefault();

            if (unit == null)
            {
                return null;
            }
            return (Unit)unit;
        }

        List<Unit> IUnitRepository.ListByParentID(Guid ParentID, int pageNumber, int pageSize)
        {

            var ListUnitByParentId = from c in _context.Unit
                                     where c.ParentId == ParentID
                                     select c;
            List<Unit> returnList = ListUnitByParentId.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            if (ListUnitByParentId == null)
            {
                return null;
            }

            return returnList;
        }

        #region CRUD TABLE Unit
        public Response DeleteUnit(Guid Id)
        {
            Response response = new Response();
            try
            {
                Unit unit = _context.Unit.Where(x => x.Id == Id).FirstOrDefault();

                if (unit != null)
                {
                    _context.Unit.Remove(unit);
                    _context.SaveChanges();

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
                        Message = "Không tìm thấy đơn vị !"
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
        public Response InsertUnit(UnitDto unitDto)
        {
            Response response = new Response();
            try
            {
                Unit unit = new Unit();
                unit = _mapper.Map<Unit>(unitDto);

                _context.Unit.Add(unit);
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
        public Response UpdateUnit(UnitDto unitDto)
        {
            Response response = new Response();
            try
            {
                Unit unit = new Unit();
                unit = _context.Unit.Where(e => e.Id == unitDto.Id).FirstOrDefault();

                if (unit != null)
                {
                    // define some col with data concrete
                    unit.UnitName = String.IsNullOrEmpty(unitDto.UnitName) ? unit.UnitName : unitDto.UnitName;
                    unit.UnitCode = String.IsNullOrEmpty(unitDto.UnitCode) ? unit.UnitCode : unitDto.UnitCode;
                    unit.ParentId = unitDto.ParentId.HasValue ? unitDto.ParentId : unit.ParentId;
                    unit.Status = unitDto.Status.HasValue ? unitDto.Status : unit.Status;
                    unit.CreatedBy = unitDto.CreatedBy.HasValue ? unitDto.CreatedBy : unit.CreatedBy;
                    unit.CreatedDate = unitDto.CreatedDate.HasValue ? unitDto.CreatedDate : unit.CreatedDate;

                    _context.Unit.Update(unit);
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
        #endregion
    }
}
