using DigitalLibary.Service.Common;
using DigitalLibary.Service.Dto;
using System;
using System.Collections.Generic;

namespace DigitalLibary.Service.Repository.IRepository
{
    public interface ISlideRepository
    {
        List<SlideDto> GetAllSlideAdmin(int pageNumber, int pageSize);
        List<SlideDto> GetAllSlideClient(int pageNumber, int pageSize);
        Response InsertSlide(SlideDto slideDto);
        Response UpdateSlide(SlideDto slideDto);
        Response DeleteSlide(Guid Id);
        Response HideSlide(Guid Id, bool check);
        SlideDto getSlideById(Guid Id);
    }
}
