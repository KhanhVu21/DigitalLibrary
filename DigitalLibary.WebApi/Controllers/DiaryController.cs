using DigitalLibary.Service.Dto;
using DigitalLibary.Service.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;

namespace DigitalLibary.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiaryController: Controller
    {
        #region Variables
        private readonly IDiaryRepository _diaryRepository;
        #endregion

        #region Contructor
        public DiaryController(IDiaryRepository diaryRepository)
        {
            _diaryRepository = diaryRepository;
        }
        #endregion

        [HttpGet("GetAllDiary")]
        public IEnumerable<DiaryDto> GetAllDiary(string Table, int pageNumber, int pageSize)
        {
            IEnumerable<DiaryDto> result = _diaryRepository.GetAllDiary(Table, pageNumber, pageSize);
            return result;
        }

    }
}
