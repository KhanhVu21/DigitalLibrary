using AutoMapper;
using DigitalLibary.Service.Dto;
using DigitalLibary.Service.Repository.IRepository;
using DigitalLibary.WebApi.Common;
using DigitalLibary.WebApi.Helper;
using DigitalLibary.WebApi.Payload;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System;

namespace DigitalLibary.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditorListController: Controller
    {
        #region Variables
        private readonly IAuditorListRepository _AuditorListRepository;
        private readonly AppSettingModel _appSettingModel;
        private readonly IMapper _mapper;
        private readonly JwtService _jwtService;
        private readonly IUserRepository _userRepository;
        private readonly SaveToDiary _saveToDiary;
        private readonly ILogger<AuditorListController> _logger;
        #endregion

        #region Contructor
        public AuditorListController(IAuditorListRepository AuditorListRepository,
        IOptionsMonitor<AppSettingModel> optionsMonitor,
        IMapper mapper,
        SaveToDiary saveToDiary,
        JwtService jwtService, IUserRepository userRepository, ILogger<AuditorListController> logger)
        {
            _appSettingModel = optionsMonitor.CurrentValue;
            _mapper = mapper;
            _AuditorListRepository = AuditorListRepository;
            _jwtService = jwtService;
            _userRepository = userRepository;
            _saveToDiary = saveToDiary;
            _logger = logger;
        }
        #endregion

        #region METHOD
        // GET: api/AuditorList/GetAllAuditorList
        [HttpGet]
        [Route("GetAllAuditorList")]
        public IEnumerable<AuditorListDto> GetAllAuditorList(int pageNumber, int pageSize)
        {
            try
            {
                var result = _AuditorListRepository.GetAllAuditorList(pageNumber, pageSize);
                _logger.LogInformation("Lấy danh sách thành công !");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Lấy danh sách không thành công: {message}", ex.Message);
                throw;
            }
        }
        // GET: api/AuditorList/GetAuditorById
        [HttpGet]
        [Route("GetAuditorById")]
        public AuditorListDto GetAuditorById(Guid IdAuditorList)
        {
            try
            {
                var result = _AuditorListRepository.GetAllAuditorListById(IdAuditorList);
                _logger.LogInformation("Lấy thành công !");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Lấy không thành công: {message}", ex.Message);
                throw;
            }
        }
        // HttpPost: api/AuditorList/InsertAuditorList
        [HttpPost]
        [Route("InsertAuditorList")]
        public IActionResult InsertAuditorList(AuditorListModel AuditorListModel)
        {
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

                var AuditorListDto = _mapper.Map<AuditorListDto>(AuditorListModel);
                var result = _AuditorListRepository.InsertAuditorList(AuditorListDto);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Thêm mới không thành công: {message}", ex.Message);
                return BadRequest(new
                {
                    Success = false,
                    Fail = true,
                    Message = "Thêm mới không thành công !"
                });
            }
        }
        // HttpPut: api/AuditorList/UpdateAuditorList
        [HttpPut]
        [Route("UpdateAuditorList")]
        public IActionResult UpdateAuditorList(AuditorListModel AuditorListModel)
        {
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

                var AuditorListDto = _mapper.Map<AuditorListDto>(AuditorListModel);
                var result = _AuditorListRepository.UpdateAuditorList(AuditorListDto.Id, AuditorListDto);

                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogInformation("Cập nhật không thành công: {message}", ex.Message);
                return BadRequest(new
                {
                    Success = false,
                    Fail = true,
                    Message = "Cập nhật không thành công !"
                });
            }
        }
        // HttpDelete: api/AuditorList/DeleteAuditorList
        [HttpDelete]
        [Route("DeleteAuditorList")]
        public IActionResult DeleteAuditorList(Guid IdAuditorList)
        {
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

                var result = _AuditorListRepository.DeleteAuditorList(IdAuditorList);
                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogInformation("Xóa không thành công: {message}", ex.Message);
                return BadRequest(new
                {
                    Success = false,
                    Fail = true,
                    Message = "Xóa không thành công !"
                });
            }
        }
        // HttpDelete: api/AuditorList/DeleteAuditorByList
        [HttpDelete]
        [Route("DeleteAuditorByList")]
        public IActionResult DeleteAuditorByList(List<Guid> IdAuditorList)
        {
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

                var result = _AuditorListRepository.DeleteAuditorListByList(IdAuditorList);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Xóa không thành công: {message}", ex.Message);
                return BadRequest(new
                {
                    Success = false,
                    Fail = true,
                    Message = "Xóa không thành công !"
                });
            }
        }
        #endregion
    }
}
