using System;

namespace DigitalLibary.WebApi.Payload
{
    public class IndividualSampleModel
    {
        public IndividualSampleModel()
        {

        }
        public Guid Id { get; set; }
        public Guid IdDocument { get; set; }
        public string NumIndividual { get; set; }
        public string Barcode { get; set; }
        public Guid StockId { get; set; }
        public bool IsLostedPhysicalVersion { get; set; }
        public bool IsDeleted { get; set; }
        public int Status { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? DocumentTypeId { get; set; }

        //extention
        public string SignIndividual { get; set; }
        public Guid IdCategory { get; set; }
    }
}
