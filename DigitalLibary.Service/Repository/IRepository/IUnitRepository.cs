using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Common;
using DigitalLibary.Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLibary.Service.Repository.IRepository
{
    public interface IUnitRepository
    {
        List<Unit> ListByParentID(Guid ParentID, int pageNumber, int pageSize);
        Unit LoadUnitByID(Guid ID);

        #region CRUD TABLE Unit
        Response InsertUnit(UnitDto unitDto);
        Response UpdateUnit(UnitDto unitDto);
        Response DeleteUnit(Guid Id);
        #endregion
    }
}
