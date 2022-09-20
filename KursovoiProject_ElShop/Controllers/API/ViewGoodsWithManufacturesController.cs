using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KursovoiProject_ElShop;
using KursovoiProject_ElShop.Controllers.Validation;
using KursovoiProject_ElShop.Models;
using Newtonsoft.Json;

namespace KursovoiProject_ElShop.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewGoodsWithManufacturesController : ControllerBase
    {
        private readonly ElShopContext _context;
        HttpClient client = new HttpClient();

        public ViewGoodsWithManufacturesController(ElShopContext context)
        {
            _context = context;
            client.BaseAddress = new Uri(BaseAddresse.Address);
        }

        // GET: api/ViewGoodsWithManufactures
        [HttpGet("Random5")]
        public async Task<ActionResult<IEnumerable<ViewGoodsWithManufacture>>> GetRandomViewGoodsWithManufactures()
        {
            var al = _context.ViewGoodsWithManufactures.Where(p=>p.IsAvaliable != false).ToList();
            foreach(var a in al)
            {
                if (a.FtppathImage == null)
                    a.FtppathImage = BaseAddresse.Server + "TovariImages/NoImageTovar.jpg";
                else
                    a.FtppathImage = BaseAddresse.Server + "TovariImages/" + a.FtppathImage;
            }
            return al.OrderBy(r => Guid.NewGuid()).Take(5).ToList();
        }

        [HttpGet("GetByCategori/{id}/{IDUser}")]
        public async Task<ActionResult<IEnumerable<TovariUsersModel>>> GetViewGoodsWithManufactures(int id, int IDUser)
        {
            var al = _context.ViewGoodsWithManufactures.Where(p => p.CategoriId == id && p.IsAvaliable != false).ToList();
            string CategoriName = _context.Categories.Where(p => p.IdCategori == id).First().NameCategori;
            foreach (var a in al)
            {
                if (a.FtppathImage == null)
                    a.FtppathImage = BaseAddresse.Server + "TovariImages/NoImageTovar.jpg";
                else
                    a.FtppathImage = BaseAddresse.Server + "TovariImages/" + a.FtppathImage;
            }
            List<TovariUsersModel> list = new List<TovariUsersModel>();
            var conn = _context.Database.GetDbConnection();

            foreach (var a in al)
            {
                TovariUsersModel b;
                if (IDUser != 0)
                {
                    conn.Open();
                    var command = conn.CreateCommand();
                    string query = @$"SET @p0='{IDUser}'; SET @p1='{a.IdGood}'; SELECT `CheckCart`(@p0, @p1) AS `CheckCart`;";
                    command.CommandText = query;
                    var reader = command.ExecuteReader();
                    int isInCart = 0;
                    while (reader.Read())
                    {
                        isInCart = reader.GetInt16(0);
                    }
                    conn.Close();
                    b = new TovariUsersModel(a.IdGood, a.Cost, a.Name, a.Description, a.FtppathImage, a.CategoriId, a.ManufactureId, a.IsAvaliable, a.IdManufacture, a.NameManufacture, a.NameWithManufacture, isInCart, CategoriName);
                }
                else
                    b = new TovariUsersModel(a.IdGood, a.Cost, a.Name, a.Description, a.FtppathImage, a.CategoriId, a.ManufactureId, a.IsAvaliable, a.IdManufacture, a.NameManufacture, a.NameWithManufacture, 0, CategoriName);
                list.Add(b);
            }
            return list.OrderBy(p=>p.NameManufacture).ToList();
        }

        [HttpGet("GetByName/{name}/{IDUser}")]
        public async Task<ActionResult<IEnumerable<TovariUsersModel>>> GetViewGoodsWithManufacturesName(string name, int IDUser)
        {
            var al = _context.ViewGoodsWithManufactures.Where(p => p.NameWithManufacture.Contains(name) && p.IsAvaliable != false).ToList();
            foreach (var a in al)
            {
                if (a.FtppathImage == null)
                    a.FtppathImage = BaseAddresse.Server + "TovariImages/NoImageTovar.jpg";
                else
                    a.FtppathImage = BaseAddresse.Server + "TovariImages/" + a.FtppathImage;
            }
            List<TovariUsersModel> list = new List<TovariUsersModel>();
            var conn = _context.Database.GetDbConnection();

            foreach (var a in al)
            {
                TovariUsersModel b;
                if (IDUser != 0)
                {
                    conn.Open();
                    var command = conn.CreateCommand();
                    string query = @$"SET @p0='{IDUser}'; SET @p1='{a.IdGood}'; SELECT `CheckCart`(@p0, @p1) AS `CheckCart`;";
                    command.CommandText = query;
                    var reader = command.ExecuteReader();
                    int isInCart = 0;
                    while (reader.Read())
                    {
                        isInCart = reader.GetInt16(0);
                    }
                    conn.Close();
                    b = new TovariUsersModel(a.IdGood, a.Cost, a.Name, a.Description, a.FtppathImage, a.CategoriId, a.ManufactureId, a.IsAvaliable, a.IdManufacture, a.NameManufacture, a.NameWithManufacture, isInCart, "");
                }
                else
                    b = new TovariUsersModel(a.IdGood, a.Cost, a.Name, a.Description, a.FtppathImage, a.CategoriId, a.ManufactureId, a.IsAvaliable, a.IdManufacture, a.NameManufacture, a.NameWithManufacture, 0, "");
                list.Add(b);
            }
            return list.OrderBy(p => p.NameManufacture).ToList();
        }

        // GET: api/ViewGoodsWithManufactures/5
        [HttpGet("GetTovar/{id}/{idUser}")]
        public async Task<ActionResult<TovariUsersModel>> GetViewGoodsWithManufacture(int id,int idUser)
        {
            TovariUsersModel b;
            var viewGoodsWithManufacture = _context.ViewGoodsWithManufactures.Where(p=>p.IdGood==id).First();
            string CategoriName = _context.Categories.Where(p => p.IdCategori == viewGoodsWithManufacture.CategoriId).First().NameCategori;
            int isInCart = 0;
            if (viewGoodsWithManufacture.FtppathImage == null)
                viewGoodsWithManufacture.FtppathImage = BaseAddresse.Server + "TovariImages/NoImageTovar.jpg";
            else
                viewGoodsWithManufacture.FtppathImage = BaseAddresse.Server + "TovariImages/" + viewGoodsWithManufacture.FtppathImage;

            List <AddsSite> Right1Adds = (List<AddsSite>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/AddsSites/GetAddsSiteType/2").Result.Content.ReadAsStringAsync().Result, typeof(List<AddsSite>));
            List <AddsSite> Right2Adds = (List<AddsSite>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/AddsSites/GetAddsSiteType/3").Result.Content.ReadAsStringAsync().Result, typeof(List<AddsSite>));
            if (idUser != 0)
            {
                var conn = _context.Database.GetDbConnection();
                conn.Open();
                var command = conn.CreateCommand();
                string query = @$"SET @p0='{idUser}'; SET @p1='{id}'; SELECT `CheckCart`(@p0, @p1) AS `CheckCart`;";
                command.CommandText = query;
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    isInCart = reader.GetInt16(0);
                }
                conn.Close();
                b = new TovariUsersModel(CategoriName, viewGoodsWithManufacture.IdGood, viewGoodsWithManufacture.Cost, viewGoodsWithManufacture.Name, viewGoodsWithManufacture.Description, viewGoodsWithManufacture.FtppathImage, viewGoodsWithManufacture.CategoriId, viewGoodsWithManufacture.ManufactureId, viewGoodsWithManufacture.IsAvaliable, viewGoodsWithManufacture.IdManufacture, viewGoodsWithManufacture.NameManufacture, viewGoodsWithManufacture.NameWithManufacture,isInCart, Right1Adds, Right2Adds);
            }
            else
                b = new TovariUsersModel(CategoriName, viewGoodsWithManufacture.IdGood, viewGoodsWithManufacture.Cost, viewGoodsWithManufacture.Name, viewGoodsWithManufacture.Description, viewGoodsWithManufacture.FtppathImage, viewGoodsWithManufacture.CategoriId, viewGoodsWithManufacture.ManufactureId, viewGoodsWithManufacture.IsAvaliable, viewGoodsWithManufacture.IdManufacture, viewGoodsWithManufacture.NameManufacture, viewGoodsWithManufacture.NameWithManufacture, 0, Right1Adds, Right2Adds);

            return b;
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
