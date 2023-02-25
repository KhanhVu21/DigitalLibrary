using System;

namespace DigitalLibary.WebApi.Payload
{
    public class UpdateUserExpireDateByUnitModel
    {
        public Guid idUnit { get; set; }
        public string effectiveDate { get; set; }
        public string expirationDate { get; set; }
    }
}
