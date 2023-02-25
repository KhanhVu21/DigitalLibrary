
using AutoMapper;
using DigitalLibary.Service.Common;
using DigitalLibary.Service.Common.FormatApi;
using DigitalLibary.Service.Dto;
using DigitalLibary.Service.Repository.IRepository;
using DigitalLibary.WebApi.Common;
using DigitalLibary.WebApi.Helper;
using DigitalLibary.WebApi.Payload;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DigitalLibary.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentInvoiceController : Controller
    {
        #region Variables
        private readonly IDocumentInvoiceRepository _documentInvoiceRepository;
        private readonly AppSettingModel _appSettingModel;
        private readonly IMapper _mapper;
        private readonly JwtService _jwtService;
        private readonly IUserRepository _userRepository;
        private readonly IIndividualSampleRepository _individualSampleRepository;
        private readonly IBookRepository _bookRepository;
        private readonly SaveToDiary _saveToDiary;
        #endregion

        #region Contructor
        public DocumentInvoiceController(IDocumentInvoiceRepository documentInvoiceRepository,
        IOptionsMonitor<AppSettingModel> optionsMonitor,
        IMapper mapper,
        SaveToDiary saveToDiary,
        IBookRepository bookRepository,
        IIndividualSampleRepository individualSampleRepository,
        JwtService jwtService, IUserRepository userRepository)
        {
            _appSettingModel = optionsMonitor.CurrentValue;
            _mapper = mapper;
            _documentInvoiceRepository = documentInvoiceRepository;
            _jwtService = jwtService;
            _userRepository = userRepository;
            _individualSampleRepository = individualSampleRepository;
            _bookRepository = bookRepository;
            _saveToDiary = saveToDiary;
        }
        #endregion

        #region MeTHod
        // GET: api/DocumentInvoice/GetListBorrowLate
        [HttpGet]
        [Route("GetListBorrowLate")]
        public List<DocumentInvoiceModel> GetListBorrowLate(string fromDate, string toDate)
        {
            try
            {
                List<DocumentInvoiceDto> documentInvoiceDtos = new List<DocumentInvoiceDto>();
                documentInvoiceDtos = _documentInvoiceRepository.getListBorrowLate(fromDate, toDate);
                 
                List<DocumentInvoiceModel> documentInvoiceModels = new List<DocumentInvoiceModel>();
                documentInvoiceModels = _mapper.Map<List<DocumentInvoiceModel>>(documentInvoiceDtos);
                return documentInvoiceModels;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // POST: api/DocumentInvoice/InsertDocumentInvoice
        [HttpPost]
        [Route("InsertDocumentInvoice")]
        public IActionResult InsertDocumentInvoice(DocumentInvoiceModel documentInvoiceModel)
        {
            Guid IdUserCurrent = Guid.NewGuid();
            Guid IdUsertDocumentInvoice = Guid.NewGuid();
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

                var duplicates = documentInvoiceModel.DocumentAndIndividual
                    .SelectMany(x => x.idIndividual)
                    .GroupBy(x => x)
                    .Where(x => x.Count() > 1)
                    .Select(x => x.Key);

                if (duplicates.Any())
                {
                    return BadRequest(new
                    {
                        message = "Đã có mã cá biệt bị trùng !"
                    });
                }


                if (checkModel != null) IdUserCurrent = checkModel.Id;

                DocumentInvoiceDto documentInvoiceDto = new DocumentInvoiceDto();
                documentInvoiceDto = _mapper.Map<DocumentInvoiceDto>(documentInvoiceModel);

                Response result = null;
                string InvoiceCode = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();

                DateTime dtDateIn = Convert.ToDateTime(documentInvoiceModel.DateIn);
                DateTime dtDateOut = Convert.ToDateTime(documentInvoiceModel.DateOut);

                // define some col with data concrete
                documentInvoiceDto.Id = Guid.NewGuid();
                documentInvoiceDto.Status = 0;
                documentInvoiceDto.CreateBy = checkModel.Id;
                documentInvoiceDto.CreateDate = DateTime.Now;
                documentInvoiceDto.DateInReality = null;
                documentInvoiceDto.InvoiceCode = InvoiceCode;
                documentInvoiceDto.DateIn = dtDateIn;
                documentInvoiceDto.DateOut = dtDateOut;
                documentInvoiceDto.IsCompleted = false;

                IdUsertDocumentInvoice = documentInvoiceDto.Id;


                result = _documentInvoiceRepository.InsertDocumentInvoice(documentInvoiceDto);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Create", "DocumentInvoice", true, IdUsertDocumentInvoice);
                    return Ok(new
                    {
                        message = result.Message,
                        Id = documentInvoiceDto.Id,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Create", "DocumentInvoice", false, IdUsertDocumentInvoice);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Create", "DocumentInvoice", false, IdUsertDocumentInvoice);
                throw;
            }
        }
        // POST: api/DocumentInvoice/ChangeStatusDocumentInvoice
        [HttpPost]
        [Route("ChangeStatusDocumentInvoice")]
        public IActionResult ChangeStatusDocumentInvoice(Guid Id, int status)
        {
            Guid IdUserCurrent = Guid.NewGuid();
            string content = status == 1 ? "trả cuốn sách" :
            status == 3 ? "báo mất sách" : "";
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


                Response result = _documentInvoiceRepository.ChangeStatusDocumentInvoice(Id, status);
                if (result.Success)
                {
                    _saveToDiary.ModifyDiary(checkModel.Id, "Update", "DocumentInvoice", true, Id, content);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.ModifyDiary(checkModel.Id, "Update", "DocumentInvoice", false, Id, content);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.ModifyDiary(IdUserCurrent, "Update", "DocumentInvoice", false, Id, content);
                throw;
            }
        }
        // GET: api/DocumentInvoice/GetDocumentInvoice
        [HttpGet]
        [Route("GetDocumentInvoice")]
        public List<DocumentInvoiceModel> GetDocumentInvoice(int pageNumber, int pageSize)
        {
            try
            {
                List<DocumentInvoiceDto> documentInvoiceDtos = new List<DocumentInvoiceDto>();
                documentInvoiceDtos = _documentInvoiceRepository.GetDocumentInvoice(pageNumber, pageSize);

                if(documentInvoiceDtos != null)
                {
                    for(int i = 0; i < documentInvoiceDtos.Count; i++)
                    {
                        if (documentInvoiceDtos[i].Status == 1 
                        && documentInvoiceDtos[i].DateInReality > documentInvoiceDtos[i].DateIn)
                        {
                            _documentInvoiceRepository.ChangeStatusDocumentInvoice(documentInvoiceDtos[i].Id, 2);                  
                        }
                        if (documentInvoiceDtos[i].IsCompleted == false)
                        {
                            if (documentInvoiceDtos[i].Status == 4
                            || documentInvoiceDtos[i].Status == 2)
                            {
                                _individualSampleRepository.ChangeIndividualToAvailable(documentInvoiceDtos[i].Id);
                                _documentInvoiceRepository.ChangeStatusCompleteInvoice(documentInvoiceDtos[i].Id);
                            }
                        }
                    }
                }

                List<DocumentInvoiceModel> documentInvoiceModels = new List<DocumentInvoiceModel>();
                documentInvoiceModels = _mapper.Map<List<DocumentInvoiceModel>>(documentInvoiceDtos);

                return documentInvoiceModels;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/DocumentInvoice/GetDocumentInvoiceById
        [HttpGet]
        [Route("GetDocumentInvoiceById")]
        public DocumentInvoiceModel GetDocumentInvoiceById(Guid Id)
        {
            try
            {
                DocumentInvoiceDto documentInvoiceDto = new DocumentInvoiceDto();
                documentInvoiceDto = _documentInvoiceRepository.GetDocumentInvoiceById(Id);

                DocumentInvoiceModel documentInvoiceModel = new DocumentInvoiceModel();
                documentInvoiceModel = _mapper.Map<DocumentInvoiceModel>(documentInvoiceDto);

                return documentInvoiceModel;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // POST: api/DocumentInvoice/UpdateDocumentInvoice
        [HttpPost]
        [Route("UpdateDocumentInvoice")]
        public IActionResult UpdateDocumentInvoice(DocumentInvoiceModel documentInvoiceModel)
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


                DocumentInvoiceDto documentInvoiceDto = new DocumentInvoiceDto();
                documentInvoiceDto = _mapper.Map<DocumentInvoiceDto>(documentInvoiceModel);
                if(documentInvoiceDto.Status != 3)
                {
                    documentInvoiceDto.DateInReality = DateTime.Now;
                }

                Response result = _documentInvoiceRepository.UpdateDocumentInvoice(documentInvoiceDto);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Update", "DocumentInvoice", true, documentInvoiceModel.Id);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Update", "DocumentInvoice", false, documentInvoiceModel.Id);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Update", "DocumentInvoice", true, documentInvoiceModel.Id);
                throw;
            }
        }
        // GET: api/DocumentInvoice/SearchDocumentInvoice
        [HttpGet]
        [Route("SearchDocumentInvoice")]
        public List<CustomApiSearchDocumentInvoice> SearchDocumentInvoice(string name)
        {
            try
            {
                List<CustomApiSearchDocumentInvoice> documentInvoices = new List<CustomApiSearchDocumentInvoice>();
                documentInvoices = _documentInvoiceRepository.ListDocumentInvoice(name);

                return documentInvoices;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
