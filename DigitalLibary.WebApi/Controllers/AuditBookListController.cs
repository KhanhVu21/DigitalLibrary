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
    public class AuditBookListController: Controller
    {
        #region Variables
        private readonly IAuditBookListRepository _AuditBookListRepository;
        private readonly AppSettingModel _appSettingModel;
        private readonly IMapper _mapper;
        private readonly JwtService _jwtService;
        private readonly IUserRepository _userRepository;
        private readonly SaveToDiary _saveToDiary;
        private readonly ILogger<AuditBookListController> _logger;
        #endregion

        #region Contructor
        public AuditBookListController(IAuditBookListRepository AuditBookListRepository,
        IOptionsMonitor<AppSettingModel> optionsMonitor,
        IMapper mapper,
        SaveToDiary saveToDiary,
        JwtService jwtService, IUserRepository userRepository, ILogger<AuditBookListController> logger)
        {
            _appSettingModel = optionsMonitor.CurrentValue;
            _mapper = mapper;
            _AuditBookListRepository = AuditBookListRepository;
            _jwtService = jwtService;
            _userRepository = userRepository;
            _saveToDiary = saveToDiary;
            _logger = logger;
        }
        #endregion

        #region METHOD
        // GET: api/AuditBookList/GetAllAuditBookList
        [HttpGet]
        [Route("GetAllAuditBookList")]
        public IEnumerable<AuditBookListDto> GetAllAuditBookList(int pageNumber, int pageSize)
        {
            try
            {
                var result = _AuditBookListRepository.GetAllAuditBookList(pageNumber, pageSize);
                _logger.LogInformation("Lấy danh sách thành công !");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Lấy danh sách không thành công: {message}", ex.Message);
                throw;
            }
        }
        // GET: api/AuditBookList/GetAuditorById
        [HttpGet]
        [Route("GetAuditorById")]
        public AuditBookListDto GetAuditorById(Guid IdAuditBookList)
        {
            try
            {
                var result = _AuditBookListRepository.GetAllAuditBookListById(IdAuditBookList);
                _logger.LogInformation("Lấy thành công !");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Lấy không thành công: {message}", ex.Message);
                throw;
            }
        }
        // HttpPost: api/AuditBookList/InsertAuditBookList
        [HttpPost]
        [Route("InsertAuditBookList")]
        public IActionResult InsertAuditBookList(AuditBookListModel AuditBookListModel)
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

                var AuditBookListDto = _mapper.Map<AuditBookListDto>(AuditBookListModel);
                var result = _AuditBookListRepository.InsertAuditBookList(AuditBookListDto);

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
        // HttpPut: api/AuditBookList/UpdateAuditBookList
        [HttpPut]
        [Route("UpdateAuditBookList")]
        public IActionResult UpdateAuditBookList(AuditBookListModel AuditBookListModel)
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

                var AuditBookListDto = _mapper.Map<AuditBookListDto>(AuditBookListModel);
                var result = _AuditBookListRepository.UpdateAuditBookList(AuditBookListDto.Id, AuditBookListDto);

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
        // HttpDelete: api/AuditBookList/DeleteAuditBookList
        [HttpDelete]
        [Route("DeleteAuditBookList")]
        public IActionResult DeleteAuditBookList(Guid IdAuditBookList)
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

                var result = _AuditBookListRepository.DeleteAuditBookList(IdAuditBookList);
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
        // HttpDelete: api/AuditBookList/DeleteAuditorByList
        [HttpDelete]
        [Route("DeleteAuditorByList")]
        public IActionResult DeleteAuditorByList(List<Guid> IdAuditBookList)
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

                var result = _AuditBookListRepository.DeleteAuditBookListByList(IdAuditBookList);

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
