using DigitalLibary.Service.Dto;
using System;
using System.Collections.Generic;

namespace DigitalLibary.WebApi.Payload
{
    public class AuditReceiptModel
    {
        public Guid? Id { get; set; }
        public DateTime? ReportCreateDate { get; set; }
        public DateTime? ReportToDate { get; set; }
        public string? Note { get; set; }
        public Guid? IdAuditMethod { get; set; }

        public List<AuditorPayload> AuditorModels { get; set; }
        public List<AuditBookListPayload> AuditBookListPayloads { get; set; }

    }
}
