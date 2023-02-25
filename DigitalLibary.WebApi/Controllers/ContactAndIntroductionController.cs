using AutoMapper;
using DigitalLibary.Data.Data;
using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Common;
using DigitalLibary.Service.Dto;
using DigitalLibary.Service.Repository.IRepository;
using DigitalLibary.WebApi.Common;
using DigitalLibary.WebApi.Helper;
using DigitalLibary.WebApi.Payload;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalLibary.WebApi.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class ContactAndIntroductionController : Controller
    {
        #region Variables
        private readonly IContactAndIntroductionRepository _contactAndIntroductionRepository;
        private readonly AppSettingModel _appSettingModel;
        private readonly IMapper _mapper;
        private readonly JwtService _jwtService;
        private readonly IUserRepository _userRepository;
        private readonly DataContext _context;
        private readonly IUnitRepository _iUnitRepository;
        private readonly SaveToDiary _saveToDiary;
        #endregion

        #region Contructor
        public ContactAndIntroductionController(IContactAndIntroductionRepository contactAndIntroductionRepository,
        IOptionsMonitor<AppSettingModel> optionsMonitor,
        IMapper mapper,
        SaveToDiary saveToDiary,
        JwtService jwtService,
        IUserRepository userRepository, 
        DataContext context, 
        IUnitRepository iUnitRepository)
        {
            _appSettingModel = optionsMonitor.CurrentValue;
            _mapper = mapper;
            _contactAndIntroductionRepository = contactAndIntroductionRepository;
            _jwtService = jwtService;
            _userRepository = userRepository;
            _context = context;
            _iUnitRepository = iUnitRepository;
            _saveToDiary = saveToDiary;
        }
        #endregion

        #region MeThod
        // add, update, remove, getAll Rule
        // POST: api/ContactAndIntroduction/InsertRule
        [HttpPost]
        [Route("InsertRule")]
        public IActionResult InsertRule(ContactAndIntroductionModel contactAndIntroductionModel)
        {
            Guid IdUserCurrent = Guid.NewGuid();
            Guid IdContact = Guid.NewGuid();
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


                ContactAndIntroductionDto contactAndIntroductionDto = new ContactAndIntroductionDto();
                contactAndIntroductionDto = _mapper.Map<ContactAndIntroductionDto>(contactAndIntroductionModel);

                // define some col with data concrete
                contactAndIntroductionDto.Id = Guid.NewGuid();
                contactAndIntroductionDto.IsActived = false;
                contactAndIntroductionDto.IsDeleted = false;
                contactAndIntroductionDto.Status = 0;
                contactAndIntroductionDto.Createby = checkModel.Id;
                contactAndIntroductionDto.CreateDate = DateTime.Now;

                IdContact = contactAndIntroductionDto.Id;

                Response result = _contactAndIntroductionRepository.InsertRule(contactAndIntroductionDto);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Create", "ContactAndIntroduction", true, contactAndIntroductionDto.Id);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Create", "ContactAndIntroduction", false, contactAndIntroductionDto.Id);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Create", "ContactAndIntroduction", false, IdContact);
                throw;
            }
        }
        // POST: api/ContactAndIntroduction/DeleteRule
        [HttpPost]
        [Route("DeleteRule")]
        public IActionResult DeleteRule(Guid Id)
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

                Response result =  _contactAndIntroductionRepository.DeleteRule(Id);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Delete", "ContactAndIntroduction", true, Id);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Delete", "ContactAndIntroduction", false, Id);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Delete", "ContactAndIntroduction", false, Id);
                throw;
            }
        }
        // GET: api/ContactAndIntroduction/GetAllRules
        [HttpGet]
        [Route("GetAllRules")]
        public List<ContactAndIntroductionModel> GetAllRules(int pageNumber, int pageSize, int type)
        {
            try
            {

                List<ContactAndIntroductionDto> ruleDto = new List<ContactAndIntroductionDto>();
                ruleDto = _contactAndIntroductionRepository.getAllRule(pageNumber, pageSize, type);

                List<ContactAndIntroductionModel> ruleModel = new List<ContactAndIntroductionModel>();
                ruleModel = _mapper.Map<List<ContactAndIntroductionModel>>(ruleDto);
                return ruleModel;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/ContactAndIntroduction/GetRule
        [HttpGet]
        [Route("GetRule")]
        public ContactAndIntroductionModel GetRule(Guid Id)
        {
            try
            {
                ContactAndIntroductionDto ruleDto = new ContactAndIntroductionDto();
                ruleDto = _contactAndIntroductionRepository.getRuleClient(Id);

                ContactAndIntroductionModel ruleModel = new ContactAndIntroductionModel();
                ruleModel = _mapper.Map<ContactAndIntroductionModel>(ruleDto);
                return ruleModel;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // POST: api/ContactAndIntroduction/UpdateRule
        [HttpPost]
        [Route("UpdateRule")]
        public IActionResult UpdateRule(ContactAndIntroductionModel contactAndIntroductionModel)
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


                ContactAndIntroductionDto contactAndIntroductionDto = new ContactAndIntroductionDto();
                contactAndIntroductionDto = _mapper.Map<ContactAndIntroductionDto>(contactAndIntroductionModel);

                Response result = _contactAndIntroductionRepository.UpdateRule(contactAndIntroductionDto);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Update", "ContactAndIntroduction", true, contactAndIntroductionModel.Id);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Update", "ContactAndIntroduction", false, contactAndIntroductionModel.Id);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Update", "ContactAndIntroduction", false, contactAndIntroductionModel.Id);
                throw;
            }
        }

        // GET: api/ContactAndIntroduction/List
        [HttpGet]
        [Route("ListUnit")]
        public async Task<ActionResult<IEnumerable<Unit>>> GetListUnit(int pageNumber, int pageSize)
        {
            return await _context.Unit.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        // GET: api/ContactAndIntroduction/List
        [HttpGet]
        [Route("ListUnitNotPagination")]
        public async Task<ActionResult<IEnumerable<Unit>>> ListUnitNotPagination()
        {
            return await _context.Unit.ToListAsync();
        }

        [HttpGet]
        [Route("ListUnitByParentID")]
        public List<Unit> ListUnitByParentID(Guid ParentID, int pageNumber, int pageSize)
        {
            return _iUnitRepository.ListByParentID(ParentID, pageNumber, pageSize);
        }

        [HttpGet]
        [Route("LoadUnitByID")]
        public Unit LoadUnitByID(Guid ID)
        {
            return _iUnitRepository.LoadUnitByID(ID);
        }

        [HttpPost]
        [Route("SaveImageIntroduction")]
        public IActionResult SaveImageIntroduction([FromForm] Byte File)
        {
            try
            {
                string IdFile = Guid.NewGuid().ToString() + ".jpg";

                if (Request.Form.Files.Count > 0)
                {
                    foreach (var file in Request.Form.Files)
                    {
                        var fileContentType = file.ContentType;

                        if (fileContentType == "image/jpeg" || fileContentType == "image/png")
                        {
                            // prepare path to save file image
                            string pathTo = _appSettingModel.ServerFileImageIntroduction;

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

                return Ok(IdFile);
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    message =  "Thêm hình ảnh không thành công"
                });
            }
        }

        [HttpGet]
        [Route("GetFileImageIntroduction")]
        public IActionResult GetFileImageIntroduction(string fileNameId)
        {
            try
            {
                var temp = fileNameId.Split('.');
                byte[] fileBytes = new byte[] { };
                if (temp[1] == "jpg" || temp[1] == "png")
                {
                    fileBytes = System.IO.File.ReadAllBytes(string.Concat(_appSettingModel.ServerFileImageIntroduction, "\\", fileNameId));
                }
                return File(fileBytes, "image/jpeg");
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}
