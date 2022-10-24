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

        public async Task<IActionResult> AddEditTovar(int id)
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");

            AddEditTovar model = new AddEditTovar();
            model.ID_Tovar = id;
            if(id > 0)
            {
                var good = (Good)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Tovars/GetTovarByID/{id}").Result.Content.ReadAsStringAsync().Result, typeof(Good));
                model.Cost = good.Cost.ToString();
                model.Description = good.Description;
                model.Name_Tovar = good.Name;
                model.Category_ID = good.CategoriId;
                model.Proizvoditel_ID = good.ManufactureId;
            }
            model.manufactures = (List<Manufacture>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Manufactures/GetManufacturesAll").Result.Content.ReadAsStringAsync().Result, typeof(List<Manufacture>));
            model.categories = (List<Category>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Categories").Result.Content.ReadAsStringAsync().Result, typeof(List<Category>));
            model.Nalichielist = (List<NalichFilialManageTovari>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Tovars/GetFilialsCount/{getEmail()}/{id}").Result.Content.ReadAsStringAsync().Result, typeof(List<NalichFilialManageTovari>));
            return PartialView("~/Views/ManageTovari/_AddEditTovar.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditTovarPost(int id, AddEditTovar model)
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");

            model.ID_Tovar = id;
            model.manufactures = (List<Manufacture>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Manufactures/GetManufacturesAll").Result.Content.ReadAsStringAsync().Result, typeof(List<Manufacture>));
            model.categories = (List<Category>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Categories").Result.Content.ReadAsStringAsync().Result, typeof(List<Category>));
            if (model.Name_Tovar == null)
            {
                ModelState.AddModelError("Name_Tovar", "Некорректное наименование товара");
                return View("~/Views/ManageTovari/_AddEditTovar.cshtml", model);
            }
            else if(model.Name_Tovar.Trim().Length < 3)
            {
                ModelState.AddModelError("Name_Tovar", "Некорректное наименование товара");
                return PartialView("~/Views/ManageTovari/_AddEditTovar.cshtml", model);
            }
            if(model.Name_Tovar != null)
            {
                string withoutslashes = model.Name_Tovar.Replace('/', ' ').Replace('\\',' ');
                string check = (string)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Tovars/CheckExistTovar/{withoutslashes}/{id}").Result.Content.ReadAsStringAsync().Result, typeof(string));
                if(check == "false")
                {
                    ModelState.AddModelError("Name_Tovar", "Такое наименование товара уже существует");
                    return PartialView("~/Views/ManageTovari/_AddEditTovar.cshtml", model);
                }
            }

            if(model.Category_ID == null)
            {
                ModelState.AddModelError("Category_ID", "Некорректная категория товара");
                return PartialView("~/Views/ManageTovari/_AddEditTovar.cshtml", model);
            }
            else if (model.Category_ID <= 0)
            {
                ModelState.AddModelError("Category_ID", "Некорректная категория товара");
                return PartialView("~/Views/ManageTovari/_AddEditTovar.cshtml", model);
            }

            if(model.Proizvoditel_ID == null)
            {
                ModelState.AddModelError("Proizvoditel_ID", "Некорректный производитель");
                return PartialView("~/Views/ManageTovari/_AddEditTovar.cshtml", model);
            }
            else if (model.Proizvoditel_ID <= 0)
            {
                ModelState.AddModelError("Proizvoditel_ID", "Некорректный производитель");
                return PartialView("~/Views/ManageTovari/_AddEditTovar.cshtml", model);
            }

            try
            {
                Convert.ToDouble(model.Cost);
                if(Convert.ToDouble(model.Cost) < 1)
                {
                    ModelState.AddModelError("Cost", "Некорректная цена");
                    return PartialView("~/Views/ManageTovari/_AddEditTovar.cshtml", model);
                }
            }
            catch
            {
                ModelState.AddModelError("Cost", "Некорректная цена");
                return PartialView("~/Views/ManageTovari/_AddEditTovar.cshtml", model);
            }

            if(model.Description == null)
            {
                ModelState.AddModelError("Description", "Некорректное описание");
                return PartialView("~/Views/ManageTovari/_AddEditTovar.cshtml", model);
            }
            else if(model.Description.Trim().Length < 10)
            {
                ModelState.AddModelError("Description", "Некорректное описание");
                return PartialView("~/Views/ManageTovari/_AddEditTovar.cshtml", model);
            }

            for(int i = 0; i<model.Nalichielist.Count();i++)
            {
                try
                {
                    Convert.ToInt16(model.Nalichielist[i].CountInFilial);
                }
                catch
                {
                    ModelState.AddModelError("Nalichielist[" + i + "].CountInFilial", "Некорректное количество");
                    return PartialView("~/Views/ManageTovari/_AddEditTovar.cshtml", model);
                }
                if(model.Nalichielist[i].CountInFilial < 0)
                {
                    ModelState.AddModelError("Nalichielist["+i+"].CountInFilial", "Некорректное количество");
                    return PartialView("~/Views/ManageTovari/_AddEditTovar.cshtml", model);
                }
            }

            ModelState.Clear();
            Good good = new Good();
            if (id == 0)
            {
                good.CategoriId = model.Category_ID;
                good.ManufactureId = model.Proizvoditel_ID;
                good.Name = model.Name_Tovar;
                good.Description = model.Description;
                good.Cost = Convert.ToDouble(model.Cost);
                good.FtppathImage = null;
                good.IsAvaliable = true;
                _context.Goods.Add(good);
                _context.SaveChanges();
                var listGoodsFilials = _context.GoodsFilials.Where(p => p.GoodsId == good.IdGood).ToList();
                foreach (var a in model.Nalichielist)
                {
                    GoodsFilial goodsFilial = listGoodsFilials.Where(p => p.FilialId == a.ID_Filial && p.GoodsId == good.IdGood).First(); ;
                    goodsFilial.CountSklad = a.CountInFilial;
                    _context.Update(goodsFilial);
                }
                _context.SaveChanges();
            }
            else
            {
                good.IdGood = id;
                good.CategoriId = model.Category_ID;
                good.ManufactureId = model.Proizvoditel_ID;
                good.Name = model.Name_Tovar;
                good.Description = model.Description;
                good.Cost = Convert.ToDouble(model.Cost);
                good.FtppathImage = null;
                good.IsAvaliable = true;
                _context.Goods.Update(good);
                _context.SaveChanges();
                var listGoodsFilials = _context.GoodsFilials.Where(p => p.GoodsId == good.IdGood).ToList();
                foreach (var a in model.Nalichielist)
                {
                    GoodsFilial goodsFilial = listGoodsFilials.Where(p => p.FilialId == a.ID_Filial && p.GoodsId == good.IdGood).First(); ;
                    goodsFilial.CountSklad = a.CountInFilial;
                    _context.Update(goodsFilial);
                }
                _context.SaveChanges();
            }

            return RedirectToAction("ChangeImage", "ManageTovari", new {id = good.IdGood});
        }

        public async Task<IActionResult> ChangeImage(int id)
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");

            changeImageTovar model = new changeImageTovar();
            model.ID_Tovar = id;
            var good = (Good)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Tovars/GetTovarByIDToChangeImage/{id}").Result.Content.ReadAsStringAsync().Result, typeof(Good));
            model.NameTovar = good.Name;
            model.FTTPPathImage = good.FtppathImage;
            return PartialView("~/Views/ManageTovari/_ChangeImage.cshtml", model);
        } 

        //СДЕЛАТЬ ОТПРАВКУ КАРТИНОК ТОВАРОВ НА СЕРВЕР
    }
}
