using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalLibary.Data.Entity
{
    [Table("DocumentAvatar")]
    public class DocumentAvatar
    {
        public DocumentAvatar()
        {

        }
        public Guid Id { get; set; }
        public string? Path { get; set; }
        public int? Status { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? NameFileAvatar { get; set; }
        public Guid IdDocument { get; set; }
        public string? FileNameExtention { get; set; }
        public string SizeImage { get; set; }
    }
}
