using KursovoiProject_ElShop.Controllers.Validation;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using MySqlConnector;
using NuGet.Protocol.Plugins;
using System.IO;
using MySqlCommand = MySql.Data.MySqlClient.MySqlCommand;

namespace KursovoiProject_ElShop.Controllers
{
    public class AdministrationController : Controller
    {
        private string constring = "server=89.108.64.223;user=remoteDB;password=ietie7chohmo;database=ElShop;";
        private readonly ElShopContext _context;
        HttpClient client = new HttpClient();
        public AdministrationController(ElShopContext context)
        {
            _context = context;
            client.BaseAddress = new Uri(BaseAddresse.Address);
        }
        public bool CheckAuthentication()
        {
            if (!User.Identity.IsAuthenticated)
                return false;
            if (User.IsInRole("Администратор"))
                return true;
            else
                return false;
        }
        public IActionResult Index()
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");
            return View();
        }

        public IActionResult Reservnoe()
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");
            return PartialView("~/Views/Administration/_ReservnoeKopirovanie.cshtml");
        }

        public IActionResult downloadReservnoe()
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");

            string file = "backup "+DateTime.Now.ToLocalTime()+".sql";
            file = file.Replace(":", ".");
            System.IO.MemoryStream content = new System.IO.MemoryStream();
            using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(constring))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    using (MySqlBackup mb = new MySqlBackup(cmd))
                    {
                        cmd.Connection = conn;
                        conn.Open();
                        mb.ExportToMemoryStream(content);
                        conn.Close();
                    }
                }
            }
            var contentType = "application/octet-stream";
            return File(content.ToArray(), contentType, file);
        }

        public IActionResult ReservnoeNotFull()
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");
            return PartialView("~/Views/Administration/_ReservnoeKopirovanieNotFull.cshtml");
        }

        public IActionResult downloadReservnoeNotFull()
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");

            string file = "structureBackup " + DateTime.Now.ToLocalTime() + ".sql";
            file = file.Replace(":", ".");
            System.IO.MemoryStream content = new System.IO.MemoryStream();
            using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(constring))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    using (MySqlBackup mb = new MySqlBackup(cmd))
                    {
                        cmd.Connection = conn;
                        conn.Open();
                        mb.ExportInfo.ExportRows = false;
                        mb.ExportToMemoryStream(content);
                        conn.Close();
                    }
                }
            }
            var contentType = "application/octet-stream";
            return File(content.ToArray(), contentType, file);
        }
    }
}
