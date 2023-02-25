using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalLibary.Data.Entity
{
    [Table("BookNameEncrypt")]
    public class BookNameEncrypt
    {
        public BookNameEncrypt()
        {

        }
        public Guid Id { get; set; }
        public string? SignCode { get; set; }
        public string? SignNum { get; set; }
        public Boolean? IsHide { get; set; }
        public Int32? Status { get; set; }
        public DateTime? CreatedDate { get; set; }

    }
}
