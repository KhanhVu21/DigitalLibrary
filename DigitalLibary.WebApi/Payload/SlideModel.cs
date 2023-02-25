using System;

namespace DigitalLibary.WebApi.Payload
{
    public class SlideModel
    {
        public SlideModel()
        {

        }
        public Guid Id { get; set; }
        public string? FileName { get; set; }
        public string? FileNameExtention { get; set; }
        public string? FilePath { get; set; }
        public int? Status { get; set; }
        public bool? IsDelete { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Description { get; set; }
        public bool? IsHide { get; set; }
        public string? Title { get; set; }
    }
}
