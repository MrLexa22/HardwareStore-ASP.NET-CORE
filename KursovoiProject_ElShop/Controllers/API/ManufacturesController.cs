using KursovoiProject_ElShop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KursovoiProject_ElShop.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManufacturesController : ControllerBase
    {
        private readonly ElShopContext _context;
        public ManufacturesController(ElShopContext context)
        {
            _context = context;
        }

        [HttpGet("GetMaufacturesByIdCateg/{id}")]
        public List<ManufacturesList> GetMaufacturesByIdCateg(int id)
        {
            List<ManufacturesList> list = new List<ManufacturesList>();
            using (var context = new ElShopContext())
            {
                var conn = context.Database.GetDbConnection();
                conn.Open();
                var command = conn.CreateCommand();
                string query = @$"SET @p0='{id}'; CALL `GetManufacturesByIdCategory`(@p0);";
                command.CommandText = query;
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ManufacturesList h = new ManufacturesList();
                    h.ManufactureId = reader.GetInt16(0);
                    h.NameManufacture = reader.GetString(1);
                    h.Selected = false;
                    list.Add(h);
                }
                conn.Close();
            }
            return list;
        }

        [HttpGet("GetManufacturesAll")]
        public async Task<ActionResult<IEnumerable<Manufacture>>> GetManufacturesAll()
        {
            return await _context.Manufactures.ToListAsync();
        }

        [HttpGet("GetManufactureByID/{id}")]
        public async Task<ActionResult<Manufacture>> GetManufactureByID(int id)
        {
            return await _context.Manufactures.Where(p=>p.IdManufacture == id).FirstAsync();
        }

        [HttpGet("CheckExistManufacture/{name}/{id}")]
        public async Task<ActionResult<Boolean>> CheckExistManufacture(string name, int id)
        {
            var list = _context.Manufactures.Where(p => p.NameManufacture.ToLower() == name.ToLower() && p.IdManufacture != id).ToList();
            Boolean result = false;
            if (list.Count() <= 0)
                result = true;
            return result;
        }

        [HttpGet("EditOrCreate/{id}/{name}")]
        public async Task<ActionResult> EditOrCreate(int id, string name)
        {
            Manufacture manufacture = new Manufacture();
            manufacture.NameManufacture = name;
            if(id == 0)
                _context.Add(manufacture);
            else
            {
                manufacture = await _context.Manufactures.FindAsync(id);
                manufacture.NameManufacture = name;
                _context.Update(manufacture);
            }
            _context.SaveChanges();
            return NoContent();
        }

        [HttpGet("GetMaufacturesByName/{name}")]
        public List<ManufacturesList> GetMaufacturesByIdCateg(string name)
        {
            List<ManufacturesList> list = new List<ManufacturesList>();
            using (var context = new ElShopContext())
            {
                var conn = context.Database.GetDbConnection();
                conn.Open();
                var command = conn.CreateCommand();
                string query = @$"SET @p0='{name}'; CALL `GetManufacturesByName`(@p0);";
                command.CommandText = query;
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ManufacturesList h = new ManufacturesList();
                    h.ManufactureId = reader.GetInt16(0);
                    h.NameManufacture = reader.GetString(1);
                    h.Selected = false;
                    list.Add(h);
                }
                conn.Close();
            }
            return list;
        }
    }
}
