using DigitalLibary.Data.Entity;
using System;
using System.Collections.Generic;

namespace DigitalLibary.Service.Common.FormatApi
{
    public class CustomApiAnalystBookByType
    {
        public Document document { get; set; }
        public int TotalDocument { get; set; }
        public int RemainDocument { get; set; }
        public int BorrowedDocument { get; set; }
        public int LostDocument { get; set; }
        public string? NameDocmentType { get; set; }
    }
    public class AnalystBookByGroupType
    {
        public Guid? IdDocumentType { get; set; }
        public string? NameDocmentType { get; set; }
        public List<CustomApiAnalystBookByType> DataAnalystBooks { get; set; }
    }
}
