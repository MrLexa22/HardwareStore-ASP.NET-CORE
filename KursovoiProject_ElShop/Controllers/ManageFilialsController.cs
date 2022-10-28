using KursovoiProject_ElShop.Controllers.Validation;
using KursovoiProject_ElShop.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KursovoiProject_ElShop.Controllers
{
    public class ManageFilialsController : Controller
    {
        private readonly ElShopContext _context;
        HttpClient client = new HttpClient();
        public ManageFilialsController(ElShopContext context)
        {
            _context = context;
            client.BaseAddress = new Uri(BaseAddresse.Address);
        }

        public bool CheckAuthentication()
        {
            if (!User.Identity.IsAuthenticated)
                return false;
            if (User.IsInRole("Администратор"))
                return true;
            else
                return false;
        }
        public IActionResult Index()
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");

            return View();
        }
        public async Task<IActionResult> GetAllFilials(string? search, int? typesort, int? type, int page = 1)
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");

            IndexManageFilials model = new IndexManageFilials();

            int pageSize = 15;
            var listFilials = (List<Filial>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Filials/GetAllFilials").Result.Content.ReadAsStringAsync().Result, typeof(List<Filial>));

            IQueryable<Filial> Blocks = listFilials.AsQueryable();

            if (search != null)
            {
                if (search.Trim() != "")
                    Blocks = Blocks.Where(p => p.NameFilial.ToLower().Contains(search.Trim().ToLower()) || p.AddressFilial.ToLower().Contains(search.Trim().ToLower()));
            }

            if (typesort == 1)
                Blocks = Blocks.OrderBy(p => p.NameFilial);

            if (type == 1)
                Blocks = Blocks.Where(p => p.Availeble == true);
            if (type == 2)
                Blocks = Blocks.Where(p => p.Availeble == false);

            var items = Blocks.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var count = Blocks.Count();
            model.filials = items;
            model.PageViewModel = new PageViewModel(count, page, pageSize);
            model.FilterViewModel = new FilterViewModel_AddsSites(search, typesort, type, null);
            return PartialView("~/Views/ManageFilials/_ListFilials.cshtml", model);
        }
    }
}
