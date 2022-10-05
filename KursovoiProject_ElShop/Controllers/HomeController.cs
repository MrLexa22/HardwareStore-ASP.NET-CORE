using KursovoiProject_ElShop.API;
using KursovoiProject_ElShop.Controllers.Validation;
using KursovoiProject_ElShop.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mail;
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
            var user = _context.Users.Where(p => p.Login == model.Email && p.Password == HashPassword.hashPassword(model.Password) && p.IsAvalible == 1);
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

        public async Task<IActionResult> ActivateAccount()
        {
            return PartialView("~/Views/Home/_ActivateAccount1.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateAccountPost(ActivateAccount1 model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("~/Views/Home/_ActivateAccount1.cshtml",model);
            }
            Random rnd = new Random();
            int code = rnd.Next(10000, 99999);
            var user = _context.Users.Where(p => p.Login == model.Email && p.IsAvalible == 0).ToList();
            if(user.Count() <= 0)
            {
                ModelState.AddModelError("", "");
            }
            if (user.First().UsersRoles.Count() > 1)
            {
                ModelState.AddModelError("", "");
            }
            else
            {
                Response.Cookies.Append("Code", HashPassword.hashPassword(code.ToString()));
                _ = EmailService.SendEmailActivation(code, model.Email);
            }
            return PartialView("~/Views/Home/_ActivateAccount2.cshtml");
        }

        public async Task<IActionResult> ActivateAccount2(string Email)
        {
            ActivateAccount1 model = new ActivateAccount1();
            model.Email = Email;
            return PartialView("~/Views/Home/_ActivateAccount2.cshtml",model);
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

        [HttpPost]
        public async Task<IActionResult> ActivateAccount2Post(ActivateAccount1 model)
        {
            if (HashPassword.hashPassword(model.Code) != Request.Cookies["Code"])
            {
                ModelState.AddModelError("", "");
                return PartialView("~/Views/Home/_ActivateAccount2.cshtml", model);
            }
            var user = _context.Users.Where(p => p.Login == model.Email).First();
            var pas = GetRandomPassword(8);
            user.Password = HashPassword.hashPassword(pas);
            user.IsAvalible = 1;
            _context.Update(user);
            _context.SaveChanges();
            Response.Cookies.Delete("Code");
            EmailService.SendEmailWithValuesForEnter(pas, model.Email);
            return PartialView("~/Views/Home/_ActivateAccount2.cshtml", model);
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
            if (!usersController.CheckPhone(0, model.Telefon))
            {
                ModelState.AddModelError("Telefon", "Данный номер телефона уже есть в базе");
                return PartialView(model);
            }
            if (!usersController.CheckEmail(0, model.Email))
            {
                ModelState.AddModelError("Email", "Email уже используется");
                return PartialView(model);
            }
            if (!ModelState.IsValid)
            {
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
            EmailService.SendEmailRegistrationAsync(p.Login, model.Password);
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

        public async Task<IActionResult> OrderUser(int id)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            User us = _context.Users.Include(p => p.OrderUsers).Where(p => p.Login == getEmail()).First();
            if (us.OrderUsers.Where(p=>p.OrderNumber == id).Count() <= 0)
                return RedirectToAction("Index", "Home");
            ModelOrder model = new ModelOrder();
            model.Right1Adds = (List<AddsSite>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/AddsSites/GetAddsSiteType/2").Result.Content.ReadAsStringAsync().Result, typeof(List<AddsSite>));
            model.Right2Adds = (List<AddsSite>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/AddsSites/GetAddsSiteType/3").Result.Content.ReadAsStringAsync().Result, typeof(List<AddsSite>));
            Order order = _context.Orders.Include(p => p.Filial).Where(p => p.OrderNumber == id).First();
            
            model.contacts = new ContactInformation();
            model.contacts.Contact_Telefon = order.ContactTelefon;
            model.contacts.Contact_Name = order.ContactName;
            model.contacts.Contact_Email = order.ContactEmail;
            model.OrderInformation = new Order();
            model.OrderInformation.OrderNumber = order.OrderNumber;
            model.OrderInformation.Status = order.Status;
            model.OrderInformation.Filial = order.Filial;
            model.OrderInformation.DateOrder = order.DateOrder;
            if (order.SborshikUserId != null)
                model.OrderInformation.SborshikUser = _context.Users.Find(order.SborshikUserId);
            if (order.SellerUserId != null)
                model.OrderInformation.SborshikUser = _context.Users.Find(order.SellerUserId);
            if (order.DateReadyToExtradition != null)
                model.OrderInformation.DateReadyToExtradition = order.DateReadyToExtradition;
            if (order.DateExtradition != null)
                model.OrderInformation.DateExtradition = order.DateExtradition;
            order.OrderContainers = _context.OrderContainers.Include(p => p.Goods).Include(p => p.Goods.Manufacture).Where(p=>p.OrderNumber == order).ToList();

            foreach (var a in order.OrderContainers)
            {
                if (a.Goods.FtppathImage == null)
                    a.Goods.FtppathImage = BaseAddresse.Server + "TovariImages/NoImageTovar.jpg";
                else
                    a.Goods.FtppathImage = BaseAddresse.Server + "TovariImages/" + a.Goods.FtppathImage;
                model.summa += Convert.ToDouble(a.Price)*a.Count;
            }
            model.OrderInformation.OrderContainers = order.OrderContainers;

            return View(model);
        }

        public async Task<IActionResult> CancelOrder(int id)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            User us = _context.Users.Include(p => p.OrderUsers).Where(p => p.Login == getEmail()).First();
            if (us.OrderUsers.Where(p => p.OrderNumber == id).Count() <= 0)
                return RedirectToAction("Index", "Home");

            Order order = _context.Orders.Include(p => p.Filial).Where(p => p.OrderNumber == id).First();
            order.OrderContainers = _context.OrderContainers.Include(p => p.Goods).Include(p => p.Goods.Manufacture).Where(p => p.OrderNumber == order).ToList();
            foreach(var a in order.OrderContainers)
            {
                var b = _context.GoodsFilials.Where(p => p.FilialId == order.FilialId && p.GoodsId == a.GoodsId).First();
                b.CountSklad += a.Count;
                _context.Update(b);
            }
            order.Status = 4;
            order.DateExtradition = DateTime.Now;
            _context.Update(order);
            _context.SaveChanges();

            return RedirectToAction("OrderUser","Home",new {id = id});
        }

        public async Task<IActionResult> MyProfile()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            User us = _context.Users.Where(p => p.Login == getEmail()).First();
            var roles = _context.UsersRoles.Include(p => p.Role).Where(p => p.UserId == us.IdUser).ToList();
            MyProfile model = new MyProfile();
            model.Roles = roles;
            model.Right1Adds = (List<AddsSite>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/AddsSites/GetAddsSiteType/2").Result.Content.ReadAsStringAsync().Result, typeof(List<AddsSite>));
            model.Right2Adds = (List<AddsSite>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/AddsSites/GetAddsSiteType/3").Result.Content.ReadAsStringAsync().Result, typeof(List<AddsSite>));
            model.user = us;
            return View(model);
        }

        public async Task<IActionResult> ChangePassword()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return PartialView("~/Views/Home/_ChangePasswordModal.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePasswordPost(ChangePassword model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("~/Views/Home/_ChangePasswordModal.cshtml",model);
            }
            var user = _context.Users.Where(p => p.Login == getEmail() && p.Password == HashPassword.hashPassword(model.Old_password));
            if (user.Count() <= 0)
            {
                ModelState.AddModelError("", "");
                return PartialView("~/Views/Home/_ChangePasswordModal.cshtml", model);
            }
            var userf = user.First();
            userf.Password = HashPassword.hashPassword(model.New_password);
            _context.Update(userf);
            _context.SaveChanges();
            return RedirectToAction("MyProfile", "Home");
        }

        public async Task<IActionResult> ChangeProfileInfo()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            User us = _context.Users.Where(p => p.Login == getEmail()).First();
            ChangeInformation model = new ChangeInformation();
            model.Telefon = us.PhoneNumber;
            model.Ima = us.LastName;
            model.Familia = us.FirstName;
            model.Email = us.Login;
            model.Id = us.IdUser;
            return PartialView("~/Views/Home/_ChangeInformationProfile.cshtml",model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeProfileInfoPost(ChangeInformation model)
        {
            User us = _context.Users.Where(p => p.Login == getEmail()).First();
            if (!usersController.CheckPhone(model.Id, model.Telefon))
            {
                ModelState.AddModelError("Telefon", "Данный номер телефона уже есть в базе");
                return PartialView("~/Views/Home/_ChangeInformationProfile.cshtml", model);
            }
            if (!usersController.CheckEmail(model.Id, model.Email))
            {
                ModelState.AddModelError("Email", "Email уже используется");
                return PartialView("~/Views/Home/_ChangeInformationProfile.cshtml", model);
            }
            if (!ModelState.IsValid)
            {
                return PartialView("~/Views/Home/_ChangeInformationProfile.cshtml", model);
            }
            us.PhoneNumber = model.Telefon;
            us.FirstName = model.Familia;
            us.LastName = model.Ima;
            us.Login = model.Email;
            _context.Update(us);
            _context.SaveChanges();
            return RedirectToAction("MyProfile", "Home");
        }

        public async Task<IActionResult> CancelAccount()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return PartialView("~/Views/Home/_CancelAccount.cshtml");
        }

        public async Task<IActionResult> CancelAccountPost()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            User us = _context.Users.Include(p=>p.UsersRoles).Where(p => p.Login == getEmail()).First();
            if(us.UsersRoles.Count() == 1)
            {
                us.IsAvalible = 0;
                _context.Update(us);
                _context.SaveChanges();
                await HttpContext.SignOutAsync("Application");
            }
            return RedirectToAction("Index", "Home"); ;
        }
    }
}