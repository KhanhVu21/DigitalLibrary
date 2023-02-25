using System;
using System.Collections.Generic;

namespace DigitalLibary.WebApi.Payload
{
    public class AuditBookListModel
    {
        public Guid? Id { get; set; }
        public Guid? IdDocument { get; set; }
        public bool? WasLost { get; set; }
        public bool? Redundant { get; set; }
        public Guid? IdStatusBook { get; set; }
        public bool? IsLiquidation { get; set; }
        public string? Note { get; set; }

    }
}
