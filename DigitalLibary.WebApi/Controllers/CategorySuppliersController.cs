using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DigitalLibary.Data.Data;
using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Repository.IRepository;
using Microsoft.Data.SqlClient;
using DigitalLibary.Service.Common;
using DigitalLibary.WebApi.Common;
using DigitalLibary.WebApi.Helper;

namespace DigitalLibary.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategorySuppliersController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ICategorySupplier _ICategorySupplier;
        private readonly SaveToDiary _saveToDiary;
        private readonly JwtService _jwtService;
        private readonly IUserRepository _userRepository;

        public CategorySuppliersController(DataContext context, ICategorySupplier _ICategorySupplier,
                    JwtService jwtService,
        IUserRepository userRepository, SaveToDiary saveToDiary)
        {
            _context = context;
            this._ICategorySupplier = _ICategorySupplier;
            _saveToDiary = saveToDiary;
            _jwtService = jwtService;
            _userRepository = userRepository;
        }

        // GET: api/CategorySuppliers
        [HttpGet]
        [Route("SearchByName")]
        public List<CategorySupplier> SearchByName(String SearchString, int pageNumber, int pageSize)
        {
            return _ICategorySupplier.SearchByName(SearchString, pageNumber, pageSize);
        }

        // GET: api/CategorySuppliers
        [HttpGet]
        [Route("NotPagination")]
        public async Task<ActionResult<IEnumerable<CategorySupplier>>> GetCategoryPublisherNotPagination()
        {
            return await _context.CategorySupplier.OrderByDescending(e => e.CreatedDate).ToListAsync();
        }

        // GET: api/CategorySuppliers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategorySupplier>>> GetCategorySupplier(int pageNumber, int pageSize)
        {
            return await _context.CategorySupplier.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        // GET: api/CategorySuppliers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategorySupplier>> GetCategorySupplier(Guid id)
        {
            var categorySupplier = await _context.CategorySupplier.FindAsync(id);

            if (categorySupplier == null)
            {
                return NotFound();
            }

            return categorySupplier;
        }

        // PUT: api/CategorySuppliers/5
        [HttpPut("{id}")]
        public IActionResult PutCategorySupplier(Guid id, CategorySupplier categorySupplier)
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

                Response result = _ICategorySupplier.UpdateCategorySupplier(categorySupplier);
                if (result.Success)
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Update", "CategorySupplier", true, id);
                    return Ok(new
                    {
                        message = result.Message,
                    });
                }
                else
                {
                    _saveToDiary.SaveDiary(checkModel.Id, "Update", "CategorySupplier", false, id);
                    return BadRequest(new
                        {
                            message = result.Message
                        });
                }
            }
            catch (Exception)
            {
                _saveToDiary.SaveDiary(IdUserCurrent, "Update", "CategorySupplier", false, id);
                throw;
            }
        }

        // POST: api/CategorySuppliers
        [HttpPost]
        public async Task<ActionResult<CategorySupplier>> PostCategorySupplier(CategorySupplier categorySupplier)
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

                bool check = _ICategorySupplier.CheckExitsSupplierCode(categorySupplier.SupplierCode);
                if (check)
                {
                    return BadRequest(new
                    {
                        message = "Mã nhà cung cấp này đã tồn tại "
                    });
                }

                categorySupplier.CreatedDate = DateTime.Now;
                _context.CategorySupplier.Add(categorySupplier);
                await _context.SaveChangesAsync();

                _saveToDiary.SaveDiary(checkModel.Id, "Create", "CategorySupplier", true, categorySupplier.Id);
                return CreatedAtAction("GetCategorySupplier", new { id = categorySupplier.Id }, categorySupplier);
            }
            catch(Exception)
            {
                throw;
            }
        }

        // DELETE: api/CategorySuppliers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategorySupplier(Guid id)
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

                var categorySupplier = await _context.CategorySupplier.FindAsync(id);
                if (categorySupplier == null)
                {
                    return NotFound();
                }

                _context.CategorySupplier.Remove(categorySupplier);
                await _context.SaveChangesAsync();

                _saveToDiary.SaveDiary(checkModel.Id, "Delete", "CategorySupplier", true, categorySupplier.Id);

                return NoContent();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool CategorySupplierExists(Guid id)
        {
            return _context.CategorySupplier.Any(e => e.Id == id);
        }
    }
}
