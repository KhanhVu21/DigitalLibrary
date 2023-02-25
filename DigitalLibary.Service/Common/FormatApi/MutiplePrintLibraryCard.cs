using System;

namespace DigitalLibary.Service.Common.FormatApi
{
    public class MutiplePrintLibraryCard
    {
        public Guid? Id { get; set; }
        public string? UnitName { get; set; } = string.Empty;
        public string? FullName { get; set; } = string.Empty;
        public string? UserCode { get; set; } = string.Empty;
        public string? Avatar { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
    }
}
