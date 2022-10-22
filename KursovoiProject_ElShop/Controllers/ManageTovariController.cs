using KursovoiProject_ElShop.Controllers.Validation;
using KursovoiProject_ElShop.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace KursovoiProject_ElShop.Controllers
{
    public class ManageTovariController : Controller
    {
        private readonly ElShopContext _context;
        HttpClient client = new HttpClient();
        public ManageTovariController(ElShopContext context)
        {
            _context = context;
            client.BaseAddress = new Uri(BaseAddresse.Address);
        }

        public bool CheckAuthentication()
        {
            if (!User.Identity.IsAuthenticated)
                return false;
            if (User.IsInRole("Сотрудник склада магазина") || User.IsInRole("Администратор"))
                return true;
            else
                return false;
        }
        public IActionResult Index()
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");
            IndexManageTovari model = new IndexManageTovari();
            model.manufactures = (List<Manufacture>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Manufactures/GetManufacturesAll").Result.Content.ReadAsStringAsync().Result, typeof(List<Manufacture>));
            model.categories = (List<Category>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Categories").Result.Content.ReadAsStringAsync().Result, typeof(List<Category>));
            return View(model);
        }

        public string getEmail()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            int i = 0;
            string email = "";
            foreach (var a in claims)
            {
                if (i == 1)
                    email = a.Value;
                i++;
            }
            return email;
        }

        public async Task<IActionResult> GetAllTovars(string? search, int? typesort, int? category, int? proizvoditel, int? visibal, int page = 1)
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");

            IndexManageTovari model = new IndexManageTovari();

            int pageSize = 15;
            var listTovari = (List<ListTovariManageTovari>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Tovars/{getEmail()}").Result.Content.ReadAsStringAsync().Result, typeof(List<ListTovariManageTovari>));

            IQueryable<ListTovariManageTovari> Blocks = listTovari.AsQueryable();

            if (search != null)
            {
                if (search.Trim() != "")
                    Blocks = Blocks.Where(p => p.NameGood.ToLower().Contains(search.Trim().ToLower()) || p.ID_Good.ToString() == search);
            }

            if (typesort == 1)
                Blocks = Blocks.OrderBy(p => p.NameGood);

            if (typesort == 2)
                Blocks = Blocks.OrderByDescending(p => p.Cost);
            if (typesort == 3)
                Blocks = Blocks.OrderBy(p => p.Cost);

            if (category > 0)
                Blocks = Blocks.Where(p => p.ID_Category == category);
            if(proizvoditel > 0)
                Blocks = Blocks.Where(p => p.ManufactID == proizvoditel);

            if(visibal == 1)
                Blocks = Blocks.Where(p => p.IsAvalib == true);
            if (visibal == 2)
                Blocks = Blocks.Where(p => p.IsAvalib == false);

            var items = Blocks.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var count = Blocks.Count();
            model.tovari = items;
            model.PageViewModel = new PageViewModel(count, page, pageSize);
            model.FilterViewModel_ManageTovari = new FilterViewModel_ManageTovari(search, typesort, category, proizvoditel,visibal);
            return PartialView("~/Views/ManageTovari/_ListTovari.cshtml", model);
        }

        public async Task<IActionResult> ConfirmDelete(int id)
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");
            ListTovariManageTovari model = new ListTovariManageTovari();
            model.ID_Good = id;
            var a = (ListTovariManageTovari)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Tovars/GetToDelete/{id}").Result.Content.ReadAsStringAsync().Result, typeof(ListTovariManageTovari));
            model.NameGood = a.NameGood;
            return PartialView("~/Views/ManageTovari/_ConfirmDelete.cshtml", model);
        }

        public async Task<IActionResult> ConfirmReturn(int id)
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");
            ListTovariManageTovari model = new ListTovariManageTovari();
            model.ID_Good = id;
            var a = (ListTovariManageTovari)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Tovars/GetToDelete/{id}").Result.Content.ReadAsStringAsync().Result, typeof(ListTovariManageTovari));
            model.NameGood = a.NameGood;
            return PartialView("~/Views/ManageTovari/_ConfirmReturn.cshtml", model);
        }
    }
}
