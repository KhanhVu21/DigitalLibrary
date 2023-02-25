using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace DigitalLibary.Data.Entity
{

    [Table("CategoryPublisher")]
    public class CategoryPublisher
    {
        public CategoryPublisher()
        {

        }
        public Guid Id { get; set; }
        public Guid? IdCategory { get; set; }
        public string? PublisherCode { get; set; }
        public string? PublisherName { get; set; }
        public string? Address { get; set; }
        public string? Note { get; set; }
        public Boolean IsHided { get; set; }
        public Boolean IsDeleted { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }

    }
}
