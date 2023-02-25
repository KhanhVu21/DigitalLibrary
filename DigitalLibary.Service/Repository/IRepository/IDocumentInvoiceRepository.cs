using DigitalLibary.Service.Common;
using DigitalLibary.Service.Common.FormatApi;
using DigitalLibary.Service.Dto;
using System;
using System.Collections.Generic;

namespace DigitalLibary.Service.Repository.IRepository
{
    public interface IDocumentInvoiceRepository
    {
        #region CRUD TABLE DOCUMENTINVOICE
        List<CustomApiSearchDocumentInvoice> ListDocumentInvoice(string name);
        List<DocumentInvoiceDto> getListBorrowLate(string fromDate, string toDate);
        List<DocumentInvoiceDto> GetDocumentInvoice(int pageNumber, int pageSize);
        DocumentInvoiceDto GetDocumentInvoiceById(Guid Id);
        List<DocumentInvoiceDetailDto> GetListDocumentInvoiceById(Guid Id);
        Response InsertDocumentInvoice(DocumentInvoiceDto documentInvoiceDto);
        Response UpdateDocumentInvoice(DocumentInvoiceDto documentInvoiceDto);
        Response ChangeStatusDocumentInvoice(Guid Id, int status);
        List<DocumentInvoiceDto> GetDocumentInvoiceByStatus(int status);
        List<DocumentInvoiceDetailDto> GetDocumentInvoiceDetailByIdDocumentIncoice(Guid Id);
        Response ChangeStatusCompleteInvoice(Guid Id);
        #endregion
    }
}
