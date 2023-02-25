using System;
using System.Collections.Generic;

namespace DigitalLibary.Service.Dto
{
    public class IndividualByDocumentDto
    {
        public IndividualByDocumentDto()
        {

        }
        public List<String>? createdDate { get; set; }
        public List<Boolean>? isLostedPhysicalVersion { get; set; }
        public List<String>? numIndividual { get; set; }
        public string? sortOrder { get; set; }
        public string? sortField { get; set; }
        public List<Guid>? stockId { get; set; }
        public Guid id { get; set; }
        public int page { get; set; }
        public int results { get; set; }
        public List<String>? barcode { get; set; }
    }

}
