using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLibary.Service.Dto
{
    public class SortAndSearchListDocument
    {
        public SortAndSearchListDocument()
        {

        }
        public int DocumentType { get; set; }
        public string? sortOrder { get; set; }
        public string? sortField { get; set; }
        public int page { get; set; }
        public int results { get; set; }
        public List<String>? DocName { get; set; }
        public List<String>? Language { get; set; }
        public List<String>? Publisher { get; set; }
        public List<String>? PublishYear { get; set; }
        public List<String>? Author { get; set; }
        public List<String>? Description { get; set; }
        public List<long>? Price { get; set; }
        public List<Boolean>? IsHavePhysicalVersion { get; set; }
        public List<Boolean>? isApproved { get; set; }
        public List<Guid>? nameCategory { get; set; }
        public List<Guid>? idCategorySign_V1 { get; set; }
        public List<Int32>? Sort { get; set; }
    }
}
