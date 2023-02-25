using System;

namespace DigitalLibary.Data.Entity
{
    public class StatusBook
    {
        public Guid Id { get; set; }
        public string? NameStatusBook { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
