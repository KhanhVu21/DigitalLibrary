using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLibary.Service.Dto
{
    public class DocumentTypeDto
    {
        public DocumentTypeDto()
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
