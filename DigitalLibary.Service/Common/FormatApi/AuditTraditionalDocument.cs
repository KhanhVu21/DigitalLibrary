using System;
using System.Collections.Generic;

namespace DigitalLibary.Service.Common.FormatApi
{
    public class AuditTraditionalDocument
    {
        public List<DataBookByDocumentType> dataBookByDocumentTypes { get; set; }
    }
    public record DataBookByDocumentType
    {
        public string? DocumentTypeName { get; set; }
        public List<DataBook> DataOfBook { get; set; }
    }
    public record DataBook
    {
        public string? DocName { get; set; }
        public string? NumIndividual { get; set; }
        public string? Author { get; set; }
        public bool? WasLost { get; set; }
        public bool? Redundant { get; set; }
        public bool? IsLiquidation { get; set; }
        public string? NameStatusBook { get; set; }
        public string? SignCode { get; set; }
        public string? EncryptDocumentName { get; set; }
        public string? Publisher { get; set; }
        public DateTime? PublishYear { get; set; }
        public Guid? Id { get; set; }
        public string? DocTypeName { get; set; }
    }
}
