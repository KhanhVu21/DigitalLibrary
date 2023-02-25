using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalLibary.Data.Entity
{
    [Table("User_Role")]
    public class User_Role
    {
        public User_Role()
        {

        }
        public Guid Id { get; set; }
        public string IdRole { get; set; }
        public string IdUser { get; set; }
    }
}
