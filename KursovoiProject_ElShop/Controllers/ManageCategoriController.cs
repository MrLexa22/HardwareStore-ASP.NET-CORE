using KursovoiProject_ElShop.Controllers.Validation;
using KursovoiProject_ElShop.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace KursovoiProject_ElShop.Controllers
{
    public class ManageCategoriController : Controller
    {
        private readonly ElShopContext _context;
        HttpClient client = new HttpClient();
        public ManageCategoriController(ElShopContext context)
        {
            _context = context;
            client.BaseAddress = new Uri(BaseAddresse.Address);
        }

        public bool CheckAuthentication()
        {
            if (!User.Identity.IsAuthenticated)
                return false;
            if (User.IsInRole("Администратор") || User.IsInRole("Сотрудник склада магазина"))
                return true;
            else
                return false;
        }
        public ActionResult Index()
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");
            return View();
        }

        public async Task<IActionResult> GetAllCategory(string? search, int? typesort, int? type, int page = 1)
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");
            IndexPageCategory model = new IndexPageCategory();

            int pageSize = 15;
            var listCategs = (List<CategoriesSite>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Categories/GetAllCategs").Result.Content.ReadAsStringAsync().Result, typeof(List<CategoriesSite>));

            IQueryable<CategoriesSite> Blocks = listCategs.AsQueryable();

            if (search != null)
            {
                if (search.Trim() != "")
                    Blocks = Blocks.Where(p => p.CategoryName.ToLower().Contains(search.Trim().ToLower()));
            }

            if (typesort == 1)
                Blocks = Blocks.OrderByDescending(p => p.CategoryName);

            if (type == 1)
                Blocks = Blocks.Where(p => p.IsMain == true);
            if (type == 2)
                Blocks = Blocks.Where(p => p.IsMain == false);

            var items = Blocks.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var count = Blocks.Count();
            model.CategoriesSiteList = items;
            model.PageViewModel = new PageViewModel(count, page, pageSize);
            model.FilterViewModel = new FilterViewModel_AddsSites(search, typesort, type, null);
            return PartialView("~/Views/ManageCategori/_ListCategory.cshtml", model);
        }

        public async Task<IActionResult> ConfirmDelete(int id)
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");
            CategoriesSite model = new CategoriesSite();
            model.CategoryId = id;
            var a = (Category)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Categories/{id}").Result.Content.ReadAsStringAsync().Result, typeof(Category));
            model.CategoryName = a.NameCategori;
            return PartialView("~/Views/ManageCategori/_ConfirmDelete.cshtml", model);
        }

        public async Task<IActionResult> AddEditCategory(int id)
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");
            CategoriesSite model = new CategoriesSite();
            if (id != null && id > 0)
                model.CategoryId = id;
            else
                model.CategoryId = 0;
            if (id > 0)
                model = (CategoriesSite)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Categories/GetCategory/{id}").Result.Content.ReadAsStringAsync().Result, typeof(CategoriesSite));

            return PartialView("~/Views/ManageCategori/_AddEditCategory.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditPost(int id, CategoriesSite model)
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");

            var count_main = (List<MainCategory>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Categories/MainCategories").Result.Content.ReadAsStringAsync().Result, typeof(List<MainCategory>));
            
            if (count_main.Count() == 4 && model.IsMain == true)
            {
                ModelState.AddModelError("IsMain", "На сайте уже 4 сверху категорий есть! Максимум 4.");
                return PartialView("~/Views/ManageCategori/_AddEditCategory.cshtml", model);
            }
            if(((List<Category>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Categories/GetByName/{model.CategoryName}/{id}").Result.Content.ReadAsStringAsync().Result, typeof(List<Category>))).Count() > 0)
            {
                ModelState.AddModelError("CategoryName", "Такое наименование категории уже существует!");
                return PartialView("~/Views/ManageCategori/_AddEditCategory.cshtml", model);
            }

            if (id > 0)
                model.CategoryId = id;

            var myContent = JsonConvert.SerializeObject(model);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            if (id > 0)
                await client.PostAsync("api/Categories/UpdateCategory", byteContent);
            else
                await client.PostAsync("api/Categories/Create", byteContent);

            return RedirectToAction("Index", "ManageCategori");
        }
    }
}
