using KursovoiProject_ElShop.Controllers.Validation;
using KursovoiProject_ElShop.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KursovoiProject_ElShop.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly ElShopContext _context;
        HttpClient client = new HttpClient();
        public StatisticsController(ElShopContext context)
        {
            _context = context;
            client.BaseAddress = new Uri(BaseAddresse.Address);
        }
        public bool CheckAuthentication()
        {
            if (!User.Identity.IsAuthenticated)
                return false;
            if (User.IsInRole("Администратор") || User.IsInRole("Сотрудник отдела продаж") || User.IsInRole("Рекламщик"))
                return true;
            else
                return false;
        }
        public IActionResult Index()
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");
            StatisticsModels model = new StatisticsModels();
            model.list = (List<StatModelFilial>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Filials/GetFilialsStatistick").Result.Content.ReadAsStringAsync().Result, typeof(List<StatModelFilial>));
            return View(model);
        }
    }
}
