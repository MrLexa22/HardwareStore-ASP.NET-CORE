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
        public async Task<IActionResult> ConfirmClose(int id)
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");

            Filial model = new Filial();
            model = (Filial)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Filials/GetFilialByID/{id}").Result.Content.ReadAsStringAsync().Result, typeof(Filial));
            return PartialView("~/Views/ManageFilials/_ConfirmClose.cshtml", model);
        }

        public async Task<IActionResult> ConfirmOpen(int id)
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");

            Filial model = new Filial();
            model = (Filial)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Filials/GetFilialByID/{id}").Result.Content.ReadAsStringAsync().Result, typeof(Filial));
            return PartialView("~/Views/ManageFilials/_ConfirmOpen.cshtml", model);
        }

        public async Task<IActionResult> AddEditFilial(int id)
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");

            Filial model = new Filial();
            if(id > 0)
                model = (Filial)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Filials/GetFilialByID/{id}").Result.Content.ReadAsStringAsync().Result, typeof(Filial));
            return PartialView("~/Views/ManageFilials/_AddEditFilial.cshtml", model);
        }
        [HttpPost]
        public async Task<IActionResult> AddEditFilialPost(int id, Filial model)
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");
            model.IdFilial = id;
            if(model.NameFilial == null)
            {
                ModelState.AddModelError("NameFilial", "Некорректное наименование филиала");
                return PartialView("~/Views/ManageFilials/_AddEditFilial.cshtml", model);
            }
            if (model.NameFilial.Trim().Length < 3 || model.NameFilial.Trim().Length > 40)
            {
                ModelState.AddModelError("NameFilial", "Минимум 3 символа. Максимум 40");
                return PartialView("~/Views/ManageFilials/_AddEditFilial.cshtml", model);
            }

            if (model.AddressFilial == null)
            {
                ModelState.AddModelError("AddressFilial", "Некорректный адрес филиала");
                return PartialView("~/Views/ManageFilials/_AddEditFilial.cshtml", model);
            }
            if (model.AddressFilial.Trim().Length < 3 || model.AddressFilial.Trim().Length > 150)
            {
                ModelState.AddModelError("AddressFilial", "Минимум 3 символа. Максимум 150");
                return PartialView("~/Views/ManageFilials/_AddEditFilial.cshtml", model);
            }

            if (model.NameFilial.Contains("/") || model.NameFilial.Contains("\\"))
            {
                ModelState.AddModelError("NameFilial", "/ или \\ недопустимы");
                return PartialView("~/Views/ManageFilials/_AddEditFilial.cshtml", model);
            }
            if (model.AddressFilial.Contains("/") || model.AddressFilial.Contains("\\"))
            {
                ModelState.AddModelError("AddressFilial", "/ или \\ недопустимы");
                return PartialView("~/Views/ManageFilials/_AddEditFilial.cshtml", model);
            }

            if((Boolean)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Filials/CheckExistFilial/{id}/{model.NameFilial}/{model.AddressFilial}").Result.Content.ReadAsStringAsync().Result, typeof(Boolean)) == false)
            {
                ModelState.AddModelError("NameFilial", "Филиал с таким адресом/наименование уже существует!");
                return PartialView("~/Views/ManageFilials/_AddEditFilial.cshtml", model);
            }

            var a = (Filial)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Filials/AddEditFilial/{id}/{model.NameFilial}/{model.AddressFilial}").Result.Content.ReadAsStringAsync().Result, typeof(Filial));

            return PartialView("~/Views/ManageFilials/_AddEditFilial.cshtml", model);
        }
    }
}
