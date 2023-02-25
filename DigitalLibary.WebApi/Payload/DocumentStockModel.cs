using System;

namespace DigitalLibary.WebApi.Payload
{
    public class DocumentStockModel
    {
        public DocumentStockModel()
        {

        }
        public Guid Id { get; set; }
        public string StockName { get; set; }
        public string? StockCode { get; set; }
        public Guid? StockParentId { get; set; }
        public string? Description { get; set; }
        public int? Status { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
