using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalLibary.Data.Entity
{
    [Table("ReceiptDetail")]
    public class ReceiptDetail
    {
        public ReceiptDetail()
        {

        }
        [Key]
        public Guid IdReceiptDetail { get; set; }
        public Guid IdDocument { get; set; }
        public string? DocumentName { get; set; }
        public int? Quantity { get; set; }
        public double? Price { get; set; }
        public double? Total { get; set; }
        public Guid IdReceipt { get; set; }
        public Guid IdPublisher { get; set; }
        public string? NamePublisher { get; set; }
        public string? Note { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
