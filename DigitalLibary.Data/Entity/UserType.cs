using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalLibary.Data.Entity
{
    [Table("UserType")]
    public class UserType
    {
        public UserType()
        {

        }
        public Guid Id { get; set; }
        public string TypeName { get; set; }
        public int? Status { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
