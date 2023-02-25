using System;

namespace DigitalLibary.Service.Common.FormatApi
{
    public class CustomApiNumIndividualLedger
    {
        public CustomApiNumIndividualLedger()
        {

        }
        public Guid IdIndividual { get; set; }
        public string NameIndividual { get; set; }
        public string DocumentName { get; set; }
        public string Author { get; set; }
        public DateTime? DateIn { get; set; }
    }
}
