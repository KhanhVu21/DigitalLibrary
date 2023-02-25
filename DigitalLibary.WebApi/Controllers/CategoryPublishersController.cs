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
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalLibary.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryPublishersController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly JwtService _jwtService;
        private readonly IUserRepository _userRepository;
        private readonly AppSettingModel _appSettingModel;
        private readonly ICategorySignRepository _categorySignRepository;
        private readonly ICategoryPublisherRepository _categoryPublisherRepository;
        private readonly SaveToDiary _saveToDiary;

        public CategoryPublishersController(DataContext context,
            JwtService jwtService, IUserRepository userRepository,
                    IOptionsMonitor<AppSettingModel> optionsMonitor,
                    SaveToDiary saveToDiary,
                    ICategorySignRepository categorySignRepository,
            ICategoryPublisherRepository categoryPublisherRepository)
        {
            _context = context;
            _appSettingModel = optionsMonitor.CurrentValue;
            _categoryPublisherRepository = categoryPublisherRepository;
            _jwtService = jwtService;
            _userRepository = userRepository;
            _categorySignRepository = categorySignRepository;
            _saveToDiary = saveToDiary;
        }


        // GET: api/CategoryPublishers/SearchByName
        [HttpGet("SearchByName")]
        public List<CategoryPublisher> SearchByName(String SearchString, int pageNumber, int pageSize)
        {
            return _categoryPublisherRepository.SearchByName(SearchString, pageNumber, pageSize);
        }

        // GET: api/CategoryPublishers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryPublisher>>> GetCategoryPublisher(int pageNumber, int pageSize)
        {
            return await _context.CategoryPublisher.OrderByDescending(e =>e.CreatedDate).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        // GET: api/CategoryPublishers
        [HttpGet]
        [Route("NotPagination")]
        public async Task<ActionResult<IEnumerable<CategoryPublisher>>> GetCategoryPublisherNotPagination()
        {
            return await _context.CategoryPublisher.OrderByDescending(e => e.CreatedDate).ToListAsync();
        }


        // GET: api/CategoryPublishers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryPublisher>> GetCategoryPublisher(Guid id)
        {
            var categoryPublisher = await _context.CategoryPublisher.FindAsync(id);

            if (categoryPublisher == null)
            {
                return NotFound();
            }

            return categoryPublisher;
        }

        // PUT: api/CategoryPublishers/5
        [HttpPut("{id}")]
        public IActionResult PutCategoryPublisher(Guid id, CategoryPublisher categoryPublisher)
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

                Response result = _categoryPublisherRepository.UpdateCategoryPublisher(categoryPublisher);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Update", "CategoryPublisher", true, id);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Update", "CategoryPublisher", false, id);
                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Update", "CategoryPublisher", false, id);
                throw;
            }
        }

        // POST: api/CategoryPublishers
        [HttpPost]
        public async Task<ActionResult<CategoryPublisher>> PostCategoryPublisher(CategoryPublisher categoryPublisher)
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

                bool check = _categoryPublisherRepository.CheckPublisherCode(categoryPublisher.PublisherCode);
                if (check)
                {
                    return BadRequest(new
                    {
                        message = "Mã nhà xuất bản này đã tồn tại "
                    });
                }

                categoryPublisher.CreatedDate = DateTime.Now;
                _context.CategoryPublisher.Add(categoryPublisher);
                await _context.SaveChangesAsync();

                _saveToDiary.SaveDiary(checkModel.Id, "Create", "CategoryPublisher", true, categoryPublisher.Id);
                return CreatedAtAction("GetCategoryPublisher", new { id = categoryPublisher.Id }, categoryPublisher);
            }
            catch(Exception)
            {
                throw;
            }
        }

        // DELETE: api/CategoryPublishers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoryPublisher(Guid id)
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

                var categoryPublisher = await _context.CategoryPublisher.FindAsync(id);
                if (categoryPublisher == null)
                {
                    return NotFound();
                }

                _context.CategoryPublisher.Remove(categoryPublisher);
                await _context.SaveChangesAsync();

                _saveToDiary.SaveDiary(checkModel.Id, "Delete", "CategoryPublisher", true, id);
                return NoContent();
            }
            catch(Exception)
            {
                throw;
            }
        }

        private bool CategoryPublisherExists(Guid id)
        {
            return _context.CategoryPublisher.Any(e => e.Id == id);
        }

        // HttpPost: /api/CategoryPublishers/InsertCategoryPublish
        [HttpPost]
        [Route("InsertCategoryPublish")]
        public IActionResult InsertCategoryPublish()
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

                bool checkAddPublish = false;
                List<AddCategoryPublishExcel> addCategoryPublishExcels = new List<AddCategoryPublishExcel>();

                using (ExcelPackage excelPackage = new ExcelPackage(fi))
                {
                    //Get a WorkSheet by name. If the worksheet doesn't exist, throw an exeption
                    ExcelWorksheet namedWorksheet = excelPackage.Workbook.Worksheets[0];

                    int rowCount = namedWorksheet.Dimension.Rows;
                    int colCount = namedWorksheet.Dimension.Columns;
                    for (int i = 2; i <= rowCount; i++)
                    {
                        CategoryPublisherDto categoryPublisherDto = new CategoryPublisherDto();
                        for (int j = 1; j <= colCount; j++)
                        {
                            string cells = "";
                            if (namedWorksheet.Cells[i, j].Value != null)
                            {
                                cells = namedWorksheet.Cells[i, j].Value.ToString();

                                if (j == 1) categoryPublisherDto.PublisherCode = cells;
                                else if (j == 2) categoryPublisherDto.PublisherName = cells;
                                else if (j == 3) categoryPublisherDto.Address = cells;
                                else if (j == 4) categoryPublisherDto.Note = cells;
                                else if (j == 5) categoryPublisherDto.IdCategory = new Guid(cells);
                            }
                        }
                        if (categoryPublisherDto.PublisherCode == null || categoryPublisherDto.PublisherName == null)
                        {
                            continue;
                        }
                        bool check = _categoryPublisherRepository.CheckPublisherCode(categoryPublisherDto.PublisherCode);
                        if (check)
                        {
                            continue;
                        }

                        categoryPublisherDto.Id = Guid.NewGuid();
                        categoryPublisherDto.Status = 0;
                        categoryPublisherDto.IsHided = false;
                        categoryPublisherDto.IsDeleted = false;
                        categoryPublisherDto.CreatedDate = DateTime.Now;
                        categoryPublisherDto.CreatedBy = checkModel.Id;


                        Response response = new Response();
                        response = _categoryPublisherRepository.InsertCategoryPublisher(categoryPublisherDto);
                        if (!response.Success)
                        {
                            checkAddPublish = true;
                            AddCategoryPublishExcel addCategoryPublishExcel = new AddCategoryPublishExcel();
                            addCategoryPublishExcel.Id = categoryPublisherDto.Id;
                            addCategoryPublishExcel.PublisherName = categoryPublisherDto.PublisherName;

                            addCategoryPublishExcels.Add(addCategoryPublishExcel);
                        }
                        else
                        {
                            _saveToDiary.SaveDiary(checkModel.Id, "Create", "CategoryPublisher", true, categoryPublisherDto.Id);
                        }
                    }
                }
                if (checkAddPublish)
                {
                    for(int i = 0; i < addCategoryPublishExcels.Count; i++)
                    {
                        _saveToDiary.SaveDiary(checkModel.Id, "Create", "CategoryPublisher", false, addCategoryPublishExcels[i].Id);
                    }
                    return Ok(new
                    {
                        Success = false,
                        Body = addCategoryPublishExcels
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
        // GET: api/CategoryPublishers/GetFileExcelCategoryPublish
        [HttpGet]
        [Route("GetFileExcelCategoryPublish")]
        public IActionResult GetFileExcelCategoryPublish()
        {
            try
            {

                var path = string.Concat(_appSettingModel.ServerFileExcel, "\\", "Mau_ImportNhaPhatHanh.xlsx");
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
                        if(units[i].Id == new Guid("d256d275-0f16-41f1-bf06-21a119d7ed09"))
                        {
                            namedWorksheet.Cells[row, 1].Value = units[i].Id;
                            namedWorksheet.Cells[row, 2].Value = units[i].CategoryName;
                            row++;
                        }
                    }

                    var pathToSave = $"{_appSettingModel.ServerFileExcel}\\{fileCodeName}.xlsx";
                    FileInfo fiToSave = new FileInfo(pathToSave);
                    //Save your file
                    excelPackage.SaveAs(fiToSave);
                }
                //download file excel
                byte[] fileBytes = System.IO.File.ReadAllBytes(string.Concat(_appSettingModel.ServerFileExcel, "\\", fileCodeName, ".xlsx"));
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Mau_ImportNhaPhatHanh.xlsx");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
