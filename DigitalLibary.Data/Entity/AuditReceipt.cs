using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLibary.Data.Entity
{
    public class AuditReceipt
    {
        public Guid Id { get; set; }
        public string? AuditNumber { get; set; }
        public DateTime? ReportCreateDate { get; set; }
        public DateTime? ReportToDate { get; set; }
        public string? Note { get; set; }
        public Guid? IdAuditMethod { get; set; }
        public int? Status { get; set; }
        public int? TotalBook { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
