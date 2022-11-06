using KursovoiProject_ElShop.Controllers.Validation;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace KursovoiProject_ElShop.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly ElShopContext _context;
        HttpClient client = new HttpClient();
        public AdministrationController(ElShopContext context)
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
    }
}
