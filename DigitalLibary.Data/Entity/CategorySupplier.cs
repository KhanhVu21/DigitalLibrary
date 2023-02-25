using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLibary.Data.Entity
{
    [Table("CategorySupplier")]
    public class CategorySupplier
    {
        public CategorySupplier()
        {

        }
        public Guid Id { get; set; }
        public string? SupplierName { get; set; }
        public string? SupplierCode { get; set; }
        public string? TaxCode { get; set; }
        public string? Address { get; set; }
        public string? Note { get; set; }
        public Guid? IdCategory { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Boolean IsHided { get; set; }
        public Boolean IsDeleted { get; set; }

    }
}
