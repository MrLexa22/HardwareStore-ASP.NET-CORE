using KursovoiProject_ElShop.Controllers.Validation;
using KursovoiProject_ElShop.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;

namespace KursovoiProject_ElShop.Controllers
{
    public class ManageCadriController : Controller
    {
        private readonly ElShopContext _context;
        HttpClient client = new HttpClient();
        public ManageCadriController(ElShopContext context)
        {
            _context = context;
            client.BaseAddress = new Uri(BaseAddresse.Address);
        }

        public bool CheckAuthentication()
        {
            if (!User.Identity.IsAuthenticated)
                return false;
            if (User.IsInRole("Сотрудник отдела кадров") || User.IsInRole("Администратор"))
                return true;
            else
                return false;
        }

        public IActionResult Index()
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");
            IndexPageCadri model = new IndexPageCadri();
            model.filials = (List<Filial>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Filials/GetAllFilials").Result.Content.ReadAsStringAsync().Result, typeof(List<Filial>));
            return View(model);
        }

        public async Task<IActionResult> GetAllCadri(int uvol, string? search, int? typesort, int? dolzhnost, int? filial, int page = 1)
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");

            IndexPageCadri model = new IndexPageCadri();
            model.statusSite = uvol;
            int pageSize = 15;
            var listAdds = (List<InfoCadri>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Workers/{uvol}").Result.Content.ReadAsStringAsync().Result, typeof(List<InfoCadri>));
            IQueryable<InfoCadri> Blocks = listAdds.AsQueryable();

            if (search != null)
            {
                if (search.Trim() != "")
                    Blocks = Blocks.Where(p => p.Familia.ToLower().Contains(search.Trim().ToLower()) || p.Ima.ToLower().Contains(search.Trim().ToLower()));
            }
            if (typesort == 1)
                Blocks = Blocks.OrderBy(p => p.Familia);
            if (typesort == 2)
                Blocks = Blocks.OrderBy(p => Convert.ToDateTime(p.DateOfBirth));
            if (dolzhnost > 0)
                Blocks = Blocks.Where(p => p.listRoles.Where(g => g.IdRole == dolzhnost).Any());
            if (filial > 0)
                Blocks = Blocks.Where(p => p.listFilial.Where(f=>f.IdFilial == filial).Any());
            if(filial == -1)
                Blocks = Blocks.Where(p => p.listFilial[0].NameFilial == "Не указан");

            var items = Blocks.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var count = Blocks.Count();
            model.listCadri = items;
            model.PageViewModel = new PageViewModel(count, page, pageSize);
            model.FilterViewModel = new FilterViewModel_AddsSites(search, typesort, dolzhnost, filial);
            return PartialView("~/Views/ManageCadri/_ListCadri.cshtml", model);
        }
    }
}
