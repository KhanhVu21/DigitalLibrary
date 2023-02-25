using System;

namespace DigitalLibary.Service.Common.FormatApi
{
    public class CustomApiListBorrowByUserType
    {
        public CustomApiListBorrowByUserType()
        {

        }
        public Guid IdUser { get; set; }
        public string NameUser { get; set; }
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }
        public string UserCode { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public Guid IdUnit { get; set; }
        public Guid IdUserType { get; set; }
    }
}
