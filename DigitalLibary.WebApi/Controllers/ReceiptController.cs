using AutoMapper;
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
using System.Threading.Tasks;

namespace DigitalLibary.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReceiptController : Controller
    {
        #region Variables
        private readonly IReceiptRepository _receiptRepository;
        private readonly AppSettingModel _appSettingModel;
        private readonly IMapper _mapper;
        private readonly JwtService _jwtService;
        private readonly IUserRepository _userRepository;
        private readonly IIndividualSampleRepository _individualSampleRepository;
        private readonly ICategorySignRepository _categorySignRepository;
        private readonly IBookRepository _bookRepository;
        private readonly SaveToDiary _saveToDiary;
        private readonly IParticipantsRepository _participantsRepository;
        #endregion

        #region Contructor
        public ReceiptController(IReceiptRepository receiptRepository,
        IOptionsMonitor<AppSettingModel> optionsMonitor,
        IMapper mapper,
        ICategorySignRepository categorySignRepository,
        IBookRepository bookRepository,
        SaveToDiary saveToDiary,
        IIndividualSampleRepository individualSampleRepository,
        IParticipantsRepository participantsRepository,
        JwtService jwtService, IUserRepository userRepository)
        {
            _appSettingModel = optionsMonitor.CurrentValue;
            _mapper = mapper;
            _receiptRepository = receiptRepository;
            _jwtService = jwtService;
            _userRepository = userRepository;
            _individualSampleRepository = individualSampleRepository;
            _categorySignRepository = categorySignRepository;
            _saveToDiary = saveToDiary;
            _participantsRepository = participantsRepository;
            _bookRepository = bookRepository;
        }
        #endregion

        #region Method
        // GET: api/Receipt/GetListBookStatus
        [HttpGet]
        [Route("GetListBookStatus")]
        public IActionResult GetListBookStatus()
        {
            try
            {
                List<string> Original = new List<string>();
                Original = _receiptRepository.GetlistBookStatus();

                return Ok(Original);
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/Receipt/GetListOriginal
        [HttpGet]
        [Route("GetListOriginal")]
        public IActionResult GetListOriginal()
        {
            try
            {
                List<string> Original = new List<string>();
                Original = _receiptRepository.GetlistOriginal();

                return Ok(Original);
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/Receipt/GetListReceipt
        [HttpPost]
        [Route("GetListReceipt")]
        public List<ReceiptModel> GetListReceipt(SortReceiptAndSearch sortReceiptAndSearch)
        {
            try
            {
                List<ReceiptDto> receiptDtos = new List<ReceiptDto>();
                receiptDtos = _receiptRepository.getAllReceipt(sortReceiptAndSearch);

                List<ReceiptModel> receiptModels = new List<ReceiptModel>();
                receiptModels = _mapper.Map<List<ReceiptModel>>(receiptDtos);

                return receiptModels;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/Receipt/GetAllReceipt
        [HttpGet]
        [Route("GetAllReceipt")]
        public List<ReceiptModel> GetAllReceipt(int pageNumber, int pageSize)
        {
            try
            {
                List<ReceiptDto> receiptDtos = new List<ReceiptDto>();
                receiptDtos = _receiptRepository.getAllReceipt(pageNumber, pageSize);

                List<ReceiptModel> receiptModels = new List<ReceiptModel>();
                receiptModels = _mapper.Map<List<ReceiptModel>>(receiptDtos);

                return receiptModels;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/Receipt/GetReceiptById
        [HttpGet]
        [Route("GetReceiptById")]
        public ReceiptModel GetReceiptById(Guid Id)
        {
            try
            {
                ReceiptDto receiptDto = new ReceiptDto();
                receiptDto = _receiptRepository.getReceipt(Id);

                ReceiptModel receipt = new ReceiptModel();
                receipt = _mapper.Map<ReceiptModel>(receiptDto);

                return receipt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        //CRUD Table CategorySign
        // POST: api/Receipt/DeleteReceiptById
        [HttpPost]
        [Route("DeleteReceiptById")]
        public IActionResult DeleteReceiptById(Guid Id)
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



                Response result = _receiptRepository.DeleteReceipt(Id);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Delete", "Receipt", true, Id);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Delete", "Receipt", false, Id);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Delete", "Receipt", false, Id);
                throw;
            }
        }
        // POST: api/Receipt/InsertReceipt
        [HttpPost]
        [Route("InsertReceipt")]
        public IActionResult InsertReceipt(ReceiptModel receiptModel)
        {
            Guid IdUserCurrent = Guid.NewGuid();
            Guid IdReceipt = Guid.NewGuid();
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

                ReceiptDto receiptDto = new ReceiptDto();
                receiptDto = _mapper.Map<ReceiptDto>(receiptModel);

                int ReceiptCodeMax = _receiptRepository.GetMaxReceiptCode("PN");
                ReceiptCodeMax += 1;

                string Receipt = "PN" + ReceiptCodeMax;

                // define some col with data concrete
                receiptDto.IdReceipt = Guid.NewGuid();
                receiptDto.Status = 0;
                receiptDto.IsDeleted = false;
                receiptDto.ReceiptCode = Receipt;

                IdReceipt = receiptDto.IdReceipt;

                Response result = _receiptRepository.InsertReceipt(receiptDto);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Create", "Receipt", true, IdReceipt);

                    for (int i = 0; i < receiptDto.participants.Count; i++)
                    {
                        ParticipantsDto participantsDto = new ParticipantsDto()
                        {
                            Id = Guid.NewGuid(),
                            IdReceipt = receiptDto.IdReceipt,
                            Name = receiptDto.participants[i].Name,
                            Position = receiptDto.participants[i].Position,
                            Mission = receiptDto.participants[i].Mission,
                            Note = receiptDto.participants[i].Note,
                            CreatedDate = DateTime.Now,
                            Status = 0
                        };

                        _participantsRepository.InsertParticipants(participantsDto);
                    }

                    for (int i = 0; i < receiptDto.DocumentListId.Count; i++)
                    {
                        for (int j = 0; j < receiptDto.DocumentListId[i].Quantity; j++)
                        {
                            CategorySignDto categorySignDto = _categorySignRepository.getCategorySignById(receiptDto.DocumentListId[i].IdCategory);

                            Guid id = categorySignDto.Id;
                            string sign = categorySignDto.SignCode;
                            //get document type id from table book
                            DocumentDto book = _receiptRepository.GetDocumentType(receiptDto.DocumentListId[i].idDocument);

                            int maxNumberIndividual = 0;

                            if(categorySignDto.SignCode == "SGK")
                            {
                                maxNumberIndividual = _individualSampleRepository
                                .getNumIndividualMax(id, receiptDto.DocumentListId[i].idDocument, book.DocumentTypeId);
                            }
                            else
                            {
                                maxNumberIndividual = _individualSampleRepository
                                .getNumIndividualMaxByInsertReceipt(id);
                            }

                            maxNumberIndividual += 1;
                            string NumberIndividual = sign + maxNumberIndividual + "/" + id;

                            string Barcode = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();       

                            IndividualSampleDto individualSampleDto = new IndividualSampleDto()
                            {
                                Id = Guid.NewGuid(),
                                IdDocument = receiptDto.DocumentListId[i].idDocument,
                                StockId = receiptDto.DocumentListId[i].IdStock,
                                NumIndividual = NumberIndividual,
                                Barcode = Barcode,
                                IsLostedPhysicalVersion = false,
                                IsDeleted = false,
                                Status = 1,
                                CreatedBy = checkModel.Id,
                                CreatedDate = DateTime.Now,
                                DocumentTypeId = book.DocumentTypeId
                            };

                            Response rs = _individualSampleRepository.InsertIndividualSample(individualSampleDto);

                            if(rs.Success)
                            {
                                _saveToDiary.SaveDiary(checkModel.Id, "Create", "Individual", true, individualSampleDto.Id);
                            }
                            else
                            {
                                _saveToDiary.SaveDiary(checkModel.Id, "Create", "Individual", false, individualSampleDto.Id);
                            }
                        }
                    }
                    return Ok(new
                    {
                        IdReceipt = receiptDto.IdReceipt
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(IdUserCurrent, "Create", "Receipt", false, IdReceipt);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Create", "Receipt", false, IdReceipt);
                throw;
            }
        }
        // GET: api/Receipt/SearchReceipt
        [HttpGet]
        [Route("SearchReceipt")]
        public ReceiptModel SearchReceipt(string code)
        {
            try
            {
                ReceiptDto receiptDto = new ReceiptDto();
                receiptDto = _receiptRepository.SearchReceipt(code);

                ReceiptModel receiptModel = new ReceiptModel();
                receiptModel = _mapper.Map<ReceiptModel>(receiptDto);
                return receiptModel;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // POST: api/Receipt/UpdateReceipt
        [HttpPost]
        [Route("UpdateReceipt")]
        public async Task<IActionResult> UpdateReceipt(ReceiptModel receiptModel)
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

                ReceiptDto receiptDto = new ReceiptDto();
                receiptDto = _mapper.Map<ReceiptDto>(receiptModel);

                Response result = await _receiptRepository.UpdateReceipt(receiptDto);
                if (result.Success)
                {
                    //insert individual
                    for (int i = 0; i < receiptDto.DocumentListId.Count; i++)
                    {
                        for (int j = 0; j < receiptDto.DocumentListId[i].Quantity; j++)
                        {
                            CategorySignDto categorySignDto = _categorySignRepository.getCategorySignById(receiptDto.DocumentListId[i].IdCategory);

                            Guid id = categorySignDto.Id;
                            string sign = categorySignDto.SignCode;
                            //get document type id from table book
                            DocumentDto book = _receiptRepository.GetDocumentType(receiptDto.DocumentListId[i].idDocument);

                            int maxNumberIndividual = 0;

                            if (categorySignDto.SignCode == "SGK")
                            {
                                maxNumberIndividual = _individualSampleRepository
                                .getNumIndividualMax(id, receiptDto.DocumentListId[i].idDocument, book.DocumentTypeId);
                            }
                            else
                            {
                                maxNumberIndividual = _individualSampleRepository
                                .getNumIndividualMaxByInsertReceipt(id);
                            }

                            maxNumberIndividual += 1;
                            string NumberIndividual = sign + maxNumberIndividual + "/" + id;

                            string Barcode = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();

                            IndividualSampleDto individualSampleDto = new IndividualSampleDto()
                            {
                                Id = Guid.NewGuid(),
                                IdDocument = receiptDto.DocumentListId[i].idDocument,
                                StockId = receiptDto.DocumentListId[i].IdStock,
                                NumIndividual = NumberIndividual,
                                Barcode = Barcode,
                                IsLostedPhysicalVersion = false,
                                IsDeleted = false,
                                Status = 1,
                                CreatedBy = checkModel.Id,
                                CreatedDate = DateTime.Now,
                                DocumentTypeId = book.DocumentTypeId
                            };

                            Response rs = _individualSampleRepository.InsertIndividualSample(individualSampleDto);
                        }
                    }
                    return Ok(new
                    {
                        Success = result.Success,
                        message = result.Message
                    });
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
        #endregion
    }
}
