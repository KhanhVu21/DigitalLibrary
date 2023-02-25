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
using System.IO;

namespace DigitalLibary.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SlideController : Controller
    {
        #region Variables
        private readonly ISlideRepository _slideRepository;
        private readonly AppSettingModel _appSettingModel;
        private readonly IMapper _mapper;
        private readonly JwtService _jwtService;
        private readonly IUserRepository _userRepository;
        private readonly SaveToDiary _saveToDiary;
        #endregion

        #region Contructor
        public SlideController(ISlideRepository slideRepository,
        IOptionsMonitor<AppSettingModel> optionsMonitor,
        IMapper mapper,
        SaveToDiary saveToDiary,
        JwtService jwtService, IUserRepository userRepository)
        {
            _appSettingModel = optionsMonitor.CurrentValue;
            _mapper = mapper;
            _slideRepository = slideRepository;
            _jwtService = jwtService;
            _saveToDiary = saveToDiary;
            _userRepository = userRepository;
        }
        #endregion

        #region MeThod
        // GET: api/Slide/GetAllSlideAdmin
        [HttpGet]
        [Route("GetAllSlideAdmin")]
        public List<SlideModel> GetAllSlideAdmin(int pageNumber, int pageSize)
        {
            try
            {
                List<SlideDto> slideDtos = new List<SlideDto>();
                slideDtos = _slideRepository.GetAllSlideAdmin(pageNumber, pageSize);

                List<SlideModel> slideModels = new List<SlideModel>();
                slideModels = _mapper.Map<List<SlideModel>>(slideDtos);

                return slideModels;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/Slide/GetAllSlideClient
        [HttpGet]
        [Route("GetAllSlideClient")]
        public List<SlideModel> GetAllSlideClient(int pageNumber, int pageSize)
        {
            try
            {
                List<SlideDto> slideDtos = new List<SlideDto>();
                slideDtos = _slideRepository.GetAllSlideClient(pageNumber, pageSize);

                List<SlideModel> slideModels = new List<SlideModel>();
                slideModels = _mapper.Map<List<SlideModel>>(slideDtos);

                return slideModels;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //CRUD Table Slide
        // POST: api/Slide/DeleteSlideByID
        [HttpPost]
        [Route("DeleteSlideByID")]
        public IActionResult DeleteSlideByID(Guid Id)
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


                Response result = _slideRepository.DeleteSlide(Id);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Delete", "Slide", true, Id);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Delete", "Slide", false, Id);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Delete", "Slide", false, Id);
                throw;
            }
        }

        // POST: api/Slide/HideSlideById
        [HttpPost]
        [Route("HideSlideById")]
        public IActionResult HideSlideById(Guid Id, bool check)
        {
            Guid IdUserCurrent = Guid.NewGuid();
            string content = check ? "ẩn slide" : "bỏ ẩn slide";
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
                

                Response result = _slideRepository.HideSlide(Id, check);
                if (result.Success)
                {
                    _saveToDiary.ModifyDiary(checkModel.Id, "Update", "Slide", true, Id, content);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.ModifyDiary(checkModel.Id, "Update", "Slide", false, Id, content);
                    return BadRequest(new
                        {
                            message = result.Message
                        });

                }
            }
            catch (Exception)
            {
                _saveToDiary.ModifyDiary(IdUserCurrent, "Update", "Slide", false, Id, content);
                throw;
            }
        }
        // POST: api/Slide/InsertSlide
        [HttpPost]
        [Route("InsertSlide")]
        public IActionResult InsertSlide([FromForm]SlideModel slideModel)
        {
            Guid IdUserCurrent = Guid.NewGuid();
            Guid IdSlide = Guid.NewGuid();
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


                SlideDto slideDto = new SlideDto();
                slideDto = _mapper.Map<SlideDto>(slideModel);

                // define some col with data concrete
                slideDto.Id = Guid.NewGuid();
                slideDto.Status = 0;
                slideDto.CreatedDate = DateTime.Now;
                slideDto.CreatedBy = checkModel.Id;
                slideDto.IsDelete = false;
                slideDto.IsHide = false;
                slideDto.FileName = slideDto.Id.ToString() + ".jpg";
                slideDto.FileNameExtention = "jpg";
                slideDto.FilePath = _appSettingModel.ServerFileSlide;

                IdSlide = slideDto.Id;

                if (Request.Form.Files.Count > 0)
                {
                    foreach (var file in Request.Form.Files)
                    {
                        var fileContentType = file.ContentType;

                        if (fileContentType == "image/jpeg" || fileContentType == "image/png")
                        {
                            // prepare path to save file image
                            string pathTo = _appSettingModel.ServerFileSlide;
                            // get extention form file name
                            string IdFile = slideDto.Id.ToString() + ".jpg";

                            // set file path to save file
                            var filename = Path.Combine(pathTo, Path.GetFileName(IdFile));

                            // save file
                            using (var stream = System.IO.File.Create(filename))
                            {
                                file.CopyTo(stream);
                            }
                        }
                    }
                }


                Response result = _slideRepository.InsertSlide(slideDto);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Create", "Slide", true, slideModel.Id);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Create", "Slide", false, slideModel.Id);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Create", "Slide", false, slideModel.Id);
                throw;
            }
        }
        // POST: api/Slide/UpdateSlide
        [HttpPost]
        [Route("UpdateSlide")]
        public IActionResult UpdateSlide([FromForm]SlideModel slideModel)
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

                SlideDto slideDto = new SlideDto();
                slideDto = _mapper.Map<SlideDto>(slideModel);

                // check file exits
                if (Request.Form.Files.Count > 0)
                {
                    foreach (var file in Request.Form.Files)
                    {
                        var fileContentType = file.ContentType;

                        if (fileContentType == "image/jpeg" || fileContentType == "image/png")
                        {
                            // prepare path to save file image
                            string pathTo = _appSettingModel.ServerFileSlide;
                            // get extention form file name
                            string IdFile = slideDto.Id.ToString() + ".jpg";

                            // set file path to save file
                            var filename = Path.Combine(pathTo, Path.GetFileName(IdFile));

                            //delete file before save
                            if (System.IO.File.Exists(filename))
                            {
                                System.IO.File.Delete(filename);
                            }

                            // save file
                            using (var stream = System.IO.File.Create(filename))
                            {
                                file.CopyTo(stream);
                            }
                        }
                    }
                }

                Response result = _slideRepository.UpdateSlide(slideDto);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Update", "Slide", true, slideModel.Id);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Update", "Slide", false, slideModel.Id);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Update", "Slide", false, slideModel.Id);
                throw;
            }
        }
        // GET: api/Slide/GetSlideById
        [HttpGet]
        [Route("GetSlideById")]
        public SlideModel GetSlideById(Guid Id)
        {
            try
            {
                SlideDto slideDto = new SlideDto();
                slideDto = _slideRepository.getSlideById(Id);

                SlideModel slideModel = new SlideModel();
                slideModel = _mapper.Map<SlideModel>(slideDto);

                return slideModel;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
