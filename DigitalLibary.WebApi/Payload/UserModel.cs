using DigitalLibary.Data.Entity;
using System;
using System.Collections.Generic;

namespace DigitalLibary.WebApi.Payload
{
    public class UserModel
    {
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
        public string? Avatar { get; set; }
        public string? AcitveUser { get; set; }
        public string? ExpireDayUser { get; set; }

        // add col role name
        public List<Role> ListRole { get; set; }
        public string IdRole { get; set; }
        public int total { get; set; }
        public Guid? idFile { get; set; }
    }
}
