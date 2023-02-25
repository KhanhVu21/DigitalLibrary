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
    public class SchoolYearController : Controller
    {
        #region Variables
        private readonly ISchoolYearRepository _schoolYearRepository;
        private readonly IMapper _mapper;
        private readonly JwtService _jwtService;
        private readonly IUserRepository _userRepository;
        private readonly SaveToDiary _saveToDiary;
        #endregion

        #region Contructor
        public SchoolYearController(ISchoolYearRepository schoolYearRepository,
        IMapper mapper,
        SaveToDiary saveToDiary,
        JwtService jwtService, IUserRepository userRepository)
        {
            _mapper = mapper;
            _schoolYearRepository = schoolYearRepository;
            _jwtService = jwtService;
            _userRepository = userRepository;
            _saveToDiary = saveToDiary;
        }
        #endregion

        #region MeThod
        // HttpPost: api/SchoolYear/InsertSchoolYear
        [HttpPost]
        [Route("InsertSchoolYear")]
        public async Task<IActionResult> InsertSchoolYear(SchoolYearModel schoolYearModel)
        {
            Guid IdUserCurrent = Guid.NewGuid();
            Guid IdSchoolYear = Guid.NewGuid();
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

                SchoolYearDto schoolYearDto = new SchoolYearDto();
                schoolYearDto = _mapper.Map<SchoolYearDto>(schoolYearModel);

                schoolYearDto.Id = Guid.NewGuid();
                schoolYearDto.CreatedDate = DateTime.Now;
                schoolYearDto.IsActived = true;
                schoolYearDto.Status = 0;
                schoolYearDto.CreatedBy = checkModel.Id;

                IdSchoolYear = schoolYearDto.Id;

                Response result = await _schoolYearRepository.InsertSchoolYear(schoolYearDto);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Create", "SchoolYear", true, IdSchoolYear);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Create", "SchoolYear", false, IdSchoolYear);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }

            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Create", "SchoolYear", false, IdSchoolYear);
                throw;
            }
        }
        // HttpPost: api/SchoolYear/DeleteSchoolDay
        [HttpPost]
        [Route("DeleteSchoolDay")]
        public async Task<IActionResult> DeleteSchoolDay(Guid Id)
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

                Response result = await _schoolYearRepository.DeleteSchoolYear(Id);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Delete", "SchoolYear", true, Id);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Delete", "SchoolYear", false, Id);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Delete", "SchoolYear", false, Id);
                throw;
            }
        }
        // HttpPost: api/SchoolYear/UpdateSchoolDay
        [HttpPost]
        [Route("UpdateSchoolDay")]
        public async Task<IActionResult> UpdateSchoolDay(SchoolYearModel schoolYearModel)
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

                SchoolYearDto schoolYearDto = new SchoolYearDto();
                schoolYearDto = _mapper.Map<SchoolYearDto>(schoolYearModel);

                Response result = await _schoolYearRepository.UpdateSchoolYear(schoolYearDto);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Update", "SchoolYear", true, schoolYearModel.Id);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Update", "SchoolYear", false, schoolYearModel.Id);
                    return BadRequest(new
                        {
                            message = result.Message
                        });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Update", "SchoolYear", false, schoolYearModel.Id);
                throw;
            }
        }
        // HttpGet: api/SchoolYear/GetAllSchoolYear
        [HttpGet]
        [Route("GetAllSchoolYear")]
        public async Task<List<SchoolYearModel>> GetAllSchoolYear(int pageNumber, int pageSize)
        {
            try
            {

                List<SchoolYearDto> schoolYearDtos = new List<SchoolYearDto>();
                schoolYearDtos = _schoolYearRepository.getSchoolYear(pageNumber, pageSize);

                List<SchoolYearModel> schoolYearModels = new List<SchoolYearModel>();
                schoolYearModels = _mapper.Map<List<SchoolYearModel>>(schoolYearDtos);
                int yearNow = DateTime.Now.Year;

                if (schoolYearModels.Count == 0)
                {
                    //assigns year, month, day
                    DateTime StartSemesterI = new DateTime(yearNow, 08, 01);
                    DateTime StartSemesterII = new DateTime(yearNow + 1, 02, 01);
                    DateTime EndAllSemester = new DateTime(yearNow + 1, 07, 01);

                    SchoolYearDto schoolYearDto = new SchoolYearDto()
                    {
                        FromYear = StartSemesterI,
                        ToYear = StartSemesterII,
                        StartSemesterI = StartSemesterI,
                        StartSemesterII = StartSemesterII,
                        EndAllSemester = EndAllSemester,
                        CreatedDate = DateTime.Now,
                        IsActived = false,
                        Status = 1
                    };
                    Response result = await _schoolYearRepository.InsertSchoolYear(schoolYearDto);
                }

                string toYear = schoolYearModels[0].EndAllSemester.Value.ToString("yyyy");
                if (DateTime.Now >= schoolYearModels[0].EndAllSemester.Value)
                {
                    //assigns year, month, day
                    DateTime StartSemesterI = new DateTime(int.Parse(toYear), 08, 01);
                    DateTime StartSemesterII = new DateTime(int.Parse(toYear) + 1, 02, 01);
                    DateTime EndAllSemester = new DateTime(int.Parse(toYear) + 1, 07, 01);

                    SchoolYearDto schoolYearDto = new SchoolYearDto()
                    {
                        FromYear = StartSemesterI,
                        ToYear = StartSemesterII,
                        StartSemesterI = StartSemesterI,
                        StartSemesterII = StartSemesterII,
                        EndAllSemester = EndAllSemester,
                        CreatedDate = DateTime.Now,
                        IsActived = false,
                        Status = 1
                    };
                    Response result = await _schoolYearRepository.InsertSchoolYear(schoolYearDto);
                }

                return schoolYearModels;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/SchoolYear/GetSchoolYearById
        [HttpGet]
        [Route("GetSchoolYearById")]
        public SchoolYearModel GetSchoolYearById(Guid Id)
        {
            try
            {
                SchoolYearDto schoolYearDto = new SchoolYearDto();
                schoolYearDto = _schoolYearRepository.getSchoolYear(Id);

                SchoolYearModel schoolYearModel = new SchoolYearModel();
                schoolYearModel = _mapper.Map<SchoolYearModel>(schoolYearDto);
                return schoolYearModel;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // POST: api/SchoolYear/ActiveSchoolYear
        [HttpPost]
        [Route("ActiveSchoolYear")]
        public IActionResult ActiveSchoolYear(Guid Id, bool IsActive)
        {
            Guid IdUserCurrent = Guid.NewGuid();
            string content = IsActive ? "duyệt năm học" : "bỏ duyệt năm học";
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

                Response result = _schoolYearRepository.ActiveYear(Id, IsActive);
                if (result.Success)
                {
                    _saveToDiary.ModifyDiary(checkModel.Id, "Update", "SchoolYear", true, Id, content);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.ModifyDiary(checkModel.Id, "Update", "SchoolYear", false, Id, content);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.ModifyDiary(IdUserCurrent, "Update", "SchoolYear", false, Id, content);
                throw;
            }
        }
        #endregion
    }
}
