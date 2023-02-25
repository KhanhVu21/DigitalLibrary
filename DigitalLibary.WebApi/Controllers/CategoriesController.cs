using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DigitalLibary.Data.Data;
using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Repository.IRepository;
using DigitalLibary.WebApi.Common;
using DigitalLibary.WebApi.Helper;

namespace DigitalLibary.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly DataContext _context;

        private readonly CategoriesRepository _categoriesRepository;
        private readonly SaveToDiary _saveToDiary;
        private readonly JwtService _jwtService;
        private readonly IUserRepository _userRepository;

        public CategoriesController(DataContext context, CategoriesRepository categoriesRepository, JwtService jwtService,
        IUserRepository userRepository, SaveToDiary saveToDiary)
        {
            _context = context;
            _categoriesRepository = categoriesRepository;
            _saveToDiary = saveToDiary;
            _jwtService = jwtService;
            _userRepository = userRepository;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategory(int pageNumber, int pageSize)
        {
            return await _context.Category.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(Guid id)
        {
            var category = await _context.Category.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // GET: api/Categories/SearchByName
        [HttpGet("SearchByName")]
        public List<Category> SearchByName(String SearchString, int pageNumber, int pageSize)
        {
            return _categoriesRepository.SearchByName(SearchString, pageNumber, pageSize);
        }

        // PUT: api/Categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(Guid id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/AddNewCategories
        [HttpPost("AddNewCategories")]
        public async Task<ActionResult<Category>> PostCategory(Category category)
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

                _context.Category.Add(category);
                await _context.SaveChangesAsync();

                _saveToDiary.SaveDiary(checkModel.Id, "Create", "Category", true, category.Id);

                return CreatedAtAction("GetCategory", new { id = category.Id }, category);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
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

                var category = await _context.Category.FindAsync(id);
                if (category == null)
                {
                    return NotFound();
                }

                _context.Category.Remove(category);
                await _context.SaveChangesAsync();

                _saveToDiary.SaveDiary(checkModel.Id, "Delete", "Category", true, id);
                return NoContent();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool CategoryExists(Guid id)
        {
            return _context.Category.Any(e => e.Id == id);
        }
    }
}
