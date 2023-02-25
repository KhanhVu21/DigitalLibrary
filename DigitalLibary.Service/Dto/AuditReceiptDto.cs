using System;
using System.Collections.Generic;

namespace DigitalLibary.Service.Dto
{
    public class AuditReceiptDto
    {
        public Guid Id { get; set; }
        public string? AuditNumber { get; set; }
        public DateTime? ReportCreateDate { get; set; }
        public DateTime? ReportToDate { get; set; }
        public string? Note { get; set; }
        public Guid? IdAuditMethod { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }

        public List<AuditorPayload> AuditorModels { get; set; }
        public List<AuditBookListPayload> AuditBookListPayloads { get; set; }
    }
    public class AuditorPayload
    {
        public Guid? IdUser { get; set; }
        public string? DescriptionRole { get; set; }
    }
    public class AuditBookListPayload
    {
        public Guid? IdDocument { get; set; }
        public bool? WasLost { get; set; }
        public bool? Redundant { get; set; }
        public bool? IsLiquidation { get; set; }
        public Guid? IdStatusBook { get; set; }
        public string? Note { get; set; }
        public Guid? IdIndividualSample { get; set; }

    }
}
