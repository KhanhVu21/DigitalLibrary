using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLibary.Service.Dto
{
    public class UnitDto
    {
        public UnitDto()
        {

        }
        public Guid Id { get; set; }
        public string UnitName { get; set; }
        public Guid? ParentId { get; set; }
        public int? Status { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UnitCode { get; set; }
    }
}
