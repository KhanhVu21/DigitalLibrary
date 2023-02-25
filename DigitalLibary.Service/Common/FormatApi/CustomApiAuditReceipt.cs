using System;

namespace DigitalLibary.Service.Common.FormatApi
{
    public class CustomApiAuditReceipt
    {
        public string? BookName { get; set; }
        public Guid? IdBook { get; set; }
        public string? NumIndividual { get; set; }
        public Guid? IdIndividual { get; set; }
        public string? TypeBook { get; set; }
        public Guid? IdTypeBook { get; set; }
        public long? PriceBook { get; set; }
        public string? Author { get; set; }
        public bool? IsLostedPhysicalVersion { get; set; }
    }
}
