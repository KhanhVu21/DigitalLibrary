using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLibary.Data.Entity
{
    [Table("IndividualSample")]
    public class IndividualSample
    {
        public IndividualSample()
        {

        }
        public Guid Id { get; set; }
        public Guid IdDocument { get; set; }
        public string NumIndividual { get; set; }
        public string Barcode { get; set; }
        public Guid StockId { get; set; }
        public bool IsLostedPhysicalVersion { get; set; }
        public bool IsDeleted { get; set; }
        public int Status { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? DocumentTypeId { get; set; }
    }
}
