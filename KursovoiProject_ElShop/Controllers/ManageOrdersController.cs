using KursovoiProject_ElShop.Controllers.Validation;
using KursovoiProject_ElShop.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace KursovoiProject_ElShop.Controllers
{
    //Статусы заказов:
    //0 - Создан. Ожидает подтверждения
    //1 - Подтверждён. Сборка
    //2 - Ожидает выдачи
    //3 - Завершён. Выдан покупателю
    //4 - Завершён. Отменён
    ////////5 - Полный возврат
    ////////6 - Частичный возврат
    public class ManageOrdersController : Controller
    {
        private readonly ElShopContext _context;
        HttpClient client = new HttpClient();
        public ManageOrdersController(ElShopContext context)
        {
            _context = context;
            client.BaseAddress = new Uri(BaseAddresse.Address);
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
        public bool CheckAuthentication()
        {
            if (!User.Identity.IsAuthenticated)
                return false;
            if (User.IsInRole("Сотрудник склада магазина") || User.IsInRole("Сотрудник отдела продаж") || User.IsInRole("Администратор"))
                return true;
            else
                return false;
        }
        public IActionResult Index()
        {
            IdexPageManageOrders model = new IdexPageManageOrders();
            model.filialsWork = (List<Filial>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Filials/GetFilialsWhereWork/{getEmail()}").Result.Content.ReadAsStringAsync().Result, typeof(List<Filial>));
            model.filialsAll = (List<Filial>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Filials/GetAllFilials").Result.Content.ReadAsStringAsync().Result, typeof(List<Filial>));
            //model.HowMuch_Status0 = (int)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Order/GetCountWithStatus0").Result.Content.ReadAsStringAsync().Result, typeof(int));
            return View(model);
        }
        public async Task<IActionResult> GetAllOrders(string? search, int? typesort, int? filial, int? statusOrder, int page = 1)
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");

            IdexPageManageOrders model = new IdexPageManageOrders();

            int pageSize = 15;
            var listOrders = (List<OderInformationForIndexPage>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Order/GetOrders").Result.Content.ReadAsStringAsync().Result, typeof(List<OderInformationForIndexPage>));

            IQueryable<OderInformationForIndexPage> Blocks = listOrders.AsQueryable();
            if (search != null)
            {
                if (search.Trim() != "")
                    Blocks = Blocks.Where(p => p.ID_Order.ToString().Contains(search.Trim().ToLower()) || p.PhoneNumber.Contains(search.Trim().ToLower()) || p.FI.ToLower().Contains(search.Trim().ToLower()));
            }
            if(statusOrder != 3 && statusOrder != 4)
                Blocks = Blocks.Where(p => p.StatusOrder == statusOrder && p.Filial_ID == filial);
            else
                Blocks = Blocks.Where(p => (p.StatusOrder == 3 || p.StatusOrder == 4) && p.Filial_ID == filial);

            Blocks = Blocks.OrderBy(p => p.DateOrder);
            if(typesort == 1)
                Blocks = Blocks.OrderByDescending(p => p.DateOrder);
            if (typesort == 2)
                Blocks = Blocks.OrderBy(p => Convert.ToDouble(p.SummaOrder));
            if (typesort == 3)
                Blocks = Blocks.OrderByDescending(p => Convert.ToDouble(p.SummaOrder));

            var items = Blocks.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var count = Blocks.Count();
            model.listOrders = items;
            model.PageViewModel = new PageViewModel(count, page, pageSize);
            model.FilterViewModel = new FilterViewModel_ManageOrders(search, typesort, filial, statusOrder);
            return PartialView("~/Views/ManageOrders/_ListOrders.cshtml", model);
        }
    }
}
