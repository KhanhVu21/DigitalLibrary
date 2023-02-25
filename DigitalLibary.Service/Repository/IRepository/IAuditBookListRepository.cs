using DigitalLibary.Service.Common;
using DigitalLibary.Service.Dto;
using System;
using System.Collections.Generic;

namespace DigitalLibary.Service.Repository.IRepository
{
    public interface IAuditBookListRepository
    {
        IEnumerable<AuditBookListDto> GetAllAuditBookList(int pageNumber, int pageSize);
        AuditBookListDto GetAllAuditBookListById(Guid IdAuditBookList);
        Response InsertAuditBookList(AuditBookListDto AuditBookListDto);
        Response UpdateAuditBookList(Guid IdAuditBookList, AuditBookListDto AuditBookListDto);
        Response DeleteAuditBookList(Guid IdAuditBookList);
        Response DeleteAuditBookListByList(List<Guid> IdAuditBookList);
    }
}
