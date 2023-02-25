using AutoMapper;
using DigitalLibary.Service.Common;
using DigitalLibary.Service.Dto;
using DigitalLibary.Service.Repository.IRepository;
using DigitalLibary.WebApi.Common;
using DigitalLibary.WebApi.Helper;
using DigitalLibary.WebApi.Payload;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalLibary.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestDayController : Controller
    {
        #region Variables
        private readonly IRestDateRepository _restDateRepository;
        private readonly AppSettingModel _appSettingModel;
        private readonly IMapper _mapper;
        private readonly JwtService _jwtService;
        private readonly SaveToDiary _saveToDiary;
        private readonly IUserRepository _userRepository;
        #endregion

        #region Contructor
        public RestDayController(IRestDateRepository restDateRepository,
        IOptionsMonitor<AppSettingModel> optionsMonitor,
        IMapper mapper,
        SaveToDiary saveToDiary,
        JwtService jwtService, IUserRepository userRepository)
        {
            _appSettingModel = optionsMonitor.CurrentValue;
            _mapper = mapper;
            _restDateRepository = restDateRepository;
            _jwtService = jwtService;
            _saveToDiary = saveToDiary;
            _userRepository = userRepository;   
        }
        #endregion

        #region MeThod
        // POST: api/RestDay/InsertRestDay
        [HttpPost]
        [Route("InsertRestDay")]
        public async Task<IActionResult> InsertRestDay(RestDayModel restDayModel)
        {
            Guid IdUserCurrent = Guid.NewGuid();
            Guid IdRestday = Guid.NewGuid();
            try
            {
                //check role admin
                Request.Headers.TryGetValue("Authorization", out var headerValue);
                if (headerValue.Count == 0)
                {
                    return BadRequest(new
                    {
                        message = "Bạn cần đăng nhập tài khoản Admin"
                    });
                }
                CheckRoleSystem checkRoleSystem = new CheckRoleSystem(_jwtService, _userRepository);
                CheckAdminModel checkModel = checkRoleSystem.CheckAdmin(headerValue);

                if (!checkModel.check)
                {
                    return BadRequest(new
                    {
                        message = "Bạn cần đăng nhập tài khoản Admin"
                    });
                }

                if (checkModel != null) IdUserCurrent = checkModel.Id;

                RestDateDto restDateDto = new RestDateDto();
                restDateDto  = _mapper.Map<RestDateDto>(restDayModel);

                restDateDto.Id = Guid.NewGuid();
                restDateDto.IsActived = true;
                restDateDto.IsDeleted = false;
                restDateDto.Status = 0;
                restDateDto.CreatedBy = checkModel.Id;
                restDateDto.CreatedDate = DateTime.Now;

                IdRestday = restDateDto.Id;

                Response result = await _restDateRepository.InsertRestDay(restDateDto);
                if(result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Create", "RestDay", true, restDateDto.Id);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Create", "RestDay", false, restDateDto.Id);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Create", "RestDay", false, IdRestday);
                throw;
            }
        }
        // POST: api/RestDay/DeleteRestDay
        [HttpPost]
        [Route("DeleteRestDay")]
        public async Task<IActionResult> DeleteRestDay(Guid Id)
        {
            Guid IdUserCurrent = Guid.NewGuid();
            try
            {
                //check role admin
                Request.Headers.TryGetValue("Authorization", out var headerValue);
                if(headerValue.Count == 0)
                {
                    return BadRequest(new
                    {
                        message = "Bạn cần đăng nhập tài khoản Admin"
                    });
                }
                CheckRoleSystem checkRoleSystem = new CheckRoleSystem(_jwtService, _userRepository);
                CheckAdminModel checkModel = checkRoleSystem.CheckAdmin(headerValue);

                if (!checkModel.check)
                {
                    return BadRequest(new
                    {
                        message = "Bạn cần đăng nhập tài khoản Admin"
                    });
                }
                if (checkModel != null) IdUserCurrent = checkModel.Id;


                Response result = await _restDateRepository.DeleteRestDay(Id);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Delete", "RestDay", true, Id);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Delete", "RestDay", false, Id);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Delete", "RestDay", false, Id);
                throw;
            }
        }
        // Get: api/RestDay/GetAllRestDay
        [HttpGet]
        [Route("GetAllRestDay")]
        public List<RestDayModel> GetRestDay(int pageNumber, int pageSize)
        {
            try
            {
                List<RestDateDto> restDateDto = new List<RestDateDto>();
                restDateDto = _restDateRepository.getRestDay(pageNumber, pageSize);

                List<RestDayModel> restDayModels = new List<RestDayModel>();
                restDayModels = _mapper.Map<List<RestDayModel>>(restDateDto);
                return restDayModels;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // POST: api/RestDay/UpdateRestDay
        [HttpPost]
        [Route("UpdateRestDay")]
        public async Task<IActionResult> UpdateRestDay(RestDayModel restDayModel)
        {
            Guid IdUserCurrent = Guid.NewGuid();
            try
            {
                //check role admin
                Request.Headers.TryGetValue("Authorization", out var headerValue);
                if (headerValue.Count == 0)
                {
                    return BadRequest(new
                    {
                        message = "Bạn cần đăng nhập tài khoản Admin"
                    });
                }
                CheckRoleSystem checkRoleSystem = new CheckRoleSystem(_jwtService, _userRepository);
                CheckAdminModel checkModel = checkRoleSystem.CheckAdmin(headerValue);

                if (!checkModel.check)
                {
                    return BadRequest(new
                    {
                        message = "Bạn cần đăng nhập tài khoản Admin"
                    });
                }

                if (checkModel != null) IdUserCurrent = checkModel.Id;

                RestDateDto restDateDto = new RestDateDto();
                restDateDto = _mapper.Map<RestDateDto>(restDayModel);

                Response result = await _restDateRepository.UpdateRestDay(restDateDto);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Update", "RestDay", true, restDayModel.Id);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Update", "RestDay", false, restDayModel.Id);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Update", "RestDay", false, restDayModel.Id);
                throw;
            }
        }
        // GET: api/RestDay/GetRestDayById
        [HttpGet]
        [Route("GetRestDayById")]
        public RestDayModel GetRestDayById(Guid Id)
        {
            try
            {
                RestDateDto restDateDto = new RestDateDto();
                restDateDto = _restDateRepository.getRestDateDto(Id);

                RestDayModel restDayModel = new RestDayModel();
                restDayModel = _mapper.Map<RestDayModel>(restDateDto);

                return restDayModel;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // POST: api/RestDay/ActiveRestDay
        [HttpPost]
        [Route("ActiveRestDay")]
        public IActionResult ActiveRestDay(Guid Id, bool IsActive)
        {
            Guid IdUserCurrent = Guid.NewGuid();
            string content = IsActive ? "kích hoạt ngày nghĩ" : "bỏ kích hoạt ngày nghĩ";
            try
            {
                //check role admin
                Request.Headers.TryGetValue("Authorization", out var headerValue);
                if (headerValue.Count == 0)
                {
                    return BadRequest(new
                    {
                        message = "Bạn cần đăng nhập tài khoản Admin"
                    });
                }

                CheckRoleSystem checkRoleSystem = new CheckRoleSystem(_jwtService, _userRepository);
                CheckAdminModel checkModel = checkRoleSystem.CheckAdmin(headerValue);

                if (!checkModel.check)
                {
                    return BadRequest(new
                    {
                        message = "Bạn cần đăng nhập tài khoản Admin"
                    });
                }

                if (checkModel != null) IdUserCurrent = checkModel.Id;

                Response result = _restDateRepository.ActiveYear(Id, IsActive);
                if (result.Success)
                {
                    _saveToDiary.ModifyDiary(checkModel.Id, "Update", "RestDay", true, Id, content);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.ModifyDiary(checkModel.Id, "Update", "RestDay", false, Id, content);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.ModifyDiary(IdUserCurrent, "Update", "RestDay", false, Id, content);
                throw;
            }
        }
        #endregion
    }
}
