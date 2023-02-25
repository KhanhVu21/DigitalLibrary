using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLibary.Service.Dto
{
    public class DocumentAvatarDto
    {
        public DocumentAvatarDto()
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
