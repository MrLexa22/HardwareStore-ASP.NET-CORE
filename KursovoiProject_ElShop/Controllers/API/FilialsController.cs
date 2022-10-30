using KursovoiProject_ElShop.Controllers.Validation;
using KursovoiProject_ElShop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KursovoiProject_ElShop.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilialsController : ControllerBase
    {
        private readonly ElShopContext _context;
        HttpClient client = new HttpClient();

        public FilialsController(ElShopContext context)
        {
            _context = context;
            client.BaseAddress = new Uri(BaseAddresse.Address);
        }

        [HttpGet("GetFilialsTovar/{id}")]
        public async Task<ActionResult<IEnumerable<FilialsNalichTovar>>> GetViewGoodsWithManufactures(int id)
        {
            List<FilialsNalichTovar> list = new List<FilialsNalichTovar>();
            var conn = _context.Database.GetDbConnection();
            conn.Open();
            var command = conn.CreateCommand();
            string query = @$"SET @p0='{id}'; CALL `GetTovarFilialsNalic`(@p0);";
            command.CommandText = query;
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                FilialsNalichTovar h = new FilialsNalichTovar();
                h.ID_Good = reader.GetInt16(0);
                h.ID_Filial = reader.GetInt16(1);
                h.CountSklad = reader.GetInt16(2);
                h.NameFilial = reader.GetString(3);
                h.AddressFilial = reader.GetString(4);
                h.Nalich = reader.GetString(5);
                list.Add(h);
            }
            conn.Close();

            return list.OrderBy(p => p.NameFilial).ToList();
        }

        [HttpGet("GetFilials")]
        public async Task<ActionResult<IEnumerable<Filial>>> GetFilials()
        {
            return await _context.Filials.Where(p=>p.Availeble==true).ToListAsync();
        }

        [HttpGet("GetFilialsWhereWork/{email}")]
        public async Task<ActionResult<IEnumerable<Filial>>> GetFilialsWhereWork(string email)
        {
            int UserID = _context.Users.Where(p => p.Login == email).First().IdUser;
            var listFilials = _context.WorkersFilials.Include(p=>p.Filial).Where(p => p.UserId == UserID).ToList();
            List<Filial> filials = new List<Filial>();
            foreach(var a in listFilials)
            {
                filials.Add(a.Filial);
            }
            return filials;
        }

        [HttpGet("GetAllFilials")]
        public async Task<ActionResult<IEnumerable<Filial>>> GetAllFilials()
        {
            return await _context.Filials.ToListAsync();
        }

        [HttpGet("CheckExistFilial/{id}/{name}/{address}")]
        public async Task<ActionResult<Boolean>> CheckExistFilial(int id, string name, string address)
        {
            Boolean result = true;
            if (_context.Filials.Where(p => (p.NameFilial.ToLower() == name.ToLower() || p.AddressFilial.ToLower() == address.ToLower()) && p.IdFilial != id).Count() > 0)
                result = false;
            return result;
        }

        [HttpGet("AddEditFilial/{id}/{name}/{address}")]
        public async Task<ActionResult<Filial>> AddEditFilial(int id, string name, string address)
        {
            Filial model = new Filial();
            if(id == 0)
            {
                model.NameFilial = name.Trim();
                model.AddressFilial = address.Trim();
                _context.Filials.Add(model);
                await _context.SaveChangesAsync();
            }
            if (id > 0)
            {
                model = await _context.Filials.FindAsync(id);
                model.NameFilial = name.Trim();
                model.AddressFilial = address.Trim();
                _context.Filials.Update(model);
                await _context.SaveChangesAsync();
            }
            return model;
        }

        [HttpGet("GetFilialsWithBoolean/{idSotr}")]
        public async Task<ActionResult<IEnumerable<FilialsWithBoolean>>> GetFilialsWithBoolean(int idSotr)
        {
            var l = await _context.Filials.ToListAsync();
            List<FilialsWithBoolean> list = new List<FilialsWithBoolean>();
            List<WorkersFilial> listFilialsWorker = new List<WorkersFilial>();
            if (idSotr > 0)
            {
                var list1 = _context.Workers.Where(p => p.IdWorker == idSotr).First();
                listFilialsWorker = _context.WorkersFilials.Where(p => p.UserId == list1.UserId).ToList();
            }
            foreach(var a in l)
            {
                FilialsWithBoolean h = new FilialsWithBoolean();
                h.ID_Filial = a.IdFilial;
                h.IsAvalib = a.Availeble;
                h.NameFilial = a.NameFilial;
                if (idSotr > 0)
                {
                    if (listFilialsWorker.Where(p => p.FilialId == a.IdFilial).Count() > 0)
                        h.IsSelected = true;
                    else
                        h.IsSelected = false;
                }
                else
                    h.IsSelected = false;
                list.Add(h);
            }
            return list;
        }

        [HttpGet("GetFilialByID/{id}")]
        public async Task<ActionResult<Filial>> GetFilialByID(int id)
        {
            return await _context.Filials.FindAsync(id);
        }

        [HttpDelete("CloseFilial/{id}")]
        public async Task<ActionResult> CloseFilial(int id)
        {
            var a = await _context.Filials.FindAsync(id);
            a.Availeble = false;
            _context.Update(a);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("OpenFilial/{id}")]
        public async Task<ActionResult> OpenFilial(int id)
        {
            var a = await _context.Filials.FindAsync(id);
            a.Availeble = true;
            _context.Update(a);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
