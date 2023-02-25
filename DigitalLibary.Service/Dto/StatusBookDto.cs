using System;

namespace DigitalLibary.Service.Dto
{
    public class StatusBookDto
    {
        public Guid Id { get; set; }
        public string? NameStatusBook { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
