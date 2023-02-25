using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLibary.Service.Dto
{
    public class DocumentInvoiceDetailDto
    {
        public DocumentInvoiceDetailDto()
        {

        }
        public Guid Id { get; set; }
        public Guid? IdDocument { get; set; }
        public int? Status { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid IdDocumentInvoice { get; set; }
        public Guid IdIndividual { get; set; }
    }
}
