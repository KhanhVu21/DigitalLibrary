using System;

namespace DigitalLibary.Data.Entity
{
    public class Diary
    {
        public Guid Id { get; set; }
        public String? Content { get; set; }
        public Guid UserId { get; set; }
        public String? UserName { get; set; }
        public DateTime DateCreate { get; set; }
        public String? Title { get; set; }
        public String? Operation { get; set; }
        public String? Table { get; set; }
        public Boolean IsSuccess { get; set; }
        public Guid WithId { get; set; }
    }
}
