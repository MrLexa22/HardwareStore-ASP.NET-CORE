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

        [HttpGet("GetAllFilials")]
        public async Task<ActionResult<IEnumerable<Filial>>> GetAllFilials()
        {
            return await _context.Filials.ToListAsync();
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
    }
}
