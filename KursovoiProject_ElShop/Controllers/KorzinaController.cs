using KursovoiProject_ElShop.Controllers.Validation;
using KursovoiProject_ElShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
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

        public async Task<IActionResult> GetCartTovari()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            TovariInCart model = new TovariInCart();
            model.list_tovari = (List<Korzina>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Korzina/GetCarts/{_context.Users.Where(p => p.Login == getEmail()).First().IdUser}").Result.Content.ReadAsStringAsync().Result, typeof(List<Korzina>));
            double summa = 0;
            foreach (var a in model.list_tovari)
            {
                a.Goods = _context.Goods.Where(p => p.IdGood == a.GoodsId).First();
                summa += a.Goods.Cost * a.Count;
                a.Goods.Manufacture = _context.Manufactures.Where(p => p.IdManufacture == a.Goods.ManufactureId).First();
                if (a.Goods.FtppathImage == null)
                    a.Goods.FtppathImage = BaseAddresse.Server + "TovariImages/NoImageTovar.jpg";
                else
                    a.Goods.FtppathImage = BaseAddresse.Server + "TovariImages/" + a.Goods.FtppathImage;
            }
            model.summa = summa;
            model.IDUser = _context.Users.Where(p => p.Login == getEmail()).First().IdUser;
            return PartialView("~/Views/Korzina/_CartTovari.cshtml", model);
        }

        public async Task<IActionResult> GetCart()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            TovariInCart model = new TovariInCart();
            model.list_tovari = (List<Korzina>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Korzina/GetCarts/{_context.Users.Where(p => p.Login == getEmail()).First().IdUser}").Result.Content.ReadAsStringAsync().Result, typeof(List<Korzina>));
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
            model.contacts = new ContactInformation();
            var user = (User)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Users/email/{getEmail()}").Result.Content.ReadAsStringAsync().Result, typeof(User));
            model.contacts.Contact_Name = user.FirstName + " " + user.LastName;
            model.contacts.Contact_Telefon = user.PhoneNumber;
            model.contacts.Contact_Email = user.Login;
            return PartialView("~/Views/Korzina/_Cart.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Oformlenie(TovariInCart list)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            informationPreOformleine model = new informationPreOformleine();
            var list_tovari = (List<Korzina>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Korzina/GetCarts/{_context.Users.Where(p => p.Login == getEmail()).First().IdUser}").Result.Content.ReadAsStringAsync().Result, typeof(List<Korzina>));
            double summa = 0;
            foreach (var a in list_tovari)
            {
                a.Goods = _context.Goods.Where(p => p.IdGood == a.GoodsId).First();
                a.Goods.Manufacture = _context.Manufactures.Where(p => p.IdManufacture == a.Goods.ManufactureId).First();
                if (a.Goods.FtppathImage == null)
                    a.Goods.FtppathImage = BaseAddresse.Server + "TovariImages/NoImageTovar.jpg";
                else
                    a.Goods.FtppathImage = BaseAddresse.Server + "TovariImages/" + a.Goods.FtppathImage;
            }
            model.tovaris_false = new List<availibleTovari>();
            model.tovaris_true = new List<availibleTovari>();
            foreach (var a in list_tovari)
            {
                var listTovariInFilial = _context.GoodsFilials.Where(p => p.FilialId == list.SelectedFilial);
                availibleTovari h = new availibleTovari();
                h.GoodsId = a.GoodsId;
                h.Goods = a.Goods;
                h.Count = a.Count;
                h.count_filial = listTovariInFilial.Where(p => p.GoodsId == a.GoodsId).First().CountSklad;
                if (a.Count > h.count_filial)
                {
                    if (h.count_filial != 0)
                    {
                        int false_count = h.Count - h.count_filial;

                        h.Count = h.count_filial;
                        model.tovaris_true.Add(h);
                        model.summa += h.Goods.Cost * h.Count;

                        availibleTovari hh = new availibleTovari();
                        hh.count_filial = h.count_filial;
                        hh.Goods = h.Goods;
                        hh.Count = false_count;
                        model.tovaris_false.Add(hh);
                    }
                    else
                        model.tovaris_false.Add(h);
                }
                else
                {
                    model.tovaris_true.Add(h);
                    model.summa += h.Goods.Cost * h.Count;
                }
            }
            model.Filial = _context.Filials.Where(p => p.IdFilial == list.SelectedFilial).First();
            model.contacts = list.contacts;
            model.Right1Adds = (List<AddsSite>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/AddsSites/GetAddsSiteType/2").Result.Content.ReadAsStringAsync().Result, typeof(List<AddsSite>));
            model.Right2Adds = (List<AddsSite>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/AddsSites/GetAddsSiteType/3").Result.Content.ReadAsStringAsync().Result, typeof(List<AddsSite>));
            return View("CheckOrder", model);
        }

        [HttpPost]
        public async Task<IActionResult> PostOformlenie(informationPreOformleine list)
        {
            var goodsInFilial = _context.GoodsFilials.Where(p => p.FilialId == list.Filial.IdFilial).ToList();
            foreach(var a in list.tovaris_true)
            {
                if (a.Count > goodsInFilial.Where(p => p.GoodsId == a.GoodsId).First().CountSklad)
                    return RedirectToAction("Index", "Korzina");
            }
            Random rnd = new Random();
            int nomerZakaz = 0;
            while (true)
            {
                int value = rnd.Next(10000, 100000);
                string g = list.Filial.IdFilial.ToString() + value.ToString();
                int n = Convert.ToInt32(g);
                if (_context.Orders.Where(p => p.OrderNumber == n).Count() == 0)
                {
                    nomerZakaz = n;
                    break;
                }
            }
            //Статусы заказов:
            //0 - Создан. Ожидает подтверждения
            //1 - Подтверждён. Сборка
            //2 - Ожидает выдачи
            //3 - Завершён. Выдан покупателю
            //4 - Завершён. Отменён
            //5 - Полный возврат
            //6 - Частичный возврат
            Order order = new Order();
            order.OrderNumber = nomerZakaz;
            order.DateOrder = DateTime.Now;
            order.Status = 0;
            order.FilialId = list.Filial.IdFilial;
            order.DateReadyToExtradition = null;
            order.DateExtradition = null;
            order.ItogSumma = Convert.ToDecimal(list.summa);
            int IDUser = _context.Users.Where(p => p.Login == getEmail()).First().IdUser;
            order.UserId = IDUser;
            _context.Orders.Add(order);
            _context.SaveChanges();
            foreach(var a in list.tovaris_true)
            {
                OrderContainer orderContainer = new OrderContainer();
                orderContainer.OrderNumberId = order.OrderNumber;
                orderContainer.GoodsId = a.GoodsId;
                orderContainer.Price = Convert.ToDecimal(a.Goods.Cost);
                orderContainer.Count = a.Count;
                _context.OrderContainers.Add(orderContainer);

                GoodsFilial goodsFilial = _context.GoodsFilials.Where(p => p.FilialId == list.Filial.IdFilial && p.GoodsId == a.GoodsId).First();
                goodsFilial.CountSklad = goodsFilial.CountSklad - a.Count;
                _context.GoodsFilials.Update(goodsFilial);

                Korzina korzina = _context.Korzinas.Where(p => p.GoodsId == a.GoodsId && p.UserId == IDUser).First();
                _context.Korzinas.Remove(korzina);
                
            }
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
    }
}
