using DigitalLibary.Service.Common;
using DigitalLibary.Service.Dto;
using System.Collections;
using System.Collections.Generic;

namespace DigitalLibary.Service.Repository.IRepository
{
    public interface IDiaryRepository
    {
        Response InsertDiary(DiaryDto diaryDto);

        public IEnumerable<DiaryDto> GetAllDiary(string Table, int pageNumber, int pageSize);
    }
}
