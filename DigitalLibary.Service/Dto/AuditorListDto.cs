using System;

namespace DigitalLibary.Service.Dto
{
    public class AuditorListDto
    {
        public Guid Id { get; set; }
        public Guid? IdUser { get; set; }
        public Guid? IdAuditReceipt { get; set; }   
        public string? DescriptionRole { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
