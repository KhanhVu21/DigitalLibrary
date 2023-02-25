using System;

namespace DigitalLibary.Service.Dto
{
    public class ParticipantsDto
    {
        public Guid Id { get; set; }
        public Guid IdReceipt { get; set; }
        public String? Name { get; set; }
        public String? Position { get; set; }
        public String? Mission { get; set; }
        public String? Note { get; set; }
        public DateTime CreatedDate { get; set; }
        public Int32 Status { get; set; }
    }
}
