using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KursovoiProject_ElShop;
using KursovoiProject_ElShop.Models;

namespace KursovoiProject_ElShop.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ElShopContext _context;

        public CategoriesController(ElShopContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
          if (_context.Categories == null)
          {
              return NotFound();
          }
            return await _context.Categories.ToListAsync();
        }

        [HttpGet("GetAllCategs")]
        public async Task<ActionResult<IEnumerable<CategoriesSite>>> GetAllCategories()
        {
            var list = await _context.Categories.ToListAsync();
            var main = await _context.MainCategories.ToListAsync();
            List<CategoriesSite> listCategs = new List<CategoriesSite>();
            foreach(var a in list)
            {
                CategoriesSite h = new CategoriesSite();
                h.CategoryName = a.NameCategori;
                h.CategoryId = a.IdCategori;
                if (main.Where(p => p.CategoriId == a.IdCategori).Count() > 0)
                    h.IsMain = true;
                else
                    h.IsMain = false;
                listCategs.Add(h);
            }
            return listCategs;
        }

        [HttpGet("GetCategory/{id}")]
        public async Task<ActionResult<CategoriesSite>> GetCategorySecond(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            CategoriesSite categ = new CategoriesSite();
            categ.CategoryId = id;
            categ.CategoryName = category.NameCategori;
            categ.IsMain = _context.MainCategories.Where(p => p.CategoriId == id).Count() > 0 ? true : false;
            return categ;
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
          if (_context.Categories == null)
          {
              return NotFound();
          }
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        [HttpGet("GetByName/{name}/{id}")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategoryByName(string name, int id)
        {
            List<Category> category = new List<Category>();
            if(id == 0)
                category = _context.Categories.Where(p=>p.NameCategori == name).ToList();
            else
                category = _context.Categories.Where(p => p.NameCategori == name && p.IdCategori != id).ToList();
            return category;
        }

        [HttpGet("MainCategories")]
        public async Task<ActionResult<IEnumerable<MainCategory>>> GetMainCategories()
        {
            return await _context.MainCategories.ToListAsync();
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("UpdateCategory")]
        public async Task<IActionResult> UpdateCategory(CategoriesSite categ)
        {
            try
            {
                Category category = new Category();
                category.NameCategori = categ.CategoryName;
                category.IdCategori = categ.CategoryId;

                if (_context.MainCategories.Where(p => p.CategoriId == categ.CategoryId).Count() == 0 && categ.IsMain == true)
                {
                    MainCategory h = new MainCategory();
                    h.CategoriId = category.IdCategori;
                    _context.Add(h);
                }
                if (_context.MainCategories.Where(p => p.CategoriId == categ.CategoryId).Count() > 0 && categ.IsMain == false)
                {
                    var f = _context.MainCategories.Where(p => p.CategoriId == categ.CategoryId).First();
                    _context.Remove(f);
                }
                _context.Update(category);
                _context.SaveChanges();
            }
            catch { }

            return NoContent();
        }

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
          if (_context.Categories == null)
          {
              return Problem("Entity set 'ElShopContext.Categories'  is null.");
          }
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = category.IdCategori }, category);
        }

        [HttpPost("Create")]
        public async Task<ActionResult<Category>> CreateCategory(CategoriesSite categ)
        {
            Category category = new Category();
            category.NameCategori = categ.CategoryName;
            category.IdCategori = categ.CategoryId;

            _context.Add(category);
            _context.SaveChanges();

            if (categ.IsMain == true)
            {
                MainCategory h = new MainCategory();
                h.CategoriId = category.IdCategori;
                _context.Add(h);
                _context.SaveChanges();
            }

            return NoContent();
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                _context.Remove(category);
                _context.SaveChanges();
            }
            catch { }

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return (_context.Categories?.Any(e => e.IdCategori == id)).GetValueOrDefault();
        }
    }
}
