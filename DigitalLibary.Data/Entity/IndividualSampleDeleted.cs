using System;

namespace DigitalLibary.Data.Entity
{
    public class IndividualSampleDeleted
    {
        public IndividualSampleDeleted()
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
    }
}
