using System;

namespace DigitalLibary.WebApi.Payload
{
    public class SchoolYearModel
    {
        public SchoolYearModel()
        {

        }
        public Guid Id { get; set; }
        public DateTime? FromYear { get; set; }
        public DateTime? ToYear { get; set; }
        public DateTime? StartSemesterI { get; set; }
        public DateTime? StartSemesterII { get; set; }
        public DateTime? EndAllSemester { get; set; }
        public bool? IsActived { get; set; }
        public int? Status { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
