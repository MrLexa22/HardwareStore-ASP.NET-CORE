using KursovoiProject_ElShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    }
}
