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
    public class ViewGoodsWithManufacturesController : ControllerBase
    {
        private readonly ElShopContext _context;

        public ViewGoodsWithManufacturesController(ElShopContext context)
        {
            _context = context;
        }

        // GET: api/ViewGoodsWithManufactures
        [HttpGet("Random5")]
        public async Task<ActionResult<IEnumerable<ViewGoodsWithManufacture>>> GetRandomViewGoodsWithManufactures()
        {
            var al = _context.ViewGoodsWithManufactures.ToList();
            foreach(var a in al)
            {
                if (a.FtppathImage == null)
                    a.FtppathImage = BaseAddresse.Server + "TovariImages/NoImageTovar.jpg";
                else
                    a.FtppathImage = BaseAddresse.Server + "TovariImages/" + a.FtppathImage;
            }
            return al.OrderBy(r => Guid.NewGuid()).Take(5).ToList();
        }

/*        // GET: api/ViewGoodsWithManufactures/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ViewGoodsWithManufacture>> GetViewGoodsWithManufacture(int id)
        {
          if (_context.ViewGoodsWithManufactures == null)
          {
              return NotFound();
          }
            var viewGoodsWithManufacture = await _context.ViewGoodsWithManufactures.FindAsync(id);

            if (viewGoodsWithManufacture == null)
            {
                return NotFound();
            }

            return viewGoodsWithManufacture;
        }

        // PUT: api/ViewGoodsWithManufactures/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutViewGoodsWithManufacture(int id, ViewGoodsWithManufacture viewGoodsWithManufacture)
        {
            if (id != viewGoodsWithManufacture.IdGood)
            {
                return BadRequest();
            }

            _context.Entry(viewGoodsWithManufacture).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ViewGoodsWithManufactureExists(id))
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

        // POST: api/ViewGoodsWithManufactures
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ViewGoodsWithManufacture>> PostViewGoodsWithManufacture(ViewGoodsWithManufacture viewGoodsWithManufacture)
        {
          if (_context.ViewGoodsWithManufactures == null)
          {
              return Problem("Entity set 'ElShopContext.ViewGoodsWithManufactures'  is null.");
          }
            _context.ViewGoodsWithManufactures.Add(viewGoodsWithManufacture);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetViewGoodsWithManufacture", new { id = viewGoodsWithManufacture.IdGood }, viewGoodsWithManufacture);
        }

        // DELETE: api/ViewGoodsWithManufactures/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteViewGoodsWithManufacture(int id)
        {
            if (_context.ViewGoodsWithManufactures == null)
            {
                return NotFound();
            }
            var viewGoodsWithManufacture = await _context.ViewGoodsWithManufactures.FindAsync(id);
            if (viewGoodsWithManufacture == null)
            {
                return NotFound();
            }

            _context.ViewGoodsWithManufactures.Remove(viewGoodsWithManufacture);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ViewGoodsWithManufactureExists(int id)
        {
            return (_context.ViewGoodsWithManufactures?.Any(e => e.IdGood == id)).GetValueOrDefault();
        }*/
    }
}
