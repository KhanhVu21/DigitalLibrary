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

namespace DigitalLibary.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnitController : Controller
    {
        #region Variables
        private readonly IUnitRepository _unitRepository;
        private readonly AppSettingModel _appSettingModel;
        private readonly IMapper _mapper;
        private readonly JwtService _jwtService;
        private readonly IUserRepository _userRepository;
        private readonly SaveToDiary _saveToDiary;
        #endregion

        #region Contructor
        public UnitController(IUnitRepository unitRepository,
        IOptionsMonitor<AppSettingModel> optionsMonitor,
        IMapper mapper,
        JwtService jwtService,
        SaveToDiary saveToDiary,
        IUserRepository userRepository)
        {
            _appSettingModel = optionsMonitor.CurrentValue;
            _mapper = mapper;
            _unitRepository = unitRepository;
            _jwtService = jwtService;
            _userRepository = userRepository;
            _saveToDiary = saveToDiary;
        }
        #endregion

        #region Method
        // GET: api/Unit/InsertUnit
        [HttpPost]
        [Route("InsertUnit")]
        public IActionResult InsertUnit(UnitModel unitModel)
        {
            Guid IdUserCurrent = Guid.NewGuid();
            Guid IdUnit = Guid.NewGuid();
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

                UnitDto unitDto = new UnitDto();
                unitDto = _mapper.Map<UnitDto>(unitModel);

                // define some col with data concrete
                unitDto.Id = Guid.NewGuid();
                unitDto.Status = 0;
                unitDto.CreatedBy = checkModel.Id;
                unitDto.CreatedDate = DateTime.Now;

                IdUnit = unitDto.Id;

                //save to table user
                Response result = _unitRepository.InsertUnit(unitDto);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Create", "Unit", true, IdUnit);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Create", "Unit", false, IdUnit);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Create", "Unit", false, IdUnit);
                throw;
            }
        }
        // POST: api/Unit/UpdateUnit
        [HttpPost]
        [Route("UpdateUnit")]
        public IActionResult UpdateUnit(UnitModel unitModel)
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

                UnitDto unitDto = new UnitDto();
                unitDto = _mapper.Map<UnitDto>(unitModel);

                Response result = _unitRepository.UpdateUnit(unitDto);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Update", "Unit", true, unitModel.Id);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Update", "Unit", false, unitModel.Id);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Update", "Unit", false, unitModel.Id);
                throw;
            }
        }
        // POST: api/Unit/DeleteUnit
        [HttpPost]
        [Route("DeleteUnit")]
        public IActionResult DeleteUnit(Guid Id)
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

                Response result = _unitRepository.DeleteUnit(Id);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Delete", "Unit", true, Id);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Delete", "Unit", false, Id);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Delete", "Unit", false, Id);
                throw;
            }
        }
        #endregion
    }
}
