using KursovoiProject_ElShop.Controllers.Validation;
using KursovoiProject_ElShop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Security.Claims;

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
            if (id != null && id > 0)
                model.ID = id;
            else
                model.ID = 0;
            if(id > 0)
            {
                AddsSite addsSite = (AddsSite)JsonConvert.DeserializeObject(client.GetAsync(@$"api/AddsSites/{id}").Result.Content.ReadAsStringAsync().Result, typeof(AddsSite));
                model.ID = id;
                model.Name = addsSite.Name;
                model.FTPImage = addsSite.FtppathImage;
                model.dateEnd = addsSite.DateEdn;
                model.href = addsSite.Href;
                model.TypeWhere = addsSite.TypeWhere;
            }
            model.categories = (List<Category>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Categories").Result.Content.ReadAsStringAsync().Result, typeof(List<Category>));
            model.manufactures = (List<Manufacture>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Manufactures/GetManufacturesAll").Result.Content.ReadAsStringAsync().Result, typeof(List<Manufacture>));
            return PartialView("~/Views/AddsSites/_AddEditAdds.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditPost(int id, AddEditAdds model, IFormFile uploadedFile)
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");
            if (model.IsEnd == true)
                model.dateEnd = null;
            if(uploadedFile == null && id == 0)
                return RedirectToAction("Index", "Home");
            AddsSite addsSite = new AddsSite();
            addsSite.DateEdn = model.dateEnd;
            addsSite.Href = model.href;
            addsSite.Name = model.Name;
            addsSite.TypeWhere = model.TypeWhere;
            AddsSite addsSiteOld = new AddsSite();
            if (id > 0)
            {
                addsSiteOld = (AddsSite)JsonConvert.DeserializeObject(client.GetAsync(@$"api/AddsSites/{id}").Result.Content.ReadAsStringAsync().Result, typeof(AddsSite));
                addsSite.IdAdds = id;
                addsSite.PublisherUserId = addsSiteOld.PublisherUserId;
                if (uploadedFile != null)
                {
                    addsSite.FtppathImage = addsSiteOld.IdAdds.ToString()+Path.GetExtension(uploadedFile.FileName);
                    SendFileToServer.DeleteOldFile(addsSiteOld.IdAdds.ToString()+Path.GetExtension(addsSiteOld.FtppathImage), addsSiteOld.TypeWhere);
                    SendFileToServer.SendFileAdds(uploadedFile, addsSiteOld.IdAdds.ToString(), Path.GetExtension(uploadedFile.FileName), model.TypeWhere);
                }
                else
                    addsSite.FtppathImage = addsSiteOld.IdAdds.ToString()+Path.GetExtension(addsSiteOld.FtppathImage);
            }
            else
            {
                User us = (User)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Users/email/{getEmail()}").Result.Content.ReadAsStringAsync().Result, typeof(User));
                addsSite.PublisherUserId = us.IdUser;
                addsSite.FtppathImage = Path.GetExtension(uploadedFile.FileName);
            }
            var myContent = JsonConvert.SerializeObject(addsSite);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            if (id == 0)
            {
                var result = (AddsSite)JsonConvert.DeserializeObject(client.PostAsync("api/AddsSites", byteContent).Result.Content.ReadAsStringAsync().Result, typeof(AddsSite));
                SendFileToServer.SendFileAdds(uploadedFile, result.IdAdds.ToString(), Path.GetExtension(uploadedFile.FileName), model.TypeWhere);
            }
            if(id > 0)
                await client.PostAsync("api/AddsSites/Edit", byteContent);
            return RedirectToAction("Index", "AddsSites");
        }

        public async Task<IActionResult> ConfirmDelete(int id)
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");
            AddEditAdds model = new AddEditAdds();
            model.ID = id;
            var a = (AddsSite)JsonConvert.DeserializeObject(client.GetAsync(@$"api/AddsSites/{id}").Result.Content.ReadAsStringAsync().Result, typeof(AddsSite));
            model.Name = a.Name;
            return PartialView("~/Views/AddsSites/_ConfirmDelete.cshtml", model);
        }
    }
}
