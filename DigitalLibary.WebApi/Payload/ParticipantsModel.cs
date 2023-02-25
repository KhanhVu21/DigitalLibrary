using System;
using System.ComponentModel.DataAnnotations;

namespace DigitalLibary.WebApi.Payload
{
    public class ParticipantsModel
    {
        public Guid? Id { get; set; }
        [Required]
        public Guid IdReceipt { get; set; }
        public String? Name { get; set; }
        public String? Position { get; set; }
        public String? Mission { get; set; }
        public String? Note { get; set; }
    }
}
