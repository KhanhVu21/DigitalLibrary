using System;
using System.Collections.Generic;

namespace DigitalLibary.Service.Common.FormatApi
{
    public class DataOfOneIdAuditReceipt
    {
        public Guid IdAuditReceipt { get; set; }
        public DateTime? ReportCreateDate { get; set; }
        public DateTime? ReportToDate { get; set; }
        public string? Note { get; set; }
        public Guid? IdAuditMethod { get; set; }
        public List<DataDocumentAndAuditBookListByIdAuditReceipt> datas { get; set; }
        public List<AuditorListByIdAuditReceipt> dataAuditor { get; set; }
    }
    public record DataDocumentAndAuditBookListByIdAuditReceipt
    {
        public string? BookName { get; set; }
        public Guid? IdBook { get; set; }
        public string? NumIndividual { get; set; }
        public Guid? IdIndividual { get; set; }
        public string? TypeBook { get; set; }
        public Guid? IdTypeBook { get; set; }
        public long? PriceBook { get; set; }
        public string? Author { get; set; }
        public bool? WasLost { get; set; }
        public bool? Redundant { get; set; }
        public bool? IsLiquidation { get; set; }
        public Guid? IdStatusBook { get; set; }
        public string? NameStatusBook { get; set; }
        public string? Note { get; set; }
    }
    public record AuditorListByIdAuditReceipt
    {
        public Guid? IdUser { get; set; }
        public string? UserName { get; set; }
        public Guid? UnitId { get; set; }
        public string? UnitName { get; set; }
        public Guid? UserTypeId { get; set; }
        public string? TypeName { get; set; }
        public string? DescriptionRole { get; set; }

    }
}
