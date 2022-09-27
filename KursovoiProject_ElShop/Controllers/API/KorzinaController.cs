using KursovoiProject_ElShop.Controllers.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KursovoiProject_ElShop.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class KorzinaController : ControllerBase
    {
        private readonly ElShopContext _context;

        public KorzinaController(ElShopContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Korzina>>> GetUsers()
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            return await _context.Korzinas.ToListAsync();
        }

        [HttpGet("AddToCart/{idUser}/{idTovar}")]
        public async Task<ActionResult<Korzina>> PostKorzinaAdd(int idUser, int idTovar)
        {
            Korzina korzina = new Korzina();
            korzina.GoodsId = idTovar;
            korzina.UserId = idUser;
            korzina.Count = 1;
            _context.Korzinas.Add(korzina);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetAddsSite", new { id = korzina.IdKorzinaGood }, korzina);
        }

        [HttpGet("GetCarts/{id}")]
        public async Task<ActionResult<List<Korzina>>> GetKorzinaByIDUser(int id)
        {
            var list = await _context.Korzinas.Where(p => p.UserId == id).ToListAsync();
            return list;
        }

        [HttpDelete("DeleteFromCart/{id}")]
        public async Task<IActionResult> DeleteFromCartId(int id)
        {
            var a = _context.Korzinas.Where(p => p.IdKorzinaGood == id).First();
            _context.Remove(a);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPut("UpdateCart/{id}/{newKolvo}")]
        public async Task<IActionResult> UpdateCart(int id, int newKolvo)
        {
            var a = _context.Korzinas.Where(p => p.IdKorzinaGood == id).First();
            a.Count = newKolvo;
            _context.Update(a);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
