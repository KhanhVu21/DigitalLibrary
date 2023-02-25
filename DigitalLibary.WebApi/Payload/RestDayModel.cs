using System;

namespace DigitalLibary.WebApi.Payload
{
    public class RestDayModel
    {
        public RestDayModel()
        {

        }
        public Guid Id { get; set; }
        public string? NameRestDate { get; set; }
        public bool IsActived { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? Status { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Note { get; set; }
    }
}
