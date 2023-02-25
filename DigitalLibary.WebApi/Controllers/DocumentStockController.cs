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
    public class DocumentStockController : Controller
    {
        #region Variables
        private readonly IDocumentStockRepository _documentStockRepository;
        private readonly AppSettingModel _appSettingModel;
        private readonly IMapper _mapper;
        private readonly JwtService _jwtService;
        private readonly IUserRepository _userRepository;
        private readonly SaveToDiary _saveToDiary;
        #endregion

        #region Contructor
        public DocumentStockController(IDocumentStockRepository documentStockRepository,
        IOptionsMonitor<AppSettingModel> optionsMonitor,
        IMapper mapper,
        SaveToDiary saveToDiary,
        JwtService jwtService, IUserRepository userRepository)
        {
            _appSettingModel = optionsMonitor.CurrentValue;
            _mapper = mapper;
            _documentStockRepository = documentStockRepository;
            _jwtService = jwtService;
            _saveToDiary = saveToDiary;
            _userRepository = userRepository;
        }
        #endregion

        #region Method
        // GET: api/DocumentStock/GetAllStockNotPage
        [HttpGet]
        [Route("GetAllStockNotPage")]
        public List<DocumentStockModel> GetAllStockNotPage()
        {
            try
            {
                List<DocumentStockDto> documentStockDtos = new List<DocumentStockDto>();
                documentStockDtos = _documentStockRepository.GetAllDocumentStocks();

                List<DocumentStockModel> documentStockModels = new List<DocumentStockModel>();
                documentStockModels = _mapper.Map<List<DocumentStockModel>>(documentStockDtos);
                return documentStockModels;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/DocumentStock/GetAllStock
        [HttpGet]
        [Route("GetAllStock")]
        public List<DocumentStockModel> GetAllStock(int pageNumber, int pageSize)
        {
            try
            {
                List<DocumentStockDto> documentStockDtos = new List<DocumentStockDto>();
                documentStockDtos = _documentStockRepository.GetAllDocumentStocks(pageNumber, pageSize);

                List<DocumentStockModel> documentStockModels = new List<DocumentStockModel>();
                documentStockModels = _mapper.Map<List<DocumentStockModel>>(documentStockDtos);
                return documentStockModels;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/DocumentStock/GetAllDocumentStocksByParentId
        [HttpGet]
        [Route("GetAllDocumentStocksByParentId")]
        public List<DocumentStockModel> GetAllDocumentStocksByParentId(Guid Id)
        {
            try
            {
                List<DocumentStockDto> documentStockDtos = new List<DocumentStockDto>();
                documentStockDtos = _documentStockRepository.GetAllDocumentStocksByParentId(Id);

                List<DocumentStockModel> documentStockModels = new List<DocumentStockModel>();
                documentStockModels = _mapper.Map<List<DocumentStockModel>>(documentStockDtos);
                return documentStockModels;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // POST: api/DocumentStock/InsertStock
        [HttpPost]
        [Route("InsertStock")]
        public IActionResult InsertStock(DocumentStockModel documentStockModel)
        {
            Guid IdUserCurrent = Guid.NewGuid();
            Guid IdStock = Guid.NewGuid();
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

                DocumentStockDto documentStockDto = new DocumentStockDto();
                documentStockDto = _mapper.Map<DocumentStockDto>(documentStockModel);

                Response result = null;
                string StockCode = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();

                // define some col with data concrete
                documentStockDto.Id = Guid.NewGuid();
                documentStockDto.StockCode = StockCode;
                documentStockDto.Status = 0;
                documentStockDto.CreatedDate = DateTime.Now;
                documentStockDto.CreatedBy = checkModel.Id;
                documentStockDto.IsDeleted = false;

                IdStock = documentStockDto.Id;

                result = _documentStockRepository.InsertDocumentStock(documentStockDto);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Create", "DocumentStock", true, IdStock);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Create", "DocumentStock", false, IdStock);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Create", "DocumentStock", false, IdStock);
                throw;
            }
        }
        // POST: api/DocumentStock/UpdateDocumentStock
        [HttpPost]
        [Route("UpdateDocumentStock")]
        public IActionResult UpdateDocumentStock(DocumentStockModel documentStockModel)
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


                DocumentStockDto documentStockDto = new DocumentStockDto();
                documentStockDto = _mapper.Map<DocumentStockDto>(documentStockModel);

                Response result = _documentStockRepository.UpdateDocumentStock(documentStockDto);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Update", "DocumentStock", true, documentStockModel.Id);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Update", "DocumentStock", false, documentStockModel.Id);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Update", "DocumentStock", false, documentStockModel.Id);
                throw;
            }
        }
        // POST: api/DocumentStock/CheckParent
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

                List<DocumentStockDto> documentStockDto = new List<DocumentStockDto>();
                documentStockDto = _documentStockRepository.GetAllDocumentStocksByParentId(Id);

                if (documentStockDto.Count > 0)
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
        // POST: api/DocumentStock/DeleteDocumentStock
        [HttpPost]
        [Route("DeleteDocumentStock")]
        public IActionResult DeleteDocumentStock(Guid Id)
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


                bool check = _documentStockRepository.CheckBookInStock(Id);
                if(check)
                {
                    return BadRequest(new
                    {
                        message = "Kho này đã có sách không thể xóa"
                    });
                }

                Response result = _documentStockRepository.DeleteDocumentStock(Id);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Delete", "DocumentStock", true, Id);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Delete", "DocumentStock", false, Id);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Delete", "DocumentStock", false, Id);
                throw;
            }
        }
        // GET: api/DocumentStock/GetAllDocumentStockId
        [HttpGet]
        [Route("GetAllDocumentStockId")]
        public DocumentStockModel GetAllDocumentStockId(Guid Id)
        {
            try
            {
                DocumentStockDto documentStockDtos = new DocumentStockDto();
                documentStockDtos = _documentStockRepository.GetAllDocumentStocksById(Id);

                DocumentStockModel documentStockModels = new DocumentStockModel();
                documentStockModels = _mapper.Map<DocumentStockModel>(documentStockDtos);
                return documentStockModels;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
