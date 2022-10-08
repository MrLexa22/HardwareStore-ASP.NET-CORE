using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KursovoiProject_ElShop;
using KursovoiProject_ElShop.Controllers.Validation;

namespace KursovoiProject_ElShop.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddsSites : ControllerBase
    {
        private readonly ElShopContext _context;

        public AddsSites(ElShopContext context)
        {
            _context = context;
        }

        // GET: api/AddsSites
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AddsSite>>> GetAddsSites()
        {
          if (_context.AddsSites == null)
          {
              return NotFound();
          }
            return await _context.AddsSites.ToListAsync();
        }

        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<AddsSite>>> GetAllAddsSites()
        {
            var addsSite = await _context.AddsSites.ToListAsync();
            foreach (var a in addsSite)
            {
                if (a.TypeWhere == 1)
                    a.FtppathImage = BaseAddresse.Server + "AddsImages/MainPage/" + a.FtppathImage;
                if (a.TypeWhere == 2)
                    a.FtppathImage = BaseAddresse.Server + "AddsImages/Right1/" + a.FtppathImage;
                if (a.TypeWhere == 3)
                    a.FtppathImage = BaseAddresse.Server + "AddsImages/Right2/" + a.FtppathImage;
            }
            return addsSite;
        }

        // GET: api/AddsSites/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AddsSite>> GetAddsSite(int id)
        {
          if (_context.AddsSites == null)
          {
              return NotFound();
          }
            var addsSite = await _context.AddsSites.FindAsync(id);

            if (addsSite == null)
            {
                return NotFound();
            }

            return addsSite;
        }

        [HttpGet("GetAddsSiteType/{type}")]
        public async Task<ActionResult<IEnumerable<AddsSite>>> GetAddsSiteType(int type)
        {
            var addsSite = await _context.AddsSites.Where(p=>p.TypeWhere == type && (p.DateEdn >= DateTime.Now || p.DateEdn == null)).ToListAsync();
            foreach(var a in addsSite)
            {
                if(type == 1)
                    a.FtppathImage = BaseAddresse.Server + "AddsImages/MainPage/" + a.FtppathImage;
                if (type == 2)
                    a.FtppathImage = BaseAddresse.Server + "AddsImages/Right1/" + a.FtppathImage;
                if (type == 3)
                    a.FtppathImage = BaseAddresse.Server + "AddsImages/Right2/" + a.FtppathImage;
            }
            return addsSite;
        }

        // PUT: api/AddsSites/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAddsSite(int id, AddsSite addsSite)
        {
            if (id != addsSite.IdAdds)
            {
                return BadRequest();
            }

            _context.Entry(addsSite).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AddsSiteExists(id))
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

        // POST: api/AddsSites
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AddsSite>> PostAddsSite(AddsSite addsSite)
        {
          if (_context.AddsSites == null)
          {
              return Problem("Entity set 'ElShopContext.AddsSites'  is null.");
          }
            _context.AddsSites.Add(addsSite);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAddsSite", new { id = addsSite.IdAdds }, addsSite);
        }

        // DELETE: api/AddsSites/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddsSite(int id)
        {
            if (_context.AddsSites == null)
            {
                return NotFound();
            }
            var addsSite = await _context.AddsSites.FindAsync(id);
            if (addsSite == null)
            {
                return NotFound();
            }

            _context.AddsSites.Remove(addsSite);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AddsSiteExists(int id)
        {
            return (_context.AddsSites?.Any(e => e.IdAdds == id)).GetValueOrDefault();
        }
    }
}
