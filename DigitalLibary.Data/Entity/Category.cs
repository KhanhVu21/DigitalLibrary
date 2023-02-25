using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace DigitalLibary.Data.Entity
{
    [Table("Category")]
    public class Category
    {
        public Category()
        {

        }
        public Guid Id { get; set; }
        public string? CategoryName { get; set; }
        public Guid? ParentId { get; set; }
        public string? Note { get; set; }
        public int? Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public Boolean IsDeleted { get; set; }
        public Boolean IsHide { get; set; }
    }
}
