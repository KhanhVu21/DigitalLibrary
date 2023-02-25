using System;

namespace DigitalLibary.Service.Dto
{
    public class BookNameEncryptDto
    {
        public Guid Id { get; set; }
        public string? SignCode { get; set; }
        public string? SignNum { get; set; }
        public Boolean IsHide { get; set; }
        public Int32 Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
