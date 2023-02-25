using System;
using System.Collections.Generic;

namespace DigitalLibary.Service.Common.FormatApi
{
    public class CustomApiListBorrowByUserTypeDetail
    {
        public CustomApiListBorrowByUserTypeDetail()
        {
            
        }
        public Guid IdUser { get; set; }
        public string NameUser { get; set; }
        public string UserCode { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public Guid IdUnit { get; set; }
        public string NameUnit { get; set; }
        public Guid IdUserType { get; set; }
        public List<ListBorrowByUserId> listBorrowByUserIds { get; set; }
    }
    public class ListBorrowByUserId
    {
        public ListBorrowByUserId()
        {

        }
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }
        public DateTime? dateReality { get; set; }
        public Guid IdDocumnent { get; set; }
        public string NameDocument { get; set; }
        public Guid IdIndividual { get; set; }
        public string NumIndividual { get; set; }
        public string Note { get; set; }
    }
}
