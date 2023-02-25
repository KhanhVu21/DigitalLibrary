using DigitalLibary.Service.Common;
using DigitalLibary.Service.Common.FormatApi;
using DigitalLibary.Service.Dto;
using System;
using System.Collections.Generic;

namespace DigitalLibary.Service.Repository.IRepository
{
    public interface IAuditReceiptRepository
    {
        IEnumerable<AuditReceiptDto> GetAllAuditReceipt(int pageNumber, int pageSize, string ReportCreateDate, string ReportToDate);
        DataOfOneIdAuditReceipt GetAuditReceiptById(Guid IdAuditReceipt);
        Response InsertAuditReceipt(AuditReceiptDto auditReceiptDto);
        Response UpdateAuditReceipt(Guid IdAuditReceipt, AuditReceiptDto auditReceiptDto);
        Response DeleteAuditReceipt(Guid IdAuditReceipt);
        Response LiquidationAuditReceiptByListId(List<Guid> IdAuditReceipt);
        Response DeleteAuditReceiptByList(List<Guid> IdAuditReceipt);
        CustomApiAuditReceipt GetDataBookByBarcode(string barcode);
        Tuple<string, string> GetUnitAndTypeOfUser(Guid IdUser);
        List<CustomApiAuditReceipt> ConfirmLostBook(int pageNumber, int pageSize, List<Guid> IdIndividual);
        ReportAuditReceipt ReportAuditReceipt(Guid IdAuditReceipt);
        AuditTraditionalDocument PrintListDataDocument(Guid IdDocumentType, int sortByCondition);
        Int64 CountAllNumberOfBook();
        List<CustomApiAuditReceipt> GetListBookToAuditReceipt(string filter, Guid IdDocumentType, int pageNumber, int pageSize);
    }
}
