using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLibary.Data.Entity
{
    public class SchoolYear
    {
        public SchoolYear()
        {

        }
        public Guid Id { get; set; }
        public DateTime? FromYear { get; set; }
        public DateTime? ToYear { get; set; }
        public DateTime? StartSemesterI { get; set; }
        public DateTime? StartSemesterII { get; set; }
        public DateTime? EndAllSemester { get; set; }
        public bool? IsActived { get; set; }
        public int? Status { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
