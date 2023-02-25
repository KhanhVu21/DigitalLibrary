using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalLibary.Data.Entity
{
    [Table("DocumentType")]
    public class DocumentType
    {
        public DocumentType()
        {

        }
        public Guid Id { get; set; }
        public string DocTypeName { get; set; }
        public string? Description { get; set; }
        public Guid? ParentId { get; set; }
        public long? Status { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
