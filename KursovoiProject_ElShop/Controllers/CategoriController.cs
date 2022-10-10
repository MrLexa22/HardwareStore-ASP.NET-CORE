using ElSt.Models;
using KursovoiProject_ElShop.Controllers.Validation;
using KursovoiProject_ElShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace KursovoiProject_ElShop.Controllers
{
    public class CategoriController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ElShopContext _context;
        HttpClient client = new HttpClient();
        public CategoriController(ILogger<HomeController> logger, ElShopContext context)
        {
            _logger = logger;
            _context = context;
            client.BaseAddress = new Uri(BaseAddresse.Address);
        }
        public IActionResult Index()
        {
            IndexPageModel model = new IndexPageModel();

            model.Right1Adds = (List<AddsSite>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/AddsSites/GetAddsSiteType/2").Result.Content.ReadAsStringAsync().Result, typeof(List<AddsSite>));
            model.Right2Adds = (List<AddsSite>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/AddsSites/GetAddsSiteType/3").Result.Content.ReadAsStringAsync().Result, typeof(List<AddsSite>));
            model.Categories = (List<Category>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Categories/").Result.Content.ReadAsStringAsync().Result, typeof(List<Category>));
            return View(model);
        }

        public async Task<IActionResult> GetSpisokTovari(int IDCategory, int manufacte, double? min, double? max, int? typesort, int page = 1)
        {
            TovariCategoryModel model = new TovariCategoryModel();
            Category categ = (Category)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Categories/{IDCategory}").Result.Content.ReadAsStringAsync().Result, typeof(Category));
            var list_tovaris = (List<TovariUsersModel>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/ViewGoodsWithManufactures/GetByCategori/{IDCategory}/0").Result.Content.ReadAsStringAsync().Result, typeof(List<TovariUsersModel>));
            if (User.Identity.IsAuthenticated)
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
                int IdUser = _context.Users.Where(p => p.Login == email).First().IdUser;
                list_tovaris = (List<TovariUsersModel>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/ViewGoodsWithManufactures/GetByCategori/{IDCategory}/{IdUser}").Result.Content.ReadAsStringAsync().Result, typeof(List<TovariUsersModel>));
            }
            else
            {
                list_tovaris = (List<TovariUsersModel>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/ViewGoodsWithManufactures/GetByCategori/{IDCategory}/0").Result.Content.ReadAsStringAsync().Result, typeof(List<TovariUsersModel>));
            }

            int pageSize = 15;
            model.CountTovar = list_tovaris.Count().ToString();
            IQueryable<TovariUsersModel> Blocks = list_tovaris.AsQueryable();
            if (min != null && max != null)
                Blocks = Blocks.Where(p => p.Cost >= min && p.Cost <= max);
            else if(min != null && max == null)
                Blocks = Blocks.Where(p => p.Cost >= min);
            else if (min == null && max != null)
                Blocks = Blocks.Where(p => p.Cost <= max);

            if (manufacte != 0)
                Blocks = Blocks.Where(p => p.IdManufacture == manufacte);

            if (typesort == 1)
                Blocks = Blocks.OrderBy(p => p.Cost);
            if(typesort == 2)
                Blocks = Blocks.OrderByDescending(p => p.Cost);
            if(typesort == 3)
                Blocks = Blocks.OrderBy(p => p.NameWithManufacture);
            var items = Blocks.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var count = Blocks.Count();
            model.list_tovaris = items;
            model.PageViewModel = new PageViewModel(count, page, pageSize);
            model.NameCategory = categ.NameCategori;
            model.IDCategory = categ.IdCategori;
            model.FilterViewModel = new FilterViewModel_Tovari((List<ManufacturesList>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Manufactures/GetMaufacturesByIdCateg/{IDCategory}").Result.Content.ReadAsStringAsync().Result, typeof(List<ManufacturesList>)), manufacte, IDCategory, min, max, typesort);

            return PartialView("~/Views/Categori/_TovariList.cshtml", model);
        }

        public async Task<IActionResult> GetSpisokTovariSearch(string search, int manufacte, double? min, double? max, int? typesort, int page = 1)
        {
            TovariCategoryModel model = new TovariCategoryModel();
            //Category categ = (Category)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Categories/{IDCategory}").Result.Content.ReadAsStringAsync().Result, typeof(Category));
            var list_tovaris = (List<TovariUsersModel>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/ViewGoodsWithManufactures/GetByName/{search}/0").Result.Content.ReadAsStringAsync().Result, typeof(List<TovariUsersModel>));
            if (User.Identity.IsAuthenticated)
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
                int IdUser = _context.Users.Where(p => p.Login == email).First().IdUser;
                list_tovaris = (List<TovariUsersModel>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/ViewGoodsWithManufactures/GetByName/{search}/{IdUser}").Result.Content.ReadAsStringAsync().Result, typeof(List<TovariUsersModel>));
            }
            else
            {
                list_tovaris = (List<TovariUsersModel>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/ViewGoodsWithManufactures/GetByName/{search}/0").Result.Content.ReadAsStringAsync().Result, typeof(List<TovariUsersModel>));
            }

            int pageSize = 15;
            model.CountTovar = list_tovaris.Count().ToString();
            IQueryable<TovariUsersModel> Blocks = list_tovaris.AsQueryable();
            if (min != null && max != null)
                Blocks = Blocks.Where(p => p.Cost >= min && p.Cost <= max);
            else if (min != null && max == null)
                Blocks = Blocks.Where(p => p.Cost >= min);
            else if (min == null && max != null)
                Blocks = Blocks.Where(p => p.Cost <= max);

            if (manufacte != 0)
                Blocks = Blocks.Where(p => p.IdManufacture == manufacte);

            if (typesort == 1)
                Blocks = Blocks.OrderBy(p => p.Cost);
            if (typesort == 2)
                Blocks = Blocks.OrderByDescending(p => p.Cost);
            if (typesort == 3)
                Blocks = Blocks.OrderBy(p => p.NameWithManufacture);
            var items = Blocks.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var count = Blocks.Count();
            model.list_tovaris = items;
            model.PageViewModel = new PageViewModel(count, page, pageSize);
            //model.NameCategory = categ.NameCategori;
            //model.IDCategory = categ.IdCategori;
            model.search = search;
            model.FilterViewModel = new FilterViewModel_Tovari((List<ManufacturesList>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Manufactures/GetMaufacturesByName/{search}").Result.Content.ReadAsStringAsync().Result, typeof(List<ManufacturesList>)), null, 0, null, null, null);

            return PartialView("~/Views/Categori/_TovariListSearch.cshtml", model);
        }

        public async Task<IActionResult> TovariCategory(int IDCategory, int? manufacte)
        {
            Category categ = (Category)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Categories/{IDCategory}").Result.Content.ReadAsStringAsync().Result, typeof(Category));
            TovariCategoryModel model = new TovariCategoryModel();
            model.Right1Adds = (List<AddsSite>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/AddsSites/GetAddsSiteType/2").Result.Content.ReadAsStringAsync().Result, typeof(List<AddsSite>));
            model.Right2Adds = (List<AddsSite>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/AddsSites/GetAddsSiteType/3").Result.Content.ReadAsStringAsync().Result, typeof(List<AddsSite>));
            model.NameCategory = categ.NameCategori;
            model.IDCategory = categ.IdCategori;
            model.Manufacture_id = manufacte;
            model.FilterViewModel = new FilterViewModel_Tovari((List<ManufacturesList>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Manufactures/GetMaufacturesByIdCateg/{IDCategory}").Result.Content.ReadAsStringAsync().Result, typeof(List<ManufacturesList>)), null, IDCategory, null, null, null);
            return View(model);
        }

        public async Task<IActionResult> SearchResult(string search)
        {
            TovariCategoryModel model = new TovariCategoryModel();
            if (search == null)
                return RedirectToAction("Index", "Home");
            if(search.Trim() == "")
                return RedirectToAction("Index", "Home");
            model.search = search;
            model.Right1Adds = (List<AddsSite>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/AddsSites/GetAddsSiteType/2").Result.Content.ReadAsStringAsync().Result, typeof(List<AddsSite>));
            model.Right2Adds = (List<AddsSite>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/AddsSites/GetAddsSiteType/3").Result.Content.ReadAsStringAsync().Result, typeof(List<AddsSite>));
            model.FilterViewModel = new FilterViewModel_Tovari((List<ManufacturesList>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Manufactures/GetMaufacturesByName/{search}").Result.Content.ReadAsStringAsync().Result, typeof(List<ManufacturesList>)), null, 0, null, null, null);
            return View(model);
        }

        public async Task<IActionResult> Tovar(int id)
        {
            TovariUsersModel model = new TovariUsersModel();
            TovariUsersModel tovar;
            if (User.Identity.IsAuthenticated)
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
                int IdUser = _context.Users.Where(p => p.Login == email).First().IdUser;
                tovar = (TovariUsersModel)JsonConvert.DeserializeObject(client.GetAsync(@$"api/ViewGoodsWithManufactures/GetTovar/{id}/{IdUser}").Result.Content.ReadAsStringAsync().Result, typeof(TovariUsersModel));
            }
            else
            {
                tovar = (TovariUsersModel)JsonConvert.DeserializeObject(client.GetAsync(@$"api/ViewGoodsWithManufactures/GetTovar/{id}/0").Result.Content.ReadAsStringAsync().Result, typeof(TovariUsersModel));
            }
            model = tovar;
            model.Right1Adds = (List<AddsSite>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/AddsSites/GetAddsSiteType/2").Result.Content.ReadAsStringAsync().Result, typeof(List<AddsSite>));
            model.Right2Adds = (List<AddsSite>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/AddsSites/GetAddsSiteType/3").Result.Content.ReadAsStringAsync().Result, typeof(List<AddsSite>));
            model.filials = (List<FilialsNalichTovar>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Filials/GetFilialsTovar/{id}").Result.Content.ReadAsStringAsync().Result, typeof(List<FilialsNalichTovar>));
            return View(model);
        }
    }
}
