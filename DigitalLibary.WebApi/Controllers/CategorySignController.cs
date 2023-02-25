using AutoMapper;
using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Common;
using DigitalLibary.Service.Dto;
using DigitalLibary.Service.Repository.IRepository;
using DigitalLibary.WebApi.Common;
using DigitalLibary.WebApi.Helper;
using DigitalLibary.WebApi.Payload;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DigitalLibary.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategorySignController : Controller
    {

        #region Variables
        private readonly ICategorySignRepository _categorySignRepository;
        private readonly AppSettingModel _appSettingModel;
        private readonly IMapper _mapper;
        private readonly JwtService _jwtService;
        private readonly IUserRepository _userRepository;
        private readonly SaveToDiary _saveToDiary;
        #endregion

        #region Contructor
        public CategorySignController(ICategorySignRepository categorySignRepository,
        IOptionsMonitor<AppSettingModel> optionsMonitor,
        IMapper mapper,
        SaveToDiary saveToDiary,
        JwtService jwtService, IUserRepository userRepository)
        {
            _appSettingModel = optionsMonitor.CurrentValue;
            _mapper = mapper;
            _categorySignRepository = categorySignRepository;
            _jwtService = jwtService;
            _userRepository = userRepository;
            _saveToDiary = saveToDiary;
        }
        #endregion

        #region Method
        // GET: api/CategorySign/CategorySignByDocument
        [HttpGet]
        [Route("CategorySignByDocument")]
        public List<CategorySignModel> CategorySignByDocument(Guid idDocument)
        {
            try
            {
                List<CategorySignDto> categorySignDtos = new List<CategorySignDto>();
                categorySignDtos = _categorySignRepository.CategorySignByDocument(idDocument);

                List<CategorySignModel> categorySignModels = new List<CategorySignModel>();
                categorySignModels = _mapper.Map<List<CategorySignModel>>(categorySignDtos);

                return categorySignModels;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/CategorySign/GetAllCategorySign
        [HttpGet]
        [Route("GetAllCategorySign")]
        public List<CategorySignModel> GetAllCategorySign()
        {
            try
            {
                List<CategorySignDto> categorySignDtos = new List<CategorySignDto>();
                categorySignDtos = _categorySignRepository.getAllCategorySign();

                List<CategorySignModel> categorySignModels = new List<CategorySignModel>();
                categorySignModels = _mapper.Map<List<CategorySignModel>>(categorySignDtos);

                return categorySignModels;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/CategorySign/GetCategorySignWithPage
        [HttpGet]
        [Route("GetCategorySignWithPage")]
        public List<CategorySignModel> GetCategorySignWithPage(int pageNumber, int pageSize)
        {
            try
            {
                List<CategorySignDto> categorySignDtos = new List<CategorySignDto>();
                categorySignDtos = _categorySignRepository.getAllCategorySign(pageNumber, pageSize);

                List<CategorySignModel> categorySignModels = new List<CategorySignModel>();
                categorySignModels = _mapper.Map<List<CategorySignModel>>(categorySignDtos);

                return categorySignModels;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/CategorySign/GetCategorySignById
        [HttpGet]
        [Route("GetCategorySignById")]
        public CategorySignModel GetCategorySignById(Guid Id)
        {
            try
            {
                CategorySignDto categorySignDtos = new CategorySignDto();
                categorySignDtos = _categorySignRepository.getCategorySignById(Id);

                CategorySignModel categorySignModels = new CategorySignModel();
                categorySignModels = _mapper.Map<CategorySignModel>(categorySignDtos);

                return categorySignModels;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/CategorySign/SearchCategorySign
        [HttpGet]
        [Route("SearchCategorySign")]
        public List<CategorySignModel> SearchCategorySign(string code)
        {
            try
            {
                List<CategorySignDto> categorySignDtos = new List<CategorySignDto>();
                categorySignDtos = _categorySignRepository.SearchCategorySign(code);

                List<CategorySignModel> categorySignModels = new List<CategorySignModel>();
                categorySignModels = _mapper.Map<List<CategorySignModel>>(categorySignDtos);
                return categorySignModels;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //CRUD Table CategorySign
        // POST: api/CategorySign/DeleteCategoryByID
        [HttpPost]
        [Route("DeleteCategoryByID")]
        public IActionResult DeleteCategoryByID(Guid Id)
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

                Response result = _categorySignRepository.DeleteCategorySign(Id);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Delete", "CategorySign", true, Id);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Delete", "CategorySign", false, Id);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Delete", "CategorySign", false, Id);
                throw;
            }
        }
        // POST: api/CategorySign/RemoveCategoryById
        [HttpPost]
        [Route("RemoveCategoryById")]
        public IActionResult RemoveCategoryById(Guid Id)
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

                Response result = _categorySignRepository.RemoveCategorySign(Id);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Delete", "CategorySign", true, Id);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Delete", "CategorySign", false, Id);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Delete", "CategorySign", false, Id);
                throw;
            }
        }
        // POST: api/CategorySign/HideCategoryById
        [HttpPost]
        [Route("HideCategoryById")]
        public IActionResult HideCategoryById(Guid Id, bool check)
        {
            Guid IdUserCurrent = Guid.NewGuid();
            string content = check ? "ẩn mã cá biệt" : "bỏ ẩn mã cá biệt";
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

                Response result = _categorySignRepository.HideCategorySign(Id, check);
                if (result.Success)
                {
                    _saveToDiary.ModifyDiary(checkModel.Id, "Update", "CategorySign", true, Id, content);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.ModifyDiary(checkModel.Id, "Update", "CategorySign", false, Id, content);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.ModifyDiary(IdUserCurrent, "Update", "CategorySign", false, Id, content);
                throw;
            }
        }
        // POST: api/CategorySign/InsertCategory
        [HttpPost]
        [Route("InsertCategory")]
        public IActionResult InsertCategory(CategorySignModel categorySignModel)
        {
            Guid IdUserCurrent = Guid.NewGuid();
            Guid IdCategorySign = Guid.NewGuid();
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

                CategorySignDto categorySignDto = new CategorySignDto();
                categorySignDto = _mapper.Map<CategorySignDto>(categorySignModel);

                // define some col with data concrete
                categorySignDto.Id = Guid.NewGuid();
                categorySignDto.Status = 0;
                categorySignDto.CreatedDate = DateTime.Now;
                categorySignDto.CreatedBy = checkModel.Id;
                categorySignDto.IsDeleted = false;
                categorySignDto.IsHided = false;

                IdCategorySign = categorySignDto.Id;

                Response result = _categorySignRepository.InsertCategorySign(categorySignDto);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Create", "CategorySign", true, categorySignDto.Id);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Create", "CategorySign", false, categorySignDto.Id);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Create", "CategorySign", false, IdCategorySign);
                throw;
            }
        }
        // POST: api/CategorySign/UpdateCategorySign
        [HttpPost]
        [Route("UpdateCategorySign")]
        public IActionResult UpdateCategorySign(CategorySignModel categorySignModel)
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

                CategorySignDto categorySignDto = new CategorySignDto();
                categorySignDto = _mapper.Map<CategorySignDto>(categorySignModel);

                Response result = _categorySignRepository.UpdateCategorySign(categorySignDto);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Update", "CategorySign", true, categorySignDto.Id);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Update", "CategorySign", false, categorySignDto.Id);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Update", "CategorySign", false, categorySignModel.Id);
                throw;
            }
        }
        // HttpPost: /api/CategorySign/InsertCategorySignByExcel
        [HttpPost]
        [Route("InsertCategorySignByExcel")]
        public IActionResult InsertCategorySignByExcel()
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

                string IdFile = "";
                // check file exit and save file to sevrer
                if (Request.Form.Files.Count > 0)
                {
                    foreach (var file in Request.Form.Files)
                    {
                        string pathTo = _appSettingModel.ServerFileExcel;
                        IdFile = DateTimeOffset.Now.ToUnixTimeSeconds().ToString() + ".xlsx";

                        // set file path to save file
                        var filename = Path.Combine(pathTo, Path.GetFileName(IdFile));

                        // save file
                        using (var stream = System.IO.File.Create(filename))
                        {
                            file.CopyTo(stream);
                        }
                    }
                }

                // path to open file excel
                var path = string.Concat(_appSettingModel.ServerFileExcel, "\\", IdFile);
                FileInfo fi = new FileInfo(path);
                string fileCodeName = Guid.NewGuid().ToString();

                bool checkAddUser = false;
                List<AddCategorySignByExcel> addCategorySignByExcels = new List<AddCategorySignByExcel>();

                using (ExcelPackage excelPackage = new ExcelPackage(fi))
                {
                    //Get a WorkSheet by name. If the worksheet doesn't exist, throw an exeption
                    ExcelWorksheet namedWorksheet = excelPackage.Workbook.Worksheets[0];

                    int rowCount = namedWorksheet.Dimension.Rows;
                    int colCount = namedWorksheet.Dimension.Columns;
                    for (int i = 2; i <= rowCount; i++)
                    {
                        CategorySignDto categorySignDto = new CategorySignDto();
                        for (int j = 1; j <= colCount; j++)
                        {
                            string cells = "";
                            if (namedWorksheet.Cells[i, j].Value != null)
                            {
                                cells = namedWorksheet.Cells[i, j].Value.ToString();

                                if (j == 1) categorySignDto.SignName = cells;
                                else if (j == 2) categorySignDto.SignCode = cells;
                                else if (j == 3) categorySignDto.IdCategory = new Guid(cells);
                            }
                        }
                         if(categorySignDto.SignName == null || categorySignDto.SignCode == null)
                        {
                            continue;
                        }

                        categorySignDto.Id = Guid.NewGuid();
                        categorySignDto.Status = 0;
                        categorySignDto.CreatedDate = DateTime.Now;
                        categorySignDto.CreatedBy = checkModel.Id;
                        categorySignDto.IsHided = false;
                        categorySignDto.IsDeleted = false;

                        Response response = new Response();
                        response = _categorySignRepository.InsertCategorySign(categorySignDto);


                        if (!response.Success)
                        {
                            checkAddUser = true;
                            AddCategorySignByExcel addCategorySignByExcel = new AddCategorySignByExcel();
                            addCategorySignByExcel.ID = categorySignDto.Id;
                            addCategorySignByExcel.SignName = categorySignDto.SignName;

                            addCategorySignByExcels.Add(addCategorySignByExcel);
                        }
                        else
                        {
                            _saveToDiary.SaveDiary(checkModel.Id, "Create", "CategorySign", true, categorySignDto.Id);
                        }
                    }
                }
                if (checkAddUser)
                {
                    for(int i = 0; i < addCategorySignByExcels.Count;i++)
                    {
                        _saveToDiary.SaveDiary(checkModel.Id, "Create", "CategorySign", false, addCategorySignByExcels[i].ID);
                    }

                    return Ok(new
                    {
                        Success = false,
                        Body = addCategorySignByExcels
                    });
                }
                else
                {
                    return Ok(new
                    {
                        Success = true,
                        Message = "Thêm mới thành công"
                    });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/CategorySign/GetFileExcelCategorySign
        [HttpGet]
        [Route("GetFileExcelCategorySign")]
        public IActionResult GetFileExcelCategorySign()
        {
            try
            {
                var path = string.Concat(_appSettingModel.ServerFileExcel, "\\", "Mau_ImportMaCaBiet.xlsx");
                FileInfo fi = new FileInfo(path);
                string fileCodeName = Guid.NewGuid().ToString();

                using (ExcelPackage excelPackage = new ExcelPackage(fi))
                {
                    //Get a WorkSheet by name. If the worksheet doesn't exist, throw an exeption
                    ExcelWorksheet namedWorksheet = excelPackage.Workbook.Worksheets[1];

                    namedWorksheet.Cells["A2:B200"].Clear();

                    // get all table unit
                    List<Category> units = new List<Category>();
                    units = _categorySignRepository.getAllCategory();
                    int row = 2;
                    for (int i = 0; i < units.Count; i++)
                    {
                        if (units[i].Id == new Guid("cdfd6cde-f21d-46aa-b333-e965d77dff09"))
                        {
                            namedWorksheet.Cells[row, 1].Value = units[i].Id;
                            namedWorksheet.Cells[row, 2].Value = units[i].CategoryName;
                            row++;
                        }
                    }

                    FileInfo fiToSave = new FileInfo(path);
                    //Save your file
                    excelPackage.SaveAs(fiToSave);
                }
                //download file excel
                byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Mau_ImportMaCaBiet.xlsx");
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
