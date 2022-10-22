using KursovoiProject_ElShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace KursovoiProject_ElShop.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkersController : ControllerBase
    {
        private readonly ElShopContext _context;
        public WorkersController(ElShopContext context)
        {
            _context = context;
        }

        public static string GetRandomPassword(int length)
        {
            const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            StringBuilder sb = new StringBuilder();
            Random rnd = new Random();

            for (int i = 0; i < length; i++)
            {
                int index = rnd.Next(chars.Length);
                sb.Append(chars[index]);
            }
            return sb.ToString();
        }

        [HttpDelete("{id}/{prikaz}")]
        public async Task<IActionResult> DeleteCadri(int id, string? prikaz)
        {
            if (prikaz == null)
                prikaz = "Не указан";
            else if (prikaz.Trim() == "")
                prikaz = "Не указан";

            var a = _context.Workers.Include(p=>p.User).Where(p=>p.IdWorker == id).First();
            a.User.IsAvalible = 0;
            _context.Users.Update(a.User);
            a.PrikazObUvolnenii = prikaz;
            _context.Workers.Update(a);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("RecoveryCadri/{id}")]
        public async Task<IActionResult> RecoveryCadri(int id)
        {
            var a = _context.Workers.Include(p => p.User).Where(p => p.IdWorker == id).First();
            a.User.IsAvalible = 1;
            _context.Users.Update(a.User);
            a.PrikazObUvolnenii = null;
            _context.Workers.Update(a);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("BlockAccount/{id}")]
        public async Task<IActionResult> BlockCadri(int id)
        {
            var a = _context.Workers.Include(p => p.User).Where(p => p.IdWorker == id).First();
            a.User.IsAvalible = 0;
            _context.Users.Update(a.User);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("UnBlockAccount/{id}")]
        public async Task<IActionResult> UnBlockCadri(int id)
        {
            var a = _context.Workers.Include(p => p.User).Where(p => p.IdWorker == id).First();
            a.User.IsAvalible = 1;
            _context.Users.Update(a.User);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("GetByID/{id}")]
        public async Task<ActionResult<InfoCadri>> GetByID(int id)
        {
            InfoCadri h = new InfoCadri();
            var a = _context.Workers.Include(p => p.User).Where(p => p.IdWorker == id).First();
            h.Familia = a.User.FirstName;
            h.Ima = a.User.LastName;
            return h;
        }

        [HttpGet("GetByIDWorker/{id}")]
        public async Task<ActionResult<Worker>> GetByIDWorker(int id)
        {
            var a = _context.Workers.Include(p => p.User).Where(p => p.IdWorker == id).First();
            return a;
        }

        // GET: api/Workers
        [HttpGet("{uval}")]
        public async Task<ActionResult<IEnumerable<InfoCadri>>> GetCadri(int uval)
        {
            List<Worker> list = new List<Worker>();
            if(uval == 0)
                list = _context.Workers.Include(p => p.User).Where(p=>p.PrikazObUvolnenii == null).ToList();
            else
                list = _context.Workers.Include(p => p.User).Where(p => p.PrikazObUvolnenii != null).ToList();
            var list_filial = _context.WorkersFilials.Include(p => p.Filial).ToList();
            var roles_list = _context.UsersRoles.Include(p => p.User).Include(p=>p.Role).ToList();
            List<InfoCadri> result = new List<InfoCadri>();
            foreach(var a in list)
            {
                InfoCadri h = new InfoCadri();
                h.Familia = a.User.FirstName;
                h.Ima = a.User.LastName;
                h.phoneNumber = a.User.PhoneNumber;
                h.PrikazOPrieme = a.PrikazOprieme;
                if(a.User.IsAvalible == 1)
                    h.avail = true;
                else
                    h.avail = false;
                h.ID_Worker = a.IdWorker;
                TimeOnly timeOnly = new TimeOnly(10, 10, 10);
                h.DateOfBirth = a.DateOfBirth.ToShortDateString();
                h.PrikazObUval = a.PrikazObUvolnenii;
                h.listFilial = new List<Filial>();
                h.listRoles = new List<Role>();
                var filialsSotr = list_filial.Where(p=>p.UserId == a.UserId).ToList();
                foreach(var filial in filialsSotr)
                {
                    Filial f = new Filial();
                    f.IdFilial = filial.Filial.IdFilial;
                    f.NameFilial = filial.Filial.NameFilial;
                    h.listFilial.Add(f);
                }
                if(h.listFilial.Count() == 0)
                {
                    Filial f = new Filial();
                    f.IdFilial = -1;
                    f.NameFilial = "Не указан";
                    h.listFilial.Add(f);
                }
                var rolesSotr = roles_list.Where(p=>p.User.IdUser == a.UserId).ToList();
                foreach(var role in rolesSotr)
                {
                    if (role.RoleId != 21 && role.RoleId != 22)
                    {
                        Role f = new Role();
                        f.IdRole = role.RoleId;
                        f.NameRole = role.Role.NameRole;
                        h.listRoles.Add(f);
                    }
                }    
                result.Add(h);
            }    
            return result;
        }

        [HttpGet("CheckExistWorkerInn/{id}/{inn}")]
        public bool CheckExistWorkerInn(int id, string inn)
        {
            bool result = false;
            if(id == 0)
            {
                if (_context.Workers.Where(p => p.Innworker == inn).Count() > 0)
                    result = false;
                else
                    result = true;
            }
            else
            {
                if (_context.Workers.Where(p => p.Innworker == inn && p.IdWorker != id).Count() > 0)
                    result = false;
                else
                    result = true;
            }
            return result;
        }

        [HttpGet("CheckExistWorkerSnils/{id}/{snils}")]
        public bool CheckExistWorkerSnils(int id, string snils)
        {
            bool result = false;
            if (id == 0)
            {
                if (_context.Workers.Where(p => p.Snils == snils).Count() > 0)
                    result = false;
                else
                    result = true;
            }
            else
            {
                if (_context.Workers.Where(p => p.Snils == snils && p.IdWorker != id).Count() > 0)
                    result = false;
                else
                    result = true;
            }
            return result;
        }

        [HttpGet("CheckExistWorkerPassport/{id}/{serpasp}/{nompasp}")]
        public bool CheckExistWorkerSnils(int id, string serpasp, string nompasp)
        {
            bool result = false;
            if (id == 0)
            {
                if (_context.Workers.Where(p => p.SeriaPasporta == serpasp && p.NomerPasporta == nompasp).Count() > 0)
                    result = false;
                else
                    result = true;
            }
            else
            {
                if (_context.Workers.Where(p => p.SeriaPasporta == serpasp && p.NomerPasporta == nompasp && p.IdWorker != id).Count() > 0)
                    result = false;
                else
                    result = true;
            }
            return result;
        }
    }
}
