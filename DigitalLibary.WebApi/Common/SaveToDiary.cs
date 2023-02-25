using DigitalLibary.Data.Data;
using DigitalLibary.Service.Dto;
using DigitalLibary.Service.Repository.IRepository;
using DigitalLibary.WebApi.Helper;
using Microsoft.Extensions.Options;
using System;

namespace DigitalLibary.WebApi.Common
{
    public class SaveToDiary
    {
        #region Variables
        private readonly AppSettingModel _appSettingModel;
        public DataContext _DbContext;
        private readonly IUserRepository _userRepository;
        private readonly IDiaryRepository _diaryRepository;
        #endregion

        #region Contructor
        public SaveToDiary(IOptionsMonitor<AppSettingModel> optionsMonitor,
        IUserRepository userRepository,
        IDiaryRepository diaryRepository,
        DataContext dataContext)
        {
            _appSettingModel = optionsMonitor.CurrentValue;
            _DbContext = dataContext;
            _userRepository = userRepository;
            _diaryRepository = diaryRepository;
        }
        #endregion

        #region METHOD
        public void SaveDiary(Guid UserId, String Operation, String Table, Boolean IsSuccess, Guid WithId)
        {
            try
            {
                DiaryDto diaryDto = new DiaryDto();
                UserDTO user1DTO = _userRepository.getUserByID(UserId);
                if (user1DTO != null)
                {
                    if(Operation == "Create")
                    {
                        diaryDto = new DiaryDto()
                        {
                            Content = $"{user1DTO.Fullname} đã thêm mới {Table}",
                            UserId = UserId,
                            DateCreate = DateTime.Now,
                            Title = "Thêm mới vào CSDL",
                            Operation = Operation,
                            Table = Table,
                            IsSuccess = IsSuccess,
                            UserName = user1DTO.Fullname,
                            WithId = WithId
                        };
                    }
                    if (Operation == "Update")
                    {
                        diaryDto = new DiaryDto()
                        {
                            Content = $"{user1DTO.Fullname} đã cập nhật {Table}",
                            UserId = UserId,
                            DateCreate = DateTime.Now,
                            Title = "Cập nhật vào CSDL",
                            Operation = Operation,
                            Table = Table,
                            IsSuccess = IsSuccess,
                            UserName = user1DTO.Fullname,
                            WithId = WithId
                        };
                    }
                    if (Operation == "Delete")
                    {
                        diaryDto = new DiaryDto()
                        {
                            Content = $"{user1DTO.Fullname} đã xóa thông tin từ {Table}",
                            UserId = UserId,
                            DateCreate = DateTime.Now,
                            Title = "Xóa thông tin CSDL",
                            Operation = Operation,
                            Table = Table,
                            IsSuccess = IsSuccess,
                            UserName = user1DTO.Fullname,
                            WithId = WithId
                        };
                    }

                    _diaryRepository.InsertDiary(diaryDto);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void ModifyDiary(Guid UserId, String Operation, String Table, Boolean IsSuccess, Guid WithId, String Content)
        {
            try
            {
                DiaryDto diaryDto = new DiaryDto();
                UserDTO user1DTO = _userRepository.getUserByID(UserId);
                if (user1DTO != null)
                {
                    if (Operation == "Update")
                    {
                        diaryDto = new DiaryDto()
                        {
                            Content = $"{user1DTO.Fullname} đã cập nhật {Table} - {Content}",
                            UserId = UserId,
                            DateCreate = DateTime.Now,
                            Title = "Cập nhật vào CSDL",
                            Operation = Operation,
                            Table = Table,
                            IsSuccess = IsSuccess,
                            UserName = user1DTO.Fullname,
                            WithId = WithId
                        };
                    }

                    _diaryRepository.InsertDiary(diaryDto);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
