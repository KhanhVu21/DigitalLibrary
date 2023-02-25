using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalLibary.Data.Entity
{
    [Table("User")]
    public class User
    {
        public User()
        {

        }
        public Guid Id { get; set; }
        public string? Fullname { get; set; }
        public string? Description { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public Guid UserTypeId { get; set; }
        public string? Address { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public string? UserCode { get; set; }
        public bool IsLocked { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public Guid UnitId { get; set; }
        public string ActiveCode { get; set; }
        public DateTime? AcitveUser { get; set; }
        public DateTime? ExpireDayUser { get; set; }

        public string? Avatar { get; set; }
    }
}
