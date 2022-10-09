using KursovoiProject_ElShop.Controllers.Validation;
using KursovoiProject_ElShop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KursovoiProject_ElShop.Controllers
{
    public class AddsSitesController : Controller
    {
        private readonly ElShopContext _context;
        HttpClient client = new HttpClient();
        public AddsSitesController(ElShopContext context)
        {
            _context = context;
            client.BaseAddress = new Uri(BaseAddresse.Address);
        }

        public bool CheckAuthentication()
        {
            if (!User.Identity.IsAuthenticated)
                return false;
            if (!User.IsInRole("Рекламщик"))
                return false;
            return true;
        }
        public ActionResult Index()
        {
            if(!CheckAuthentication())
                return RedirectToAction("Index", "Home");
            return View();
        }
        public async Task<IActionResult> GetAllAdds(string? search, int? typesort, int? type, int? status, int page = 1)
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");
            ListAdds model = new ListAdds();

            int pageSize = 15;
            var listAdds = (List<AddsSite>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/AddsSites/All").Result.Content.ReadAsStringAsync().Result, typeof(List<AddsSite>));
            foreach(var a in listAdds)
            {
                if(a.PublisherUserId != null)
                    a.PublisherUser = (User)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Users/{a.PublisherUserId}").Result.Content.ReadAsStringAsync().Result, typeof(User));
            }    
            IQueryable<AddsSite> Blocks = listAdds.AsQueryable();
            Blocks = Blocks.OrderBy(p => p.DateEdn);

            if(search != null)
            {
                if(search.Trim() != "")
                    Blocks = Blocks.Where(p=>p.Name.ToLower().Contains(search.Trim().ToLower()));
            }
            if (typesort == 1)
                Blocks = Blocks.OrderByDescending(p => p.DateEdn);
            if (typesort == 2)
                Blocks = Blocks.OrderBy(p => p.Name);
            if (type > 0)
                Blocks = Blocks.Where(p => p.TypeWhere == type);
            if (status == 1)
                Blocks = Blocks.Where(p => p.DateEdn >= DateTime.Now || p.DateEdn == null);
            if(status == 2)
                Blocks = Blocks.Where(p => p.DateEdn < DateTime.Now);

            var items = Blocks.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var count = Blocks.Count();
            model.AddsSite = items;
            model.PageViewModel = new PageViewModel(count, page, pageSize);
            model.FilterViewModel = new FilterViewModel_AddsSites(search, typesort, type, status);
            return PartialView("~/Views/AddsSites/_ListAdds.cshtml", model);
        }

        public async Task<IActionResult> AddEditAdds(int id)
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");
            AddEditAdds model = new AddEditAdds();
            model.categories = (List<Category>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Categories").Result.Content.ReadAsStringAsync().Result, typeof(List<Category>));
            return PartialView("~/Views/AddsSites/_AddEditAdds.cshtml", model);
        }
    }
}
