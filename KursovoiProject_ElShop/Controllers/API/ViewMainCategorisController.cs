using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KursovoiProject_ElShop;

namespace KursovoiProject_ElShop.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewMainCategorisController : ControllerBase
    {
        private readonly ElShopContext _context;

        public ViewMainCategorisController(ElShopContext context)
        {
            _context = context;
        }

        // GET: api/ViewMainCategoris
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ViewMainCategori>>> GetViewMainCategoris()
        {
          if (_context.ViewMainCategoris == null)
          {
              return NotFound();
          }
            return await _context.ViewMainCategoris.ToListAsync();
        }

 /*       // GET: api/ViewMainCategoris/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ViewMainCategori>> GetViewMainCategori(int id)
        {
          if (_context.ViewMainCategoris == null)
          {
              return NotFound();
          }
            var viewMainCategori = await _context.ViewMainCategoris.FindAsync(id);

            if (viewMainCategori == null)
            {
                return NotFound();
            }

            return viewMainCategori;
        }

        // PUT: api/ViewMainCategoris/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutViewMainCategori(int id, ViewMainCategori viewMainCategori)
        {
            if (id != viewMainCategori.IdCategori)
            {
                return BadRequest();
            }

            _context.Entry(viewMainCategori).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ViewMainCategoriExists(id))
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

        // POST: api/ViewMainCategoris
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ViewMainCategori>> PostViewMainCategori(ViewMainCategori viewMainCategori)
        {
          if (_context.ViewMainCategoris == null)
          {
              return Problem("Entity set 'ElShopContext.ViewMainCategoris'  is null.");
          }
            _context.ViewMainCategoris.Add(viewMainCategori);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetViewMainCategori", new { id = viewMainCategori.IdCategori }, viewMainCategori);
        }

        // DELETE: api/ViewMainCategoris/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteViewMainCategori(int id)
        {
            if (_context.ViewMainCategoris == null)
            {
                return NotFound();
            }
            var viewMainCategori = await _context.ViewMainCategoris.FindAsync(id);
            if (viewMainCategori == null)
            {
                return NotFound();
            }

            _context.ViewMainCategoris.Remove(viewMainCategori);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ViewMainCategoriExists(int id)
        {
            return (_context.ViewMainCategoris?.Any(e => e.IdCategori == id)).GetValueOrDefault();
        }*/
    }
}
