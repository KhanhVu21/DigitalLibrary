using System;
namespace DigitalLibary.Service.Dto
{
    public class CategorySign_V1Dto
    {
        public CategorySign_V1Dto()
        {

        }
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public string? SignName { get; set; }
        public string? SignCode { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? IdCategory { get; set; }
        public bool IsHided { get; set; }
        public bool IsDeleted { get; set; }
    }
}
