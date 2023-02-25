using DigitalLibary.Service.Common;
using DigitalLibary.Service.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalLibary.Service.Repository.IRepository
{
    public interface IReceiptRepository
    {
        #region CRUD TABLE RECEIPT
        List<ReceiptDto> getAllReceipt(int pageNumber, int pageSize);
        List<ReceiptDto> getAllReceipt(SortReceiptAndSearch sortReceiptAndSearch);
        ReceiptDto getReceipt(Guid Id);
        ReceiptDto SearchReceipt(string code);
        Response InsertReceipt(ReceiptDto receiptDto);
        Task<Response> UpdateReceipt(ReceiptDto receiptDto);
        Response DeleteReceipt(Guid Id);
        DocumentDto GetDocumentType(Guid Id);
        public int GetMaxReceiptCode(string Code);
        public List<string> GetlistOriginal();
        public List<string> GetlistBookStatus();
        #endregion
    }
}
