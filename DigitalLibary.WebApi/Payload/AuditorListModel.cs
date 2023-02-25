using System;

namespace DigitalLibary.WebApi.Payload
{
    public class AuditorListModel
    {
        public Guid? Id { get; set; }
        public Guid? IdUser { get; set; }
        public Guid? IdAuditReceipt { get; set; }
        public string? DescriptionRole { get; set; }
    }
}
