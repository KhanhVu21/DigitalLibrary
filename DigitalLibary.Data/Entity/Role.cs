using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalLibary.Data.Entity
{
    [Table("Role")]
    public class Role
    {
        public Role()
        {

        }
        public Guid Id { get; set; }
        public string RoleName { get; set; }
        public int Status { get; set; }
        public bool IsDeleted { get; set; }
    }
}
