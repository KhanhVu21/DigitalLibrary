using System;

namespace DigitalLibary.Data.Entity
{
    public class AuditorList
    {
        public Guid Id { get; set; }
        public Guid? IdUser { get; set; }
        public Guid? IdAuditReceipt { get; set; }   
        public string? DescriptionRole { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
