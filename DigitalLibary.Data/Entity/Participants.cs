using System;

namespace DigitalLibary.Data.Entity
{
    public class Participants
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
