using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalLibary.Data.Entity
{
    [Table("Unit")]
    public class Unit
    {
        public Unit()
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
