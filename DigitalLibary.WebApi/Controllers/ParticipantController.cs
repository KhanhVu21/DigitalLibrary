using AutoMapper;
using DigitalLibary.Service.Dto;
using DigitalLibary.Service.Repository.IRepository;
using DigitalLibary.Service.Repository.RepositoryIPL;
using DigitalLibary.WebApi.Common;
using DigitalLibary.WebApi.Helper;
using DigitalLibary.WebApi.Payload;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System;
using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Common;

namespace DigitalLibary.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParticipantController: Controller
    {
        #region Variables
        private readonly IParticipantsRepository _participantsRepository;
        private readonly AppSettingModel _appSettingModel;
        private readonly IMapper _mapper;
        private readonly JwtService _jwtService;
        private readonly IUserRepository _userRepository;
        private readonly SaveToDiary _saveToDiary;
        #endregion

        #region Contructor
        public ParticipantController(IParticipantsRepository participantsRepository,
        IOptionsMonitor<AppSettingModel> optionsMonitor,
        IMapper mapper,
        SaveToDiary saveToDiary,
        JwtService jwtService, IUserRepository userRepository)
        {
            _appSettingModel = optionsMonitor.CurrentValue;
            _mapper = mapper;
            _participantsRepository = participantsRepository;
            _jwtService = jwtService;
            _userRepository = userRepository;
            _saveToDiary = saveToDiary;
        }
        #endregion

        #region METHOD
        // GET: api/Participant/GetlistMissionParticipants
        [HttpGet]
        [Route("GetlistMissionParticipants")]
        public IActionResult GetlistMissionParticipants(int pageNumber, int pageSize)
        {
            try
            {
                List<string> listName = _participantsRepository.GetlistMission(pageNumber, pageSize);
                return Ok(listName);
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    message = "Đã xảy ra lỗi",
                    Success = false,
                });

            }
        }
        // GET: api/Participant/GetlistPositionParticipants
        [HttpGet]
        [Route("GetlistPositionParticipants")]
        public IActionResult GetlistPositionParticipants(int pageNumber, int pageSize)
        {
            try
            {
                List<string> listName = _participantsRepository.GetlistPosition(pageNumber, pageSize);
                return Ok(listName);
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    message = "Đã xảy ra lỗi",
                    Success = false,
                });

            }
        }
        // GET: api/Participant/GetlistNameParticipants
        [HttpGet]
        [Route("GetlistNameParticipants")]
        public IActionResult GetlistNameParticipants(int pageNumber, int pageSize)
        {
            try
            {
                List<string> listName = _participantsRepository.GetlistName(pageNumber, pageSize);
                return Ok(listName);
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    message = "Đã xảy ra lỗi",
                    Success = false,
                });

            }
        }
        // GET: api/Participant/GetlistParticipants
        [HttpGet]
        [Route("GetlistParticipants")]
        public IActionResult GetlistParticipants(int pageNumber, int pageSize)
        {
            try
            {
                List<ParticipantsDto> participantsDtos = _participantsRepository.GetListParticipants(pageNumber, pageSize);
                return Ok(participantsDtos);
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    message = "Đã xảy ra lỗi",
                    Success = false,
                });

            }
        }
        // GET: api/Participant/GetlistParticipantsByIdReceipt
        [HttpGet]
        [Route("GetlistParticipantsByIdReceipt")]
        public IActionResult GetlistParticipantsByIdReceipt(int pageNumber, int pageSize, Guid IdReceipt)
        {
            try
            {
                List<ParticipantsDto> participantsDtos = _participantsRepository.GetListParticipantByIdReceipt(pageNumber, pageSize, IdReceipt);
                return Ok(participantsDtos);
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    message = "Đã xảy ra lỗi",
                    Success = false,
                });

            }
        }
        // GET: api/Participant/GetlistParticipantsById
        [HttpGet]
        [Route("GetlistParticipantsById")]
        public IActionResult GetlistParticipantsById(Guid IdParticipant)
        {
            try
            {
                ParticipantsDto participantsDtos = _participantsRepository.GetParticipantsById(IdParticipant);
                return Ok(participantsDtos);
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    message = "Đã xảy ra lỗi",
                    Success = false,
                });
            }
        }
        // POST: api/Participant/InsertParticipant
        [HttpPost]
        [Route("InsertParticipant")]
        public IActionResult InsertParticipant(ParticipantsModel participantModel)
        {
            Guid IdUserCurrent = Guid.NewGuid();
            Guid Idparticipants = Guid.NewGuid();
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

                ParticipantsDto participantsDto = new ParticipantsDto();
                participantsDto = _mapper.Map<ParticipantsDto>(participantModel);

                // define some col with data concrete
                participantsDto.Id = Guid.NewGuid();
                participantsDto.Status = 0;
                participantsDto.CreatedDate = DateTime.Now;

                Idparticipants = participantsDto.Id;

                Response result = _participantsRepository.InsertParticipants(participantsDto);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Create", "Participants", true, participantsDto.Id);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Create", "Participants", false, participantsDto.Id);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Create", "Participants", false, Idparticipants);
                return BadRequest(new
                {
                    message = "Đã xảy ra lỗi",
                    Success = false,
                });
            }
        }
        // POST: api/Participant/InsertParticipants
        [HttpPost]
        [Route("InsertParticipants")]
        public IActionResult InsertParticipants(List<ParticipantsModel> participantModel)
        {
            Guid IdUserCurrent = Guid.NewGuid();
            Guid Idparticipants = Guid.NewGuid();
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

                for(int i = 0; i< participantModel.Count; i++)
                {
                    ParticipantsDto participantsDto = new ParticipantsDto();
                    participantsDto = _mapper.Map<ParticipantsDto>(participantModel[i]);

                    // define some col with data concrete
                    participantsDto.Id = Guid.NewGuid();
                    participantsDto.Status = 0;
                    participantsDto.CreatedDate = DateTime.Now;

                    Idparticipants = participantsDto.Id;

                    Response result = _participantsRepository.InsertParticipants(participantsDto);
                    if (result.Success)
                    {
                        _saveToDiary.SaveDiary(checkModel.Id, "Create", "Participants", true, participantsDto.Id);
                    }
                    else
                    {
                        _saveToDiary.SaveDiary(checkModel.Id, "Create", "Participants", false, participantsDto.Id);
                    }
                }

                return Ok(new
                {
                    message = "Thêm mới thành công !"
                });

            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Create", "Participants", false, Idparticipants);
                return BadRequest(new
                {
                    message = "Đã xảy ra lỗi",
                    Success = false,
                });
            }
        }
        // HttpDelete: api/CategorySign/RemoveParticipantsById
        [HttpDelete]
        [Route("RemoveParticipantsById")]
        public IActionResult RemoveParticipantsById(Guid IdParticipant)
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

                Response result = _participantsRepository.DeleteParticipants(IdParticipant);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Delete", "Participants", true, IdParticipant);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Delete", "Participants", false, IdParticipant);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Delete", "Participants", false, IdParticipant);
                throw;
            }
        }
        // HttpPut: api/Participant/UpdateParticipant
        [HttpPut]
        [Route("UpdateParticipant")]
        public IActionResult UpdateParticipant(ParticipantsModel participantsModel)
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

                ParticipantsDto participantsDto = new ParticipantsDto();
                participantsDto = _mapper.Map<ParticipantsDto>(participantsModel);

                Response result = _participantsRepository.UpdateParticipants(participantsDto);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Update", "Participants", true, (Guid)participantsModel.Id);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Update", "Participants", false, (Guid)participantsModel.Id);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Update", "Participants", false, (Guid)participantsModel.Id);
                return BadRequest(new
                {
                    message = "Đã xảy ra lỗi",
                    Success = false,
                });
            }
        }
        #endregion
    }
}
