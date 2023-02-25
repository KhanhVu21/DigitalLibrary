using DigitalLibary.Service.Common;
using DigitalLibary.Service.Dto;
using System;
using System.Collections.Generic;

namespace DigitalLibary.Service.Repository.IRepository
{
    public interface IDocumentStockRepository
    {
        #region CRUD DOCUMENTSTOCK
        List<DocumentStockDto> GetAllDocumentStocks();
        List<DocumentStockDto> GetAllDocumentStocks(int pageNumber, int pageSize);
        List<DocumentStockDto> GetAllDocumentStocksByParentId(Guid Id);
        DocumentStockDto GetAllDocumentStocksById(Guid Id);
        Response InsertDocumentStock(DocumentStockDto documentStockDto);
        Response UpdateDocumentStock(DocumentStockDto documentStockDto);
        Response DeleteDocumentStock(Guid Id);
        Boolean CheckBookInStock(Guid IdStock);
        List<Tuple<Guid, string>> GetIdAndNameDocumentStock();
        #endregion
    }
}
