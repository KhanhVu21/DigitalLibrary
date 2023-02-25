using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Common;
using DigitalLibary.Service.Dto;
using System;
using System.Collections.Generic;

namespace DigitalLibary.Service.Repository.IRepository
{
    public interface ICategoryPublisherRepository
    {
        public List<CategoryPublisher> SearchByName(String SearchString, int pageNumber, int pageSize);
        Response InsertCategoryPublisher(CategoryPublisherDto categoryPublisherDto);
        Boolean CheckPublisherCode(string PublishCode);
        Boolean CheckPublisherCodeUpdate(CategoryPublisher categoryPublisher);
        public CategoryPublisher GetPublishById (Guid id);
        Response UpdateCategoryPublisher(CategoryPublisher categoryPublisher);
        List<Tuple<string, string>> GetAllPublisher();
    }

}
