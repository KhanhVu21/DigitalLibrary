using AutoMapper;
using DigitalLibary.Data.Data;
using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Common;
using DigitalLibary.Service.Dto;
using DigitalLibary.Service.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DigitalLibary.Service.Repository.RepositoryIPL
{
    public class DiaryRepository: IDiaryRepository
    {
        #region Variables
        private readonly IMapper _mapper;
        public DataContext _DbContext;
        #endregion

        #region Constructors
        public DiaryRepository(DataContext DbContext, IMapper mapper)
        {
            _DbContext = DbContext;
            _mapper = mapper;
        }

        #endregion

        #region METHOD
        public IEnumerable<DiaryDto> GetAllDiary(String table, int pageNumber, int pageSize)
        {
            try
            {
                IEnumerable<Diary> diary = _DbContext.Diary.Where(e => e.Table.ToLower() == table.ToLower()).OrderByDescending(e => e.DateCreate).ToList();

                if (pageNumber != 0 && pageSize != 0)
                {
                    if (pageNumber < 0) { pageNumber = 1; }
                    diary = diary.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }

                IEnumerable<DiaryDto> diaryDtos = _mapper.Map<IEnumerable<DiaryDto>>(diary);

                return diaryDtos;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Response InsertDiary(DiaryDto diaryDto)
        {
            Response response = new Response();
            try
            {
                Diary diary = new Diary();
                diary = _mapper.Map<Diary>(diaryDto);

                _DbContext.Diary.Add(diary);
                _DbContext.SaveChanges();

                response = new Response()
                {
                    Success = true,
                    Fail = false,
                    Message = "Thêm mới thành công !"
                };
                return response;
            }
            catch (Exception)
            {
                response = new Response()
                {
                    Success = false,
                    Fail = true,
                    Message = "Thêm mới không thành công !"
                };
                return response;
            }
        }
        #endregion
    }
}
