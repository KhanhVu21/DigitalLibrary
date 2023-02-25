using System;

namespace DigitalLibary.Service.Common.FormatApi
{
    public class CustomApiListBorrowLateByUserType
    {
        public CustomApiListBorrowLateByUserType()
        {

        }
        public Guid IdUser { get; set; }
        public string NameUser { get; set; }
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }
        public Guid IdUnit { get; set; }
        public string NameUnit { get; set; }
        public Guid IdIndividual { get; set; }
        public string NumIndividual { get; set; }
        public Guid IdDocument { get; set; }
        public string NameDocument { get; set; }
        public string Author { get; set; }
        public string InvoiceCode { get; set; }
        public int NumberDayLate { get; set; }
    }
}
