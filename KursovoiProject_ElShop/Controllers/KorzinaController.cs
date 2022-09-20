using KursovoiProject_ElShop.Controllers.Validation;
using KursovoiProject_ElShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

namespace KursovoiProject_ElShop.Controllers
{
    public class KorzinaController : Controller
    {
        private readonly ElShopContext _context;
        HttpClient client = new HttpClient();
        public KorzinaController(ElShopContext context)
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

        [ActionName("AddToCart")]
        public async Task<IActionResult> AddToCart(int id)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            var response = await client.GetAsync(@$"api/Korzina/AddToCart/{_context.Users.Where(p => p.Login == getEmail()).First().IdUser}/{id}");
            return RedirectToAction("Index", "Korzina");
        }
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            TovariInCart model = new TovariInCart();
            model.Right1Adds = (List<AddsSite>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/AddsSites/GetAddsSiteType/2").Result.Content.ReadAsStringAsync().Result, typeof(List<AddsSite>));
            model.Right2Adds = (List<AddsSite>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/AddsSites/GetAddsSiteType/3").Result.Content.ReadAsStringAsync().Result, typeof(List<AddsSite>));
            return View(model);
        }

        public async Task<IActionResult> GetCart()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            TovariInCart model = new TovariInCart();
            model.list_tovari = (List<Korzina>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Korzina/GetCarts/{_context.Users.Where(p => p.Login == getEmail()).First().IdUser}").Result.Content.ReadAsStringAsync().Result, typeof(List<Korzina>));
            foreach (var a in model.list_tovari)
            {
                a.Goods = _context.Goods.Where(p => p.IdGood == a.GoodsId).First();
                a.Goods.Manufacture = _context.Manufactures.Where(p => p.IdManufacture == a.Goods.ManufactureId).First();
                if (a.Goods.FtppathImage == null)
                    a.Goods.FtppathImage = BaseAddresse.Server + "TovariImages/NoImageTovar.jpg";
                else
                    a.Goods.FtppathImage = BaseAddresse.Server + "TovariImages/" + a.Goods.FtppathImage;
            }
            var list_f = (List<Filial>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Filials/GetFilials").Result.Content.ReadAsStringAsync().Result, typeof(List<Filial>));
            model.list_filial = new List<FilialsWithBoll>();
            foreach (var f in list_f)
            {
                FilialsWithBoll h = new FilialsWithBoll();
                h.IdFilial = f.IdFilial;
                h.AddressFilial = f.AddressFilial;
                h.NameFilial = f.NameFilial;
                h.Availeble = f.Availeble;
                model.list_filial.Add(h);
            }
            return PartialView("~/Views/Categori/_Cart.cshtml",model);
        }
    }
}
