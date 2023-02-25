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
using System.Drawing;

namespace DigitalLibary.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IndividualSampleController : Controller
    {
        #region Variables
        private readonly IIndividualSampleRepository _individualSampleRepository;
        private readonly AppSettingModel _appSettingModel;
        private readonly IMapper _mapper;
        private readonly JwtService _jwtService;
        private readonly IUserRepository _userRepository;
        private readonly IDocumentInvoiceRepository _documentInvoiceRepository;
        private readonly SaveToDiary _saveToDiary;
        #endregion

        #region Contructor
        public IndividualSampleController(IIndividualSampleRepository individualSampleRepository,
        IOptionsMonitor<AppSettingModel> optionsMonitor,
        IMapper mapper,
        SaveToDiary saveToDiary,
        IDocumentInvoiceRepository documentInvoiceRepository,
        JwtService jwtService, IUserRepository userRepository)
        {
            _appSettingModel = optionsMonitor.CurrentValue;
            _mapper = mapper;
            _individualSampleRepository = individualSampleRepository;
            _jwtService = jwtService;
            _userRepository = userRepository;
            _documentInvoiceRepository = documentInvoiceRepository;
            _saveToDiary = saveToDiary;
        }
        #endregion

        #region Method
        // Get: api/IndividualSample/GetBookNameEncrypt
        [HttpGet]
        [Route("GetBookNameEncrypt")]
        public String GetBookNameEncrypt(string bookName)
        {
            try
            {
                return _individualSampleRepository.GetCodeBookNameEncrypt(bookName);
            }
            catch (Exception)
            {
                throw;
            }
        }
        // Get: api/IndividualSample/GetSpineByBarcode
        [HttpGet]
        [Route("GetSpineByBarcode")]
        public CustomApiSpineBook GetSpineByBarcode(string barcode)
        {
            try
            {
                CustomApiSpineBook customApiSpineBook = new CustomApiSpineBook();
                customApiSpineBook = _individualSampleRepository.SpineBookByBarcode(barcode);
                return customApiSpineBook;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // Get: api/IndividualSample/GetSpineByListIdIndividual
        [HttpGet]
        [Route("GetSpineByListIdIndividual")]
        public List<CustomApiSpineBookByGroup> GetSpineByListIdIndividual(String ListIdIndividual)
        {
            try
            {
                var customApiSpineBook = new List<CustomApiSpineBookByGroup>();
                customApiSpineBook = _individualSampleRepository.customApiSpineBook(ListIdIndividual);
                return customApiSpineBook;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // Get: api/IndividualSample/GetSpineByIdIndividual
        [HttpGet]
        [Route("GetSpineByIdIndividual")]
        public List<CustomApiSpineBook> GetSpineByIdIndividual(Guid IdDocument, Guid IdIndividual, String ListIdIndividual)
        {
            try
            {
                List<CustomApiSpineBook> customApiSpineBook = new List<CustomApiSpineBook>();
                customApiSpineBook = _individualSampleRepository.customApiSpineBook(IdDocument, IdIndividual, ListIdIndividual);
                return customApiSpineBook;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // Get: api/IndividualSample/GetIndividualByDateNotRepeat
        [HttpGet]
        [Route("GetIndividualByDateNotRepeat")]
        public HashSet<string> GetIndividualByDateNotRepeat(Guid IdDocument)
        {
            try
            {
                HashSet<string> dateTimes = _individualSampleRepository.GetIndividualByDate(IdDocument);
                return dateTimes;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // Get: api/IndividualSample/GetIndividualStockNotRepeat
        [HttpGet]
        [Route("GetIndividualStockNotRepeat")]
        public CustomApiIndividual GetIndividualStockNotRepeat(Guid IdDocument)
        {
            try
            {
                CustomApiIndividual customApiIndividual = new CustomApiIndividual();
                customApiIndividual = _individualSampleRepository.CustomApiIndividual(IdDocument);
                return customApiIndividual;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // Get: api/IndividualSample/GetIndividualSampleLostById
        [HttpGet]
        [Route("GetIndividualSampleLostById")]
        public IndividualSampleModel GetIndividualSampleLostById(Guid Id)
        {
            try
            {
                IndividualSampleDto individualSampleDtos = new IndividualSampleDto();
                individualSampleDtos = _individualSampleRepository.getIndividualSampleLostById(Id);

                IndividualSampleModel individualSampleModels = new IndividualSampleModel();
                individualSampleModels = _mapper.Map<IndividualSampleModel>(individualSampleDtos);
                return individualSampleModels;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // Get: api/IndividualSample/GetAllIndividualSampleIsLost
        [HttpGet]
        [Route("GetAllIndividualSampleIsLost")]
        public List<IndividualSampleModel> GetAllIndividualSampleIsLost(int pageNumber, int pageSize)
        {
            try
            {
                List<IndividualSampleDto> individualSampleDtos = new List<IndividualSampleDto>();
                individualSampleDtos = _individualSampleRepository.getIndividualSampleIsLost(pageNumber, pageSize);

                List<DocumentInvoiceDto> documentInvoiceDtos = _documentInvoiceRepository.GetDocumentInvoiceByStatus(3);
                if (documentInvoiceDtos != null)
                {
                    for (int i = 0; i < documentInvoiceDtos.Count; i++)
                    {
                        List<DocumentInvoiceDetailDto> documentInvoiceDetailDtos = _documentInvoiceRepository
                        .GetDocumentInvoiceDetailByIdDocumentIncoice(documentInvoiceDtos[i].Id);

                        for (int j = 0; j < documentInvoiceDetailDtos.Count; j++)
                        {
                            IndividualSampleDto individualSampleDto = _individualSampleRepository
                            .getIndividualSampleById(documentInvoiceDetailDtos[j].IdIndividual);

                            if (individualSampleDto != null)
                            {
                                if (individualSampleDto.Status == 1 && individualSampleDto.IsLostedPhysicalVersion == false)
                                {
                                    _documentInvoiceRepository.ChangeStatusDocumentInvoice(documentInvoiceDtos[i].Id, 1);
                                }
                            }
                        }
                    }
                }

                List<IndividualSampleModel> individualSampleModels = new List<IndividualSampleModel>();
                individualSampleModels = _mapper.Map<List<IndividualSampleModel>>(individualSampleDtos);
                return individualSampleModels;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // POST: api/IndividualSample/ChangeLostPhysicalVersion
        [HttpPost]
        [Route("ChangeLostPhysicalVersion")]
        public IActionResult ChangeLostPhysicalVersion(Guid IdDocument, Guid IdIndividual, bool isLost)
        {
            Guid IdUserCurrent = Guid.NewGuid();
            string content = isLost ? "báo mất bản cứng của sách" : "hồi cố sách";
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
                Response result = _individualSampleRepository.ReturnBook(IdDocument, IdIndividual, isLost);
                if (result.Success)
                {
                    _saveToDiary.ModifyDiary(checkModel.Id, "Update", "Individual", true, IdIndividual, content);
                    _saveToDiary.ModifyDiary(checkModel.Id, "Update", "Document", true, IdDocument, content);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.ModifyDiary(checkModel.Id, "Update", "Individual", false, IdIndividual, content);
                    _saveToDiary.ModifyDiary(checkModel.Id, "Update", "Document", false, IdDocument, content);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.ModifyDiary(IdUserCurrent, "Update", "Individual", false, IdIndividual, content);
                _saveToDiary.ModifyDiary(IdUserCurrent, "Update", "Document", false, IdDocument, content);
                throw;
            }
        }
        // POST: api/IndividualSample/InsertIndividualSample
        [HttpPost]
        [Route("InsertIndividualSample")]
        public IActionResult InsertIndividualSample(List<IndividualSampleModel> insertIndividualSampleModel)
        {
            Guid IdUserCurrent = Guid.NewGuid();
            Guid IdIndividual = Guid.NewGuid();
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

                Response result = null;
                for (int i = 0; i < insertIndividualSampleModel.Count; i++)
                {
                    IndividualSampleDto individualSampleDto = new IndividualSampleDto();
                    individualSampleDto = _mapper.Map<IndividualSampleDto>(insertIndividualSampleModel[i]);
                    string NumberIndividual = "";

                    if (insertIndividualSampleModel[i].SignIndividual == "SGK")
                    {
                        // return false if no record otherwire
                        bool checkExitDocumentAndDocumentType = _individualSampleRepository
                        .CheckExitDocumnentAndDocumentType(insertIndividualSampleModel[i].IdDocument, (Guid)insertIndividualSampleModel[i].DocumentTypeId);

                        if (!checkExitDocumentAndDocumentType)
                        {
                            Guid id = insertIndividualSampleModel[i].IdCategory;
                            string sign = insertIndividualSampleModel[i].SignIndividual;

                            // generate number Individual
                            int increase = 1;
                            NumberIndividual = sign + increase + "/" + individualSampleDto.IdCategory;
                        }
                        else
                        {
                            Guid id = insertIndividualSampleModel[i].IdCategory;
                            string sign = insertIndividualSampleModel[i].SignIndividual;
                            Guid IdDocument = insertIndividualSampleModel[i].IdDocument;
                            Guid IdDocumentType = (Guid)insertIndividualSampleModel[i].DocumentTypeId;
                            int maxNumberIndividual = _individualSampleRepository.getNumIndividualMax(id, IdDocument, IdDocumentType);
                            // generate number Individual
                            int increase = maxNumberIndividual + 1;
                            NumberIndividual = sign + increase + "/" + individualSampleDto.IdCategory;
                        }
                    }
                    else
                    {
                        Guid IdCategorySign = insertIndividualSampleModel[i].IdCategory;
                        int maxNumberIndividual = _individualSampleRepository.getNumIndividualMaxByIdCategorySign(IdCategorySign);
                        string sign = insertIndividualSampleModel[i].SignIndividual;

                        // generate number Individual
                        int increase = maxNumberIndividual + 1;
                        NumberIndividual = sign + increase + "/" + individualSampleDto.IdCategory;
                    }


                    Random generator = new Random();
                    string Barcode = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();

                    // define some col with data concrete
                    individualSampleDto.Id = Guid.NewGuid();
                    individualSampleDto.NumIndividual = NumberIndividual;
                    individualSampleDto.Barcode = Barcode;
                    individualSampleDto.IsLostedPhysicalVersion = false;
                    individualSampleDto.IsDeleted = false;
                    individualSampleDto.Status = 1;
                    individualSampleDto.CreatedBy = checkModel.Id;
                    individualSampleDto.CreatedDate = DateTime.Now;

                    IdIndividual = individualSampleDto.Id;

                    result = _individualSampleRepository.InsertIndividualSample(individualSampleDto);

                    if (result.Success)
                    {
                        _saveToDiary.SaveDiary(checkModel.Id, "Create", "Individual", true, IdIndividual);
                    }
                }
                if (result.Success)
                {
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Create", "Individual", false, IdIndividual);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Create", "Individual", false, IdIndividual);
                throw;
            }
        }
        // Get: api/IndividualSample/GetAllIndividualSample
        [HttpGet]
        [Route("GetAllIndividualSample")]
        public List<IndividualSampleModel> GetAllIndividualSample(int pageNumber, int pageSize)
        {
            try
            {
                List<IndividualSampleDto> individualSampleDtos = new List<IndividualSampleDto>();
                individualSampleDtos = _individualSampleRepository.getIndividualSample(pageNumber, pageSize);

                List<IndividualSampleModel> individualSampleModels = new List<IndividualSampleModel>();
                individualSampleModels = _mapper.Map<List<IndividualSampleModel>>(individualSampleDtos);
                return individualSampleModels;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // Get: api/IndividualSample/GetIndividualSampleById
        [HttpGet]
        [Route("GetIndividualSampleById")]
        public IndividualSampleModel GetIndividualSampleById(Guid Id)
        {
            try
            {

                IndividualSampleDto individualSampleDtos = new IndividualSampleDto();
                individualSampleDtos = _individualSampleRepository.getIndividualSampleById(Id);

                IndividualSampleModel individualSampleModels = new IndividualSampleModel();
                individualSampleModels = _mapper.Map<IndividualSampleModel>(individualSampleDtos);
                return individualSampleModels;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // POST: api/IndividualSample/DeleteIndividualSample
        [HttpPost]
        [Route("DeleteIndividualSample")]
        public IActionResult DeleteIndividualSample(Guid Id)
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


                Response result = _individualSampleRepository.DeleteIndividualSample(Id);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Delete", "Individual", true, Id);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Delete", "Individual", false, Id);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Delete", "Individual", false, Id);
                throw;
            }
        }
        // POST: api/IndividualSample/UpdateIndividualSample
        [HttpPost]
        [Route("UpdateIndividualSample")]
        public IActionResult UpdateIndividualSample(IndividualSampleModel individualSampleModel)
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

                //check categorysign with signcode skg
                string numIndividual = individualSampleModel.SignIndividual + "/" + individualSampleModel.IdCategory;
                if (individualSampleModel.SignIndividual.Substring(0, 3) == "SGK")
                {
                    IndividualSampleDto individualSampleDtos = new IndividualSampleDto();
                    individualSampleDtos = _individualSampleRepository.getIndividualSampleById(individualSampleModel.Id);
                    if (numIndividual != individualSampleDtos.NumIndividual)
                    {
                        // return true if not found resuilt otherwire
                        bool resultCheck = _individualSampleRepository
                        .CheckExitNumberIndividual(numIndividual, individualSampleModel.IdDocument, (Guid)individualSampleModel.DocumentTypeId);
                        if (!resultCheck)
                        {
                            return BadRequest(new
                            {
                                message = "Mã cá biệt đã tồn tại, hãy chọn mã cá biệt khác",
                            });
                        }
                    }
                }
                else
                {
                    //return true if not found resuilt otherwire
                    bool resultCheck = _individualSampleRepository
                    .CheckExitNumberIndividualByNumIndividual(numIndividual, individualSampleModel.Id);
                    if (!resultCheck)
                    {
                        return BadRequest(new
                        {
                            message = "Mã cá biệt đã tồn tại, hãy chọn mã cá biệt khác",
                        });
                    }
                }


                IndividualSampleDto individualSampleDto = new IndividualSampleDto();
                individualSampleDto = _mapper.Map<IndividualSampleDto>(individualSampleModel);
                //check signCategory if not exits number
                bool checkNumber = false;
                for (int i = 0; i < individualSampleModel.SignIndividual.Length; i++)
                {
                    if (individualSampleModel.SignIndividual[i] >= '0' && individualSampleModel.SignIndividual[i] <= '9')
                    {
                        checkNumber = true;
                    }
                }
                if (!checkNumber)
                {
                    Guid id = individualSampleDto.IdCategory;
                    string sign = individualSampleDto.SignIndividual;
                    int maxNumberIndividual = _individualSampleRepository
                    .getNumIndividualMax(id, individualSampleModel.IdDocument, (Guid)individualSampleModel.DocumentTypeId);
                    maxNumberIndividual += 1;

                    string numIndividualIncrease = individualSampleModel.SignIndividual + maxNumberIndividual + "/" + individualSampleModel.IdCategory;
                    individualSampleDto.NumIndividual = numIndividualIncrease;
                }
                else
                {
                    individualSampleDto.NumIndividual = numIndividual;
                }

                Response result = _individualSampleRepository.UpdateIndividualSample(individualSampleDto);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Update", "Individual", true, individualSampleModel.Id);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Update", "Individual", false, individualSampleModel.Id);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Update", "Individual", false, individualSampleModel.Id);
                throw;
            }
        }
        // POST: api/IndividualSample/DeleteIndividualSampleByList
        [HttpPost]
        [Route("DeleteIndividualSampleByList")]
        public IActionResult DeleteIndividualSampleByList(List<Guid> IdIndividualSample)
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

                var result = _individualSampleRepository.DeleteIndividualSampleByList(IdIndividualSample);
                if (result.Success)
                {
                    return Ok(new
                    {
                        message = result.Message,
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
        // POST: api/IndividualSample/CheckIdIndividualExitsInDocumentInvoice
        [HttpPost]
        [Route("CheckIdIndividualExitsInDocumentInvoice")]
        public IActionResult CheckIdIndividualExitsInDocumentInvoice(List<Guid> IdIndividualSample)
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

                var result = _individualSampleRepository.CheckIdIndividualExitsInDocumentInvoice(IdIndividualSample);
                if (result.Item1)
                {
                    return Ok(new
                    {
                        Message = "Mã cá biệt này đã tồn tại trong phiếu mượn. Bạn có chắc chắn muốn xóa !",
                        Success = true,
                        Fail = false,
                        NumIndividualExist = result.Item2
                    });
                }
                else
                {
                    return Ok(new
                    {
                        Message = "Mã cá biệt này có thể xóa !",
                        Success = false,
                        Fail = true,
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
