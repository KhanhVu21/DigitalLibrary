using System;
using System.Collections.Generic;

namespace DigitalLibary.Service.Dto
{
    public class SortAndSearchListUsser
    {
        public SortAndSearchListUsser()
        {

        }
        public string? sortOrder { get; set; }
        public string? sortField { get; set; }
        public int page { get; set; }
        public int results { get; set; }
        public List<String>? Fullname { get; set; }
        public List<String>? Email { get; set; }
        public List<String>? Phone { get; set; }
        public List<String>? Address { get; set; }
        public List<String>? ActiveCode { get; set; }
        public List<Guid>? userTypeId { get; set; }
        public List<Guid>? unitId { get; set; }
    }
}
