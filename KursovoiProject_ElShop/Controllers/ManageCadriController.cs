using KursovoiProject_ElShop.Controllers.Validation;
using KursovoiProject_ElShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace KursovoiProject_ElShop.Controllers
{
    public class ManageCadriController : Controller
    {
        private readonly ElShopContext _context;
        HttpClient client = new HttpClient();
        public ManageCadriController(ElShopContext context)
        {
            _context = context;
            client.BaseAddress = new Uri(BaseAddresse.Address);
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
        public bool CheckAuthentication()
        {
            if (!User.Identity.IsAuthenticated)
                return false;
            if (User.IsInRole("Сотрудник отдела кадров") || User.IsInRole("Администратор"))
                return true;
            else
                return false;
        }

        public IActionResult Index()
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");
            IndexPageCadri model = new IndexPageCadri();
            model.filials = (List<Filial>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Filials/GetAllFilials").Result.Content.ReadAsStringAsync().Result, typeof(List<Filial>));
            return View(model);
        }

        public async Task<IActionResult> GetAllCadri(int uvol, string? search, int? typesort, int? dolzhnost, int? filial, int page = 1)
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");

            IndexPageCadri model = new IndexPageCadri();
            model.statusSite = uvol;
            int pageSize = 15;
            var listAdds = (List<InfoCadri>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Workers/{uvol}").Result.Content.ReadAsStringAsync().Result, typeof(List<InfoCadri>));
            IQueryable<InfoCadri> Blocks = listAdds.AsQueryable();

            if (search != null)
            {
                if (search.Trim() != "")
                    Blocks = Blocks.Where(p => p.Familia.ToLower().Contains(search.Trim().ToLower()) || p.Ima.ToLower().Contains(search.Trim().ToLower()));
            }
            if (typesort == 1)
                Blocks = Blocks.OrderBy(p => p.Familia);
            if (typesort == 2)
                Blocks = Blocks.OrderBy(p => Convert.ToDateTime(p.DateOfBirth));
            if (dolzhnost > 0)
                Blocks = Blocks.Where(p => p.listRoles.Where(g => g.IdRole == dolzhnost).Any());
            if (filial > 0)
                Blocks = Blocks.Where(p => p.listFilial.Where(f=>f.IdFilial == filial).Any());
            if(filial == -1)
                Blocks = Blocks.Where(p => p.listFilial[0].NameFilial == "Не указан");

            var items = Blocks.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var count = Blocks.Count();
            model.listCadri = items;
            model.PageViewModel = new PageViewModel(count, page, pageSize);
            model.FilterViewModel = new FilterViewModel_AddsSites(search, typesort, dolzhnost, filial);
            return PartialView("~/Views/ManageCadri/_ListCadri.cshtml", model);
        }
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");

            BeforeDeleteCadri model = new BeforeDeleteCadri();
            model.ID_Worker = id;
            var a = (InfoCadri)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Workers/GetByID/{id}").Result.Content.ReadAsStringAsync().Result, typeof(InfoCadri));
            model.FI = a.Familia + " "+a.Ima;
            return PartialView("~/Views/ManageCadri/_ConfirmDelete.cshtml", model);
        }
        public async Task<IActionResult> ConfirmRecovery(int id)
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");

            BeforeDeleteCadri model = new BeforeDeleteCadri();
            model.ID_Worker = id;
            var a = (InfoCadri)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Workers/GetByID/{id}").Result.Content.ReadAsStringAsync().Result, typeof(InfoCadri));
            model.FI = a.Familia + " " + a.Ima;
            return PartialView("~/Views/ManageCadri/_ConfirmRecovery.cshtml", model);
        }
        public async Task<IActionResult> ConfirmBlock(int id)
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");

            BeforeDeleteCadri model = new BeforeDeleteCadri();
            model.ID_Worker = id;
            var a = (InfoCadri)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Workers/GetByID/{id}").Result.Content.ReadAsStringAsync().Result, typeof(InfoCadri));
            model.FI = a.Familia + " " + a.Ima;
            return PartialView("~/Views/ManageCadri/_ConfirmBlock.cshtml", model);
        }

        public async Task<IActionResult> ConfirmUnBlock(int id)
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");

            BeforeDeleteCadri model = new BeforeDeleteCadri();
            model.ID_Worker = id;
            var a = (InfoCadri)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Workers/GetByID/{id}").Result.Content.ReadAsStringAsync().Result, typeof(InfoCadri));
            model.FI = a.Familia + " " + a.Ima;
            return PartialView("~/Views/ManageCadri/_ConfirmUnBlock.cshtml", model);
        }

        public async Task<IActionResult> AddEditCadri(int id)
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");

            AddEditCadri model = new AddEditCadri();
            model.ID_Worker = id;
            if(model.ID_Worker > 0)
            {
                //Worker worker = (Worker)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Workers/GetByIDWorker/{id}").Result.Content.ReadAsStringAsync().Result, typeof(Worker));
                Worker worker = _context.Workers.Include(p => p.User).Where(p => p.IdWorker == model.ID_Worker).First();
                model.First_Name = worker.User.FirstName;
                model.Last_Name = worker.User.LastName;
                model.Telefon = worker.User.PhoneNumber;
                model.Login = worker.User.Login;

                model.INN = worker.Innworker;
                model.SNILS = worker.Snils;
                model.SeriaPasport = worker.SeriaPasporta;
                model.NomerPasport = worker.NomerPasporta;
                model.DateOfBirth = worker.DateOfBirth.ToDateTime(TimeOnly.Parse("10:00 PM"));
                model.PrikazOPrinat = worker.PrikazOprieme;
                model.AddressRegistration = worker.AddressRegistration;
                if (model.PrikazObUval != null)
                    model.PrikazObUval = worker.PrikazObUvolnenii;
            }
            model.filials = (List<FilialsWithBoolean>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Filials/GetFilialsWithBoolean/{id}").Result.Content.ReadAsStringAsync().Result, typeof(List<FilialsWithBoolean>));
            model.roles = (List<RolesWithBoolean>)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Users/GetRolesUser/{id}").Result.Content.ReadAsStringAsync().Result, typeof(List<RolesWithBoolean>));
            return View("~/Views/ManageCadri/AddEditCadri.cshtml", model);
        }

        public static int GetAge(DateTime birthDate)
        {
            var now = DateTime.Today;
            return now.Year - birthDate.Year - 1 +
                ((now.Month > birthDate.Month || now.Month == birthDate.Month && now.Day >= birthDate.Day) ? 1 : 0);
        }

        public static string FirstUpper(string str)
        {
            try
            {
                string[] s = str.Split(' ');

                for (int i = 0; i < s.Length; i++)
                {
                    if (s[i].Length > 1)
                        s[i] = s[i].Substring(0, 1).ToUpper() + s[i].Substring(1, s[i].Length - 1).ToLower();
                    else s[i] = s[i].ToUpper();
                }
                return string.Join(" ", s);
            }
            catch
            {
                return str;
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddEditCadriPost(int id, AddEditCadri model)
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");

            model.ID_Worker = id;

            if(model.First_Name == null)
                ModelState.AddModelError("First_Name", "Некорректная фамилия сотрудника");
            if (model.First_Name != null)
            {
                if (model.First_Name.Trim() == "")
                {
                    ModelState.AddModelError("First_Name", "Некорректная фамилия сотрудника");
                }
                else
                    model.First_Name = FirstUpper(model.First_Name);
            }

            if (model.Last_Name == null)
                ModelState.AddModelError("Last_Name", "Некорректное имя сотрудника");
            if (model.Last_Name != null)
            {
                if (model.Last_Name.Trim() == "")
                {
                    ModelState.AddModelError("Last_Name", "Некорректное имя сотрудника");
                }
                else
                    model.Last_Name = FirstUpper(model.Last_Name);
            }
            
            if(GetAge(model.DateOfBirth) < 14 || GetAge(model.DateOfBirth) > 100)
                ModelState.AddModelError("DateOfBirth", "Некорректная дата рождения");

            if(model.PrikazOPrinat == null)
                ModelState.AddModelError("PrikazOPrinat", "Некорректный приказ о принятии на работу");

            if (model.PrikazOPrinat != null)
                if (model.PrikazOPrinat.Trim() == "")
                    ModelState.AddModelError("PrikazOPrinat", "Некорректный приказ о принятии на работу");

            if (model.AddressRegistration == null)
                ModelState.AddModelError("AddressRegistration", "Некорректный адрес");

            if (model.AddressRegistration != null)
                if (model.AddressRegistration.Trim() == "")
                    ModelState.AddModelError("AddressRegistration", "Некорректный адрес");

            if((model.roles.Where(p=>p.ID_Role == 15).First().IsSelected == true || model.roles.Where(p => p.ID_Role == 19).First().IsSelected == true) && model.filials.Where(p=>p.IsSelected == true).Count() == 0)
                ModelState.AddModelError("filials", "Выберите хотя бы один филиал");

            if(model.roles.Count <= 0)
                ModelState.AddModelError("roles", "Выберите хотя бы одну должность");

            if (ModelState.IsValid == false && ModelState.ErrorCount > 1)
                return View("~/Views/ManageCadri/AddEditCadri.cshtml", model);

            if (((bool)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Workers/CheckExistWorkerInn/{id}/{model.INN}").Result.Content.ReadAsStringAsync().Result, typeof(bool))) == false)
                ModelState.AddModelError("INN", "Такой ИНН уже используется");

            if (((bool)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Workers/CheckExistWorkerSnils/{id}/{model.SNILS}").Result.Content.ReadAsStringAsync().Result, typeof(bool))) == false)
                ModelState.AddModelError("SNILS", "Такой СНИЛС уже используется");

            if (((bool)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Workers/CheckExistWorkerPassport/{id}/{model.SeriaPasport}/{model.NomerPasport}").Result.Content.ReadAsStringAsync().Result, typeof(bool))) == false)
                ModelState.AddModelError("SeriaPasport", "Такие паспортные данные уже используется");
            
            if (id == 0)
            {
                if (((bool)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Users/CheckLogin/{id}/{model.Login}").Result.Content.ReadAsStringAsync().Result, typeof(bool))) == false)
                    ModelState.AddModelError("Login", "Такой логин уже используется");

                if (((bool)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Users/CheckPhone/{id}/{model.Telefon}").Result.Content.ReadAsStringAsync().Result, typeof(bool))) == false)
                    ModelState.AddModelError("Telefon", "Такой телефон уже используется");
            }
            else
            {
                Worker worker = await _context.Workers.FindAsync(model.ID_Worker);
                User user = await _context.Users.FindAsync(worker.UserId);
                if (((bool)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Users/CheckLogin/{user.IdUser}/{model.Login}").Result.Content.ReadAsStringAsync().Result, typeof(bool))) == false)
                    ModelState.AddModelError("Login", "Такой логин уже используется");

                if (((bool)JsonConvert.DeserializeObject(client.GetAsync(@$"api/Users/CheckPhone/{user.IdUser}/{model.Telefon}").Result.Content.ReadAsStringAsync().Result, typeof(bool))) == false)
                    ModelState.AddModelError("Telefon", "Такой телефон уже используется");
            }

            if (ModelState.IsValid == false && ModelState.ErrorCount > 1)
                return View("~/Views/ManageCadri/AddEditCadri.cshtml", model);
            model.Password = "1";
            var myContent = JsonConvert.SerializeObject(model);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            if (model.ID_Worker == 0)
            {
                User user = new User();
                user.FirstName = model.First_Name;
                user.LastName = model.Last_Name;
                user.PhoneNumber = model.Telefon;
                user.Login = model.Login;
                user.IsAvalible = 1;
                var pas = GetRandomPassword(8);
                user.Password = HashPassword.hashPassword(pas);
                EmailService.SendEmailWithValuesForEnter(pas, model.Login);
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                Worker worker = new Worker();
                worker.AddressRegistration = model.AddressRegistration;
                worker.PrikazOprieme = model.PrikazOPrinat;
                worker.UserId = user.IdUser;
                worker.DateOfBirth = DateOnly.FromDateTime(model.DateOfBirth);
                worker.Innworker = model.INN;
                worker.Snils = model.SNILS;
                worker.SeriaPasporta = model.SeriaPasport;
                worker.NomerPasporta = model.NomerPasport;
                _context.Workers.Add(worker);
                await _context.SaveChangesAsync();

                foreach (var a in model.filials)
                {
                    if (a.IsSelected == true)
                    {
                        WorkersFilial workersFilial = new WorkersFilial();
                        workersFilial.FilialId = a.ID_Filial;
                        workersFilial.UserId = user.IdUser;
                        _context.WorkersFilials.Add(workersFilial);
                    }
                }
                await _context.SaveChangesAsync();

                foreach (var a in model.roles)
                {
                    if (a.IsSelected == true)
                    {
                        UsersRole usersRole = new UsersRole();
                        usersRole.UserId = user.IdUser;
                        usersRole.RoleId = a.ID_Role;
                        _context.UsersRoles.Add(usersRole);
                    }
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                Worker worker = await _context.Workers.FindAsync(model.ID_Worker);

                User user = await _context.Users.FindAsync(worker.UserId);
                user.FirstName = model.First_Name;
                user.LastName = model.Last_Name;
                user.PhoneNumber = model.Telefon;
                if (user.Login != model.Login)
                {
                    user.Login = model.Login;
                    var pas = GetRandomPassword(8);
                    user.Password = HashPassword.hashPassword(pas);
                    EmailService.SendEmailWithValuesForEnter(pas, model.Login);
                }
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                worker.AddressRegistration = model.AddressRegistration;
                worker.PrikazOprieme = model.PrikazOPrinat;
                worker.UserId = user.IdUser;
                worker.DateOfBirth = DateOnly.FromDateTime(model.DateOfBirth);
                worker.Innworker = model.INN;
                worker.Snils = model.SNILS;
                worker.SeriaPasporta = model.SeriaPasport;
                worker.NomerPasporta = model.NomerPasport;
                _context.Workers.Update(worker);
                await _context.SaveChangesAsync();

                foreach(var a in _context.WorkersFilials.Where(p=>p.UserId == user.IdUser))
                {
                    _context.WorkersFilials.Remove(a);
                }
                foreach (var a in _context.UsersRoles.Where(p => p.UserId == user.IdUser))
                {
                    if(a.RoleId != 21 && a.RoleId != 22)
                        _context.UsersRoles.Remove(a);
                }
                await _context.SaveChangesAsync();

                foreach (var a in model.filials)
                {
                    if (a.IsSelected == true)
                    {
                        WorkersFilial workersFilial = new WorkersFilial();
                        workersFilial.FilialId = a.ID_Filial;
                        workersFilial.UserId = user.IdUser;
                        _context.WorkersFilials.Add(workersFilial);
                    }
                }
                foreach (var a in model.roles)
                {
                    if (a.IsSelected == true)
                    {
                        UsersRole usersRole = new UsersRole();
                        usersRole.UserId = user.IdUser;
                        usersRole.RoleId = a.ID_Role;
                        _context.UsersRoles.Add(usersRole);
                    }
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index","ManageCadri");
        }
    }
}
