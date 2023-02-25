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

namespace DigitalLibary.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentTypeController : Controller
    {
        #region Variables
        private readonly IDocumentTypeRepository _documentTypeRepository;
        private readonly AppSettingModel _appSettingModel;
        private readonly IMapper _mapper;
        private readonly JwtService _jwtService;
        private readonly IUserRepository _userRepository;
        private readonly IBookRepository _bookRepository;
        private readonly SaveToDiary _saveToDiary;
        #endregion

        #region Contructor
        public DocumentTypeController(IDocumentTypeRepository documentTypeRepository,
        IOptionsMonitor<AppSettingModel> optionsMonitor,
        IMapper mapper,
        SaveToDiary saveToDiary,
        IBookRepository bookRepository,
        JwtService jwtService, IUserRepository userRepository)
        {
            _appSettingModel = optionsMonitor.CurrentValue;
            _mapper = mapper;
            _documentTypeRepository = documentTypeRepository;
            _jwtService = jwtService;
            _userRepository = userRepository;
            _bookRepository = bookRepository;
            _saveToDiary = saveToDiary;
        }
        #endregion

        #region Method
        // GET: api/DocumentType/GetAllTypeNotPage
        [HttpGet]
        [Route("GetAllTypeNotPage")]
        public List<DocumentTypeModel> GetAllTypeNotPage(int status)
        {
            try
            {
                List<DocumentTypeDto> documentTypeDtos = new List<DocumentTypeDto>();
                documentTypeDtos = _documentTypeRepository.GetAllDocumentType(status);

                List<DocumentTypeModel> documentTypeModels = new List<DocumentTypeModel>();
                documentTypeModels = _mapper.Map<List<DocumentTypeModel>>(documentTypeDtos);
                return documentTypeModels;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/DocumentType/GetAllDocumentType
        [HttpGet]
        [Route("GetAllDocumentType")]
        public List<DocumentTypeModel> GetAllDocumentType(int pageNumber, int pageSize)
        {
            try
            {
                List<DocumentTypeDto> documentTypeDtos = new List<DocumentTypeDto>();
                documentTypeDtos = _documentTypeRepository.GetAllDocumentType(pageNumber, pageSize);

                List<DocumentTypeModel> documentTypeModels = new List<DocumentTypeModel>();
                documentTypeModels = _mapper.Map<List<DocumentTypeModel>>(documentTypeDtos);
                return documentTypeModels;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/DocumentType/GetAllDocumentTypeByParentId
        [HttpGet]
        [Route("GetAllDocumentTypeByParentId")]
        public List<DocumentTypeModel> GetAllDocumentTypeByParentId(Guid Id)
        {
            try
            {
                List<DocumentTypeDto> documentTypeDtos = new List<DocumentTypeDto>();
                documentTypeDtos = _documentTypeRepository.GetAllDocumentTypeByParentId(Id);

                List<DocumentTypeModel> documentStockModels = new List<DocumentTypeModel>();
                documentStockModels = _mapper.Map<List<DocumentTypeModel>>(documentTypeDtos);
                return documentStockModels;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/DocumentType/GetDocumentTypeById
        [HttpGet]
        [Route("GetDocumentTypeById")]
        public DocumentTypeModel GetDocumentTypeById(Guid Id)
        {
            try
            {
                DocumentTypeDto documentTypeDto = new DocumentTypeDto();
                documentTypeDto = _documentTypeRepository.GetAllDocumentTypeById(Id);

                DocumentTypeModel documentTypeModel = new DocumentTypeModel();
                documentTypeModel = _mapper.Map<DocumentTypeModel>(documentTypeDto);
                return documentTypeModel;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // POST: api/DocumentType/InsertDocumentType
        [HttpPost]
        [Route("InsertDocumentType")]
        public IActionResult InsertDocumentType(DocumentTypeModel documentTypeModel)
        {
            Guid IdUserCurrent = Guid.NewGuid();
            Guid IdDocumentType = Guid.NewGuid();
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

                DocumentTypeDto documentTypeDto = new DocumentTypeDto();
                documentTypeDto = _mapper.Map<DocumentTypeDto>(documentTypeModel);

                // define some col with data concrete
                documentTypeDto.Id = Guid.NewGuid();
                documentTypeDto.CreatedDate = DateTime.Now;
                documentTypeDto.CreatedBy = checkModel.Id;
                documentTypeDto.IsDeleted = false;

                IdDocumentType = documentTypeDto.Id;

                Response result = _documentTypeRepository.InsertDocumentType(documentTypeDto);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Create", "DocumentType", true, IdDocumentType);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Create", "DocumentType", false, IdDocumentType);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Create", "DocumentType", false, IdDocumentType);
                throw;
            }
        }
        // POST: api/DocumentType/UpdateDocumentType
        [HttpPost]
        [Route("UpdateDocumentType")]
        public IActionResult UpdateDocumentType(DocumentTypeModel documentTypeModel)
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


                DocumentTypeDto documentTypeDto = new DocumentTypeDto();
                documentTypeDto = _mapper.Map<DocumentTypeDto>(documentTypeModel);

                Response result = _documentTypeRepository.UpdateDocumentType(documentTypeDto);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Update", "DocumentType", true, documentTypeModel.Id);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Update", "DocumentType", false, documentTypeModel.Id);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Update", "DocumentType", false, documentTypeModel.Id);
                throw;
            }
        }
        // POST: api/DocumentType/CheckParent
        [HttpPost]
        [Route("CheckParent")]
        public IActionResult CheckParent(Guid Id)
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

                List<DocumentTypeDto> documentTypeDtos = new List<DocumentTypeDto>();
                documentTypeDtos = _documentTypeRepository.GetAllDocumentTypeByParentId(Id);

                if (documentTypeDtos.Count > 0)
                {
                    return Ok(new
                    {
                        message = "Kệ này còn chứa thông tin kèm theo",
                        parent = true
                    });
                }
                else
                {
                    return Ok(new
                    {
                        message = "Kệ này không chứa thông tin kèm theo",
                        parent = false
                    });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        // POST: api/DocumentType/DeleteDocumentType
        [HttpPost]
        [Route("DeleteDocumentType")]
        public IActionResult DeleteDocumentType(Guid Id)
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


                Response result = _documentTypeRepository.DeleteDocumentType(Id);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Delete", "DocumentType", true, Id);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Delete", "DocumentType", false, Id);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Delete", "DocumentType", false, Id);
                throw;
            }
        }
        #endregion
    }
}
