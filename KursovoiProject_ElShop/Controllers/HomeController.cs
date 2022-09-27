using KursovoiProject_ElShop.API;
using KursovoiProject_ElShop.Controllers.Validation;
using KursovoiProject_ElShop.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace KursovoiProject_ElShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ElShopContext _context;
        HttpClient client = new HttpClient();
        UsersController usersController;
        public HomeController(ILogger<HomeController> logger, ElShopContext context)
        {
            _logger = logger;
            _context = context;
            usersController = new UsersController(_context);
            client.BaseAddress = new Uri(BaseAddresse.Address);
        }

        public async Task<IActionResult> IndexAsync()
        {
            IndexPageModel model = new IndexPageModel();

            model.MainAdds = (List<AddsSite>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/AddsSites/GetAddsSiteType/1").Result.Content.ReadAsStringAsync().Result, typeof(List<AddsSite>));
            model.Right1Adds = (List<AddsSite>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/AddsSites/GetAddsSiteType/2").Result.Content.ReadAsStringAsync().Result, typeof(List<AddsSite>));
            model.Right2Adds = (List<AddsSite>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/AddsSites/GetAddsSiteType/3").Result.Content.ReadAsStringAsync().Result, typeof(List<AddsSite>));
            model.Categories = (List<Category>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Categories/").Result.Content.ReadAsStringAsync().Result, typeof(List<Category>));
            model.goodsRandom = (List<ViewGoodsWithManufacture>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/ViewGoodsWithManufactures/Random5").Result.Content.ReadAsStringAsync().Result, typeof(List<ViewGoodsWithManufacture>));

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Application");
            return RedirectToAction("Index", "Home");
        }

        public async Task AuthenticationAsync(int id)
        {
            User userDB = _context.Users.Find(id);
            var claimsIdentity = new ClaimsIdentity("Application");
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, userDB.FirstName + " " + userDB.LastName));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, userDB.Login));
            var rolesUser = _context.UsersRoles.Where(p=>p.UserId == id).ToList();
            if (rolesUser.Where(p=>p.RoleId==22).Count() >0)
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Покупатель"));

            await HttpContext.SignInAsync("Application", new ClaimsPrincipal(claimsIdentity));
        }

        [HttpGet]
        [ActionName("AuthenticateUser")]
        public async Task<IActionResult> Authen()
        {
            ModelState.Clear();
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AuthenticateUser(AuthenticationUser model)
        {
            var user = _context.Users.Where(p => p.Login == model.Email && p.Password == HashPassword.hashPassword(model.Password));
            if (user.Count() > 0)
            {
                await AuthenticationAsync(user.First().IdUser);
                ModelState.Clear();
                return RedirectToAction("Index", "Home");
            }
            else
                ModelState.AddModelError("", "");
            return PartialView("AuthenticateUser", model);
        }

        [HttpGet]
        [ActionName("RegistrateUser")]
        public async Task<IActionResult> Reg()
        {
            ModelState.Clear();
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrateUser(RegistrateUsers model)
        {
            if(!ModelState.IsValid)
            {
                return PartialView();
            }

            if (!usersController.CheckPhone(0,model.Telefon) || !usersController.CheckEmail(0, model.Email))
            {
                ModelState.AddModelError("", "");
                return PartialView();
            }

            User p = new User();
            p.Login = model.Email;
            p.FirstName = model.Familia;
            p.LastName = model.Ima;
            p.PhoneNumber = model.Telefon;
            p.Password = HashPassword.hashPassword(model.Password);
            p.IsAvalible = 1;

            string json = JsonConvert.SerializeObject(p);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponseMessage = client.PostAsync(@$"api/Users/", content).Result;
            p = (User)JsonConvert.DeserializeObject(httpResponseMessage.Content.ReadAsStringAsync().Result, typeof(User));
            await AuthenticationAsync(p.IdUser);

            return RedirectToAction("Index", "Home");
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

        public async Task<IActionResult> MyOrders()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            OrdersUser model = new OrdersUser();
            model.Right1Adds = (List<AddsSite>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/AddsSites/GetAddsSiteType/2").Result.Content.ReadAsStringAsync().Result, typeof(List<AddsSite>));
            model.Right2Adds = (List<AddsSite>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/AddsSites/GetAddsSiteType/3").Result.Content.ReadAsStringAsync().Result, typeof(List<AddsSite>));
            return View(model);
        }

        public async Task<IActionResult> nowOrders()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            OrdersUser model = new OrdersUser();
            User user = _context.Users.Where(p => p.Login == getEmail()).First();
            model.Orders = _context.ViewOrdersClients.Where(p => p.UserId == user.IdUser && p.Status != 3 && p.Status != 4 && p.Status != 5 && p.Status != 6).ToList();
            return PartialView("~/Views/Home/_nowOrdersClient.cshtml", model);
        }

        public async Task<IActionResult> oldOrders()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            OrdersUser model = new OrdersUser();
            User user = _context.Users.Where(p => p.Login == getEmail()).First();
            model.Orders = _context.ViewOrdersClients.Where(p => p.UserId == user.IdUser && p.Status != 0 && p.Status != 1 && p.Status != 2).ToList();
            return PartialView("~/Views/Home/_nowOrdersClient.cshtml", model);
        }
    }
}