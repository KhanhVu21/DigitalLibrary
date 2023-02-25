using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalLibary.Data.Entity
{
    [Table("ContactAndIntroduction")]
    public class ContactAndIntroduction
    {
        public ContactAndIntroduction()
        {

        }
        public Guid Id { get; set; }
            public string? col { get; set; }
            public string? col1 { get; set; }
            public string? col2 { get; set; }
            public string? col3 { get; set; }
            public string? col4 { get; set; }
            public string? col5 { get; set; }
            public string? col6 { get; set; }
            public string? col7 { get; set; }
            public string? col8 { get; set; }
            public string? col9 { get; set; }
            public string? col10 { get; set; }
            public bool? IsActived { get; set; }
            public bool? IsDeleted { get; set; }
            public int? Type { get; set; }
            public int? Status { get; set; }
            public Guid? Createby { get; set; }
            public DateTime? CreateDate { get; set; }
    }
}
