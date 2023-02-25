using System;

namespace DigitalLibary.Data.Entity
{
    public class AuditMethod
    {
        public Guid Id { get; set; }
        public string? NameMethod { get; set; }
        public string? CodeMethod { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
