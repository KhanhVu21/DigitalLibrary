using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalLibary.Data.Entity
{
    [Table("Receipt")]
    public class Receipt
    {
        public Receipt()
        {

        }
        [Key]
        public Guid IdReceipt { get; set; }
        public Guid ReceiverIdUser { get; set; }
        public string? ReceiverName { get; set; }
        public string? ReceiverPosition { get; set; }
        public string? ReceiverUnitRepresent { get; set; }
        public string? DeliverName { get; set; }
        public string? DeliverPosition { get; set; }
        public string? DeliverUnitRepresent { get; set; }
        public string? ReceiptCode { get; set; }
        public string? Original { get; set; }
        public string? Reason { get; set; }
        public string? BookStatus { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
