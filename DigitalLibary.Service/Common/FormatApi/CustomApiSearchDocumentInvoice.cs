using DigitalLibary.Data.Entity;
using System.Collections.Generic;

namespace DigitalLibary.Service.Common.FormatApi
{
    public class CustomApiSearchDocumentInvoice
    {
        public CustomApiSearchDocumentInvoice()
        {

        }
        public List<DocumentInvoice> documentInvoices { get; set; }
        public string NameUser { get; set; }
        public string IdUser { get; set; }
        public string Email { get; set; }
    }
}
