using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLibary.Data.Entity
{
    public class CategorySign
    {
        public CategorySign()
        {

        }
        public Guid Id { get; set; }
        public string? SignName { get; set; }
        public string? SignCode { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? IdCategory { get; set; }
        public bool IsHided { get; set; }
        public bool IsDeleted { get; set; }
    }
}
