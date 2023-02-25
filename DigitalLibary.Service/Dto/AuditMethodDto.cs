using System;

namespace DigitalLibary.Service.Dto
{
    public class AuditMethodDto
    {
        public Guid Id { get; set; }
        public string? NameMethod { get; set; }
        public string? CodeMethod { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
