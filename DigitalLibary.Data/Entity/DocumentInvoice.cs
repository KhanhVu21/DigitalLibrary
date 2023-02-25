using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLibary.Data.Entity
{
    public class DocumentInvoice
    {
        public DocumentInvoice()
        {

        }
        public Guid Id { get; set; }
        public string InvoiceCode { get; set; }
        public Guid UserId { get; set; }
        public DateTime DateOut { get; set; }
        public DateTime DateIn { get; set; }
        public DateTime? DateInReality { get; set; }
        public int? Status { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? Note { get; set; }
        public bool? IsCompleted { get; set; }
    }
}
