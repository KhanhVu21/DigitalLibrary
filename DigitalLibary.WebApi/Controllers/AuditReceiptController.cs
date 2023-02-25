using AutoMapper;
using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Common;
using DigitalLibary.Service.Common.FormatApi;
using DigitalLibary.Service.Dto;
using DigitalLibary.Service.Repository.IRepository;
using DigitalLibary.Service.Repository.RepositoryIPL;
using DigitalLibary.WebApi.Common;
using DigitalLibary.WebApi.Helper;
using DigitalLibary.WebApi.Payload;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace DigitalLibary.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditReceiptController: Controller
    {
        #region Variables
        private readonly IAuditReceiptRepository _auditReceiptRepository;
        private readonly AppSettingModel _appSettingModel;
        private readonly IMapper _mapper;
        private readonly JwtService _jwtService;
        private readonly IUserRepository _userRepository;
        private readonly SaveToDiary _saveToDiary;
        private readonly ILogger<AuditReceiptController> _logger;
        private readonly IBookRepository _bookRepository;
        private readonly IIndividualSampleRepository _individualSampleRepository;
        private readonly IDocumentTypeRepository _documentTypeRepository;
        private readonly IAuditorListRepository _auditorListRepository;
        #endregion

        #region Contructor
        public AuditReceiptController(IAuditReceiptRepository auditReceiptRepository,
        IBookRepository bookRepository,
        IIndividualSampleRepository individualSampleRepository,
        IOptionsMonitor<AppSettingModel> optionsMonitor,
        IDocumentTypeRepository documentTypeRepository,
        IAuditorListRepository auditorListRepository,
        IMapper mapper,
        SaveToDiary saveToDiary,
        JwtService jwtService, IUserRepository userRepository, ILogger<AuditReceiptController> logger)
        {
            _appSettingModel = optionsMonitor.CurrentValue;
            _mapper = mapper;
            _auditReceiptRepository = auditReceiptRepository;
            _jwtService = jwtService;
            _userRepository = userRepository;
            _saveToDiary = saveToDiary;
            _logger = logger;
            _bookRepository = bookRepository;
            _individualSampleRepository = individualSampleRepository;
            _documentTypeRepository = documentTypeRepository;
            _auditorListRepository = auditorListRepository;
        }
        #endregion

        #region METHOD
        // HttpGet: api/AuditReceipt/GetListBookToAuditReceipt
        [HttpGet]
        [Route("GetListBookToAuditReceipt")]
        public IActionResult GetListBookToAuditReceipt(string filter, Guid IdDocumentType, int pageNumber, int pageSize)
        {
            try
            {
                var result = _auditReceiptRepository.GetListBookToAuditReceipt(filter, IdDocumentType, pageNumber, pageSize);
                _logger.LogInformation("Lấy thành công !");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Lấy không thành công: {message}", ex.Message);
                throw;
            }
        }
        // HttpGet: api/AuditReceipt/CountAllNumberOfBook
        [HttpGet]
        [Route("CountAllNumberOfBook")]
        public IActionResult CountAllNumberOfBook()
        {
            try
            {
                var result = _auditReceiptRepository.CountAllNumberOfBook();
                _logger.LogInformation("Lấy thành công !");
                return Ok(new
                {
                    quantity = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Lấy không thành công: {message}", ex.Message);
                throw;
            }
        }
        // HttpGet: api/AuditReceipt/PrintListDataDocument
        [HttpGet]
        [Route("PrintListDataDocument")]
        public AuditTraditionalDocument PrintListDataDocument(Guid IdDocumentType, int sortByCondition)
        {
            try
            {
                var result = _auditReceiptRepository.PrintListDataDocument(IdDocumentType, sortByCondition);
                _logger.LogInformation("Lấy thành công !");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Lấy không thành công: {message}", ex.Message);
                throw;
            }
        }
        // HttpGet: api/AuditReceipt/ReportAuditReceipt
        [HttpGet]
        [Route("ReportAuditReceipt")]
        public ReportAuditReceipt ReportAuditReceipt(Guid IdAuditReceipt)
        {
            try
            {
                var result = _auditReceiptRepository.ReportAuditReceipt(IdAuditReceipt);
                _logger.LogInformation("Lấy thành công !");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Lấy không thành công: {message}", ex.Message);
                throw;
            }
        }
        // HttpPost: api/AuditReceipt/ConfirmLostBook
        [HttpPost]
        [Route("ConfirmLostBook")]
        public List<CustomApiAuditReceipt> ConfirmLostBook(int pageNumber, int pageSize, List<Guid> IdIndividual) 
        {
            try
            {
                var result = _auditReceiptRepository.ConfirmLostBook(pageNumber, pageSize, IdIndividual);
                _logger.LogInformation("Lấy thành công !");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Lấy không thành công: {message}", ex.Message);
                throw;
            }
        }
        // HttpPut: api/AuditReceipt/LiquidationAuditReceipt
        [HttpPut]
        [Route("LiquidationAuditReceipt")]
        public IActionResult LiquidationAuditReceipt(List<Guid> IdAuditReceipt)
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

                var result = _auditReceiptRepository.LiquidationAuditReceiptByListId(IdAuditReceipt);
                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogInformation("Thanh lý không thành công: {message}", ex.Message);
                return BadRequest(new
                {
                    Success = false,
                    Fail = true,
                    Message = "Thanh lý không thành công !"
                });
            }
        }
        // GET: api/AuditReceipt/GetUnitAndTypeOfUser
        [HttpGet]
        [Route("GetUnitAndTypeOfUser")]
        public Tuple<string, string> GetUnitAndTypeOfUser(Guid IdUser)
        {
            try
            {
                var result = _auditReceiptRepository.GetUnitAndTypeOfUser(IdUser);
                _logger.LogInformation("Lấy thành công !");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Lấy không thành công: {message}", ex.Message);
                throw;
            }
        }
        // POST: api/AuditReceipt/InsertRedundantDocument
        [HttpPost]
        [Route("InsertRedundantDocument")]
        public IActionResult InsertRedundantDocument([FromForm] DocumentAndIndividualSampleModel documentModel)
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

                var documentDto = _mapper.Map<DocumentDto>(documentModel);

                // define some col with data concrete
                documentDto.ID = Guid.NewGuid();
                documentDto.NumberView = 0;
                documentDto.NumberLike = 0;
                documentDto.NumberUnlike = 0;
                documentDto.ModifiedDate = DateTime.Now;
                documentDto.IsApproved = false;
                documentDto.Status = 0;
                documentDto.CreatedDate = DateTime.Now;
                documentDto.IsDeleted = false;
                documentDto.CreatedBy = checkModel.Id;
                documentDto.IsHavePhysicalVersion = false;

                if (documentModel.Author == "undefined" || documentModel.Author == "null")
                {
                    documentDto.Author = null;
                }
                if (documentModel.Description == "undefined" || documentModel.Description == "null")
                {
                    documentDto.Description = null;
                }

                var documentAvatar = new DocumentAvatarDto
                {
                    // set data document avatar
                    Id = Guid.NewGuid(),
                    Path = _appSettingModel.ServerFileImage + @"\",
                    Status = 0,
                    CreateBy = checkModel.Id,
                    CreateDate = DateTime.Now,
                    FileNameExtention = "jpg",
                    SizeImage = "1",
                    IdDocument = documentDto.ID
                };


                if (Request.Form.Files.Count > 0)
                {
                    foreach (var file in Request.Form.Files)
                    {
                        var fileContentType = file.ContentType;

                        if (fileContentType == "image/jpeg" || fileContentType == "image/png")
                        {
                            documentAvatar.NameFileAvatar = file.FileName;

                            // prepare path to save file image
                            string pathTo = _appSettingModel.ServerFileImage;
                            // get extention form file name
                            string IdFile = documentAvatar.Id.ToString() + ".jpg";

                            // set file path to save file
                            var filename = Path.Combine(pathTo, Path.GetFileName(IdFile));

                            // save file
                            using (var stream = System.IO.File.Create(filename))
                            {
                                file.CopyTo(stream);
                            }
                        }
                        else if (fileContentType == "application/pdf")
                        {
                            documentDto.OriginalFileName = file.FileName;
                            documentDto.FileName = documentDto.ID.ToString() + ".pdf";
                            documentDto.FileNameExtention = "pdf";
                            documentDto.FilePath = _appSettingModel.ServerFilePdf;

                            string pathTo = _appSettingModel.ServerFilePdf;
                            string IdFile = documentDto.ID.ToString() + "." + documentDto.FileNameExtention;

                            var filename = Path.Combine(pathTo, Path.GetFileName(IdFile));

                            using (var stream = System.IO.File.Create(filename))
                            {
                                file.CopyTo(stream);
                            }
                        }
                    }
                }

                Response result = _bookRepository.InsertDocument(documentDto);
                var customApiAuditReceipt = new List<CustomApiAuditReceipt>();

                if (result.Success)
                {
                    //get name document type by id
                    var nameDocumentType = _documentTypeRepository.GetNameDocumentType(documentDto.DocumentTypeId);

                    if(documentModel.Quantity != "null")
                    {
                        //insert individual of book
                        for (int i = 0; i < int.Parse(documentModel.Quantity); i++)
                        {
                            var individualSampleDto = new IndividualSampleDto();
                            string NumberIndividual = "";

                            if (documentModel.SignIndividual == "SGK")
                            {
                                // return false if no record otherwire
                                bool checkExitDocumentAndDocumentType = _individualSampleRepository
                                .CheckExitDocumnentAndDocumentType(documentDto.ID, documentDto.DocumentTypeId);

                                if (!checkExitDocumentAndDocumentType)
                                {
                                    var id = Guid.Parse(documentModel.IdCategory);
                                    string sign = documentModel.SignIndividual;

                                    // generate number Individual
                                    int increase = 1;
                                    NumberIndividual = sign + increase + "/" + documentModel.IdCategory;
                                }
                                else
                                {
                                    var id = Guid.Parse(documentModel.IdCategory);
                                    string sign = documentModel.SignIndividual;
                                    Guid IdDocument = documentDto.ID;
                                    Guid IdDocumentType = documentDto.DocumentTypeId;
                                    int maxNumberIndividual = _individualSampleRepository.getNumIndividualMax(id, IdDocument, IdDocumentType);
                                    // generate number Individual
                                    int increase = maxNumberIndividual + 1;
                                    NumberIndividual = sign + increase + "/" + documentModel.IdCategory;
                                }
                            }
                            else
                            {
                                Guid IdCategorySign = Guid.Parse(documentModel.IdCategory);
                                int maxNumberIndividual = _individualSampleRepository.getNumIndividualMaxByIdCategorySign(IdCategorySign);
                                string sign = documentModel.SignIndividual;

                                // generate number Individual
                                int increase = maxNumberIndividual + 1;
                                NumberIndividual = sign + increase + "/" + documentModel.IdCategory;
                            }

                            // define some col with data concrete
                            individualSampleDto.Id = Guid.NewGuid();
                            individualSampleDto.NumIndividual = NumberIndividual;
                            individualSampleDto.Barcode = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
                            individualSampleDto.IsLostedPhysicalVersion = false;
                            individualSampleDto.IsDeleted = false;
                            individualSampleDto.Status = 1;
                            individualSampleDto.CreatedBy = checkModel.Id;
                            individualSampleDto.CreatedDate = DateTime.Now;
                            individualSampleDto.IdDocument = documentDto.ID;
                            individualSampleDto.StockId = Guid.Parse(documentModel.IdStock);
                            individualSampleDto.IdCategory = Guid.Parse(documentModel.IdCategory);
                            individualSampleDto.DocumentTypeId = documentDto.DocumentTypeId;

                            result = _individualSampleRepository.InsertIndividualSample(individualSampleDto);

                            //prepare data to return
                            var dataReturn = new CustomApiAuditReceipt()
                            {
                                IdBook = documentDto.ID,
                                BookName = documentDto.DocName,
                                PriceBook = documentDto.Price,
                                IdTypeBook = documentDto.DocumentTypeId,
                                TypeBook = nameDocumentType,
                                NumIndividual = individualSampleDto.NumIndividual,
                                IdIndividual = individualSampleDto.Id
                            };

                            customApiAuditReceipt.Add(dataReturn);
                        }
                    }
                    //insert document avartar
                    result = _bookRepository.InsertDocumentAvatar(documentAvatar);

                    return Ok(customApiAuditReceipt);
                }
                else
                {
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/AuditReceipt/GetInformationBookByBarCode
        [HttpGet]
        [Route("GetInformationBookByBarCode")]
        public CustomApiAuditReceipt GetInformationBookByBarCode(string Barcode)
        {
            try
            {
                var result = _auditReceiptRepository.GetDataBookByBarcode(Barcode);
                _logger.LogInformation("Lấy thành công !");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Lấy không thành công: {message}", ex.Message);
                throw;
            }
        }
        // GET: api/AuditReceipt/GetAllAuditReceipt
        [HttpGet]
        [Route("GetAllAuditReceipt")]
        public IEnumerable<AuditReceiptDto> GetAllAuditReceipt(int pageNumber, int pageSize, string reportCreateDate, string reportToDate)
        {
            try
            {
                var result = _auditReceiptRepository.GetAllAuditReceipt(pageNumber, pageSize, reportCreateDate, reportToDate);
                _logger.LogInformation("Lấy danh sách thành công !");
                return result;
            }
            catch(Exception ex)
            {
                _logger.LogInformation("Lấy danh sách không thành công: {message}", ex.Message);
                throw;
            }
        }
        // GET: api/AuditReceipt/GetAuditReceiptById
        [HttpGet]
        [Route("GetAuditReceiptById")]
        public DataOfOneIdAuditReceipt GetAuditReceiptById(Guid IdAuditReceipt)
        {
            try
            {
                var result = _auditReceiptRepository.GetAuditReceiptById(IdAuditReceipt);
                _logger.LogInformation("Lấy thành công !");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Lấy không thành công: {message}", ex.Message);
                throw;
            }
        }
        // HttpPost: api/AuditReceipt/InsertAuditReceipt
        [HttpPost]
        [Route("InsertAuditReceipt")]
        public IActionResult InsertAuditReceipt(AuditReceiptModel auditReceiptModel)
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

                var auditReceiptDto = _mapper.Map<AuditReceiptDto>(auditReceiptModel);
                var result = _auditReceiptRepository.InsertAuditReceipt(auditReceiptDto);

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
        // HttpPut: api/AuditReceipt/UpdateAuditReceipt
        [HttpPut]
        [Route("UpdateAuditReceipt")]
        public IActionResult UpdateAuditReceipt(AuditReceiptModel auditReceiptModel)
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

                var auditReceiptDto = _mapper.Map<AuditReceiptDto>(auditReceiptModel);
                var result = _auditReceiptRepository.UpdateAuditReceipt(auditReceiptDto.Id,auditReceiptDto);

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
        // HttpDelete: api/AuditReceipt/DeleteAuditReceipt
        [HttpDelete]
        [Route("DeleteAuditReceipt")]
        public IActionResult DeleteAuditReceipt(Guid IdAuditReceipt)
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

                var result = _auditReceiptRepository.DeleteAuditReceipt(IdAuditReceipt);
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
        // HttpDelete: api/AuditReceipt/DeleteAuditReceiptByList
        [HttpDelete]
        [Route("DeleteAuditReceiptByList")]
        public IActionResult DeleteAuditReceiptByList(List<Guid> IdAuditReceipt)
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

                var result = _auditReceiptRepository.DeleteAuditReceiptByList(IdAuditReceipt);

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
