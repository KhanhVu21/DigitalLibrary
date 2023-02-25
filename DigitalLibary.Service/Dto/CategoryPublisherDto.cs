using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLibary.Service.Dto
{
    public class CategoryPublisherDto
    {
        public CategoryPublisherDto()
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
