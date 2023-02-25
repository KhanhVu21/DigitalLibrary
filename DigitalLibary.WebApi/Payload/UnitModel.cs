using System;

namespace DigitalLibary.WebApi.Payload
{
    public class UnitModel
    {
        public UnitModel()
        {

        }
        public Guid Id { get; set; }
        public string UnitName { get; set; }
        public Guid? ParentId { get; set; }
        public int? Status { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UnitCode { get; set; }
    }
}
