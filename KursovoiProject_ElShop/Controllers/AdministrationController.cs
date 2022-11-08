using CsvHelper;
using KursovoiProject_ElShop.Controllers.Validation;
using KursovoiProject_ElShop.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using MySqlConnector;
using NuGet.Protocol.Plugins;
using System.Globalization;
using System.IO;
using System.Text;
using static System.Net.WebRequestMethods;
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
        public IActionResult Index(string? error)
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");
            if(error == "1")
                ViewData["Message"] = "Вы выбрали некорректный тип файла при импорте БД";
            if (error == "2")
                ViewData["Message"] = "Вы выбрали файл частичного копирования, а не полного";
            if (error == "3")
                ViewData["Message"] = "РЕЗЕРВНОЕ КОПИРОВАНИЕ ЗАВЕРШЕНО!";
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

        public IActionResult Vosstanovlenie()
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");
            return PartialView("~/Views/Administration/_Vostanovlenie.cshtml", new CreatePostFile());
        }

        public IActionResult VostanovleniePost(CreatePostFile model)
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");
            if (model.MyFile == null)
            {
                return RedirectToAction("Index", "Administration", new { error = "1" });
            }
            string g = Path.GetExtension(model.MyFile.FileName).ToString();
            if (g != ".sql")
            {
                return RedirectToAction("Index", "Administration", new { error = "1" });
            }
            if (model.MyFile.FileName.Contains("structure"))
            {
                return RedirectToAction("Index", "Administration", new { error = "2" });
            }

            Stream stream = new MemoryStream();
            model.MyFile.CopyTo(stream);
            using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(constring))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    using (MySqlBackup mb = new MySqlBackup(cmd))
                    {
                        cmd.Connection = conn;
                        conn.Open();
                        //mb.ImportFromStream(stream);
                        conn.Close();
                    }
                }
            }
            return RedirectToAction("Index", "Administration", new { error = "3" });
        }

        public IActionResult Export()
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");
            return PartialView("~/Views/Administration/_Export.cshtml");
        }
        public IActionResult downloadExport(int typeexport, int whatexport)
        {
            if (!CheckAuthentication())
                return RedirectToAction("Index", "Home");

            System.IO.MemoryStream content = new System.IO.MemoryStream();
            if (typeexport == 1)
            {
                using (var writer = new StreamWriter(content, Encoding.UTF8))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture, false))
                {
                    if(whatexport == 1)
                        csv.WriteRecords(_context.AddsSites.ToList());
                    if (whatexport == 2)
                        csv.WriteRecords(_context.Categories.ToList());
                    if (whatexport == 3)
                        csv.WriteRecords(_context.Filials.ToList());
                    if (whatexport == 4)
                        csv.WriteRecords(_context.Goods.ToList());
                    if (whatexport == 5)
                        csv.WriteRecords(_context.GoodsFilials.ToList());
                    if (whatexport == 6)
                        csv.WriteRecords(_context.Korzinas.ToList());
                    if (whatexport == 7)
                        csv.WriteRecords(_context.MainCategories.ToList());
                    if (whatexport == 8)
                        csv.WriteRecords(_context.Manufactures.ToList());
                    if (whatexport == 9)
                        csv.WriteRecords(_context.OrderContainers.ToList());
                    if (whatexport == 10)
                        csv.WriteRecords(_context.Orders.ToList());
                    if (whatexport == 11)
                        csv.WriteRecords(_context.Roles.ToList());
                    if (whatexport == 12)
                        csv.WriteRecords(_context.Users.ToList());
                    if (whatexport == 13)
                        csv.WriteRecords(_context.UsersRoles.ToList());
                    if (whatexport == 14)
                        csv.WriteRecords(_context.Workers.ToList());
                    if (whatexport == 15)
                        csv.WriteRecords(_context.WorkersFilials.ToList());
                }

                if (whatexport == 1)
                    return File(content.ToArray(), "text/csv", "Export ad blocks " + DateTime.Now.ToLocalTime().ToString().Replace(":", ".") + ".csv");
                if (whatexport == 2)
                    return File(content.ToArray(), "text/csv", "Export categories " + DateTime.Now.ToLocalTime().ToString().Replace(":", ".") + ".csv");
                if (whatexport == 3)
                    return File(content.ToArray(), "text/csv", "Export filials " + DateTime.Now.ToLocalTime().ToString().Replace(":", ".") + ".csv");
                if (whatexport == 4)
                    return File(content.ToArray(), "text/csv", "Export goods " + DateTime.Now.ToLocalTime().ToString().Replace(":", ".") + ".csv");
                if (whatexport == 5)
                    return File(content.ToArray(), "text/csv", "Export availability of goods at filials " + DateTime.Now.ToLocalTime().ToString().Replace(":", ".") + ".csv");
                if (whatexport == 6)
                    return File(content.ToArray(), "text/csv", "Export shopping baskets " + DateTime.Now.ToLocalTime().ToString().Replace(":", ".") + ".csv");
                if (whatexport == 7)
                    return File(content.ToArray(), "text/csv", "Export main categories " + DateTime.Now.ToLocalTime().ToString().Replace(":", ".") + ".csv");
                if (whatexport == 8)
                    return File(content.ToArray(), "text/csv", "Export manufactures " + DateTime.Now.ToLocalTime().ToString().Replace(":", ".") + ".csv");
                if (whatexport == 9)
                    return File(content.ToArray(), "text/csv", "Export composition of customer orders " + DateTime.Now.ToLocalTime().ToString().Replace(":", ".") + ".csv");
                if (whatexport == 10)
                    return File(content.ToArray(), "text/csv", "Export orders " + DateTime.Now.ToLocalTime().ToString().Replace(":", ".") + ".csv");
                if (whatexport == 11)
                    return File(content.ToArray(), "text/csv", "Export roles " + DateTime.Now.ToLocalTime().ToString().Replace(":", ".") + ".csv");
                if (whatexport == 12)
                    return File(content.ToArray(), "text/csv", "Export users " + DateTime.Now.ToLocalTime().ToString().Replace(":", ".") + ".csv");
                if (whatexport == 13)
                    return File(content.ToArray(), "text/csv", "Export roles of users " + DateTime.Now.ToLocalTime().ToString().Replace(":", ".") + ".csv");
                if (whatexport == 14)
                    return File(content.ToArray(), "text/csv", "Export workers " + DateTime.Now.ToLocalTime().ToString().Replace(":", ".") + ".csv");
                if (whatexport == 15)
                    return File(content.ToArray(), "text/csv", "Export workers filials " + DateTime.Now.ToLocalTime().ToString().Replace(":", ".") + ".csv");
            }
            else
            {
                string file = "structureBackup " + DateTime.Now.ToLocalTime() + ".sql";
                using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(constring))
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        using (MySqlBackup mb = new MySqlBackup(cmd))
                        {
                            cmd.Connection = conn;
                            conn.Open();
                            mb.ExportInfo.ExportTableStructure = false;
                            mb.ExportInfo.ExportFunctions = false;
                            mb.ExportInfo.ExportProcedures = false;
                            mb.ExportInfo.ExportTriggers = false;
                            mb.ExportInfo.ExportViews = false;
                            mb.ExportInfo.EnableComment = false;
                            mb.ExportInfo.ExportEvents = false;
                            mb.ExportInfo.AddCreateDatabase = false;
                            mb.ExportInfo.AddDropDatabase= false;
                            mb.ExportInfo.ExportRoutinesWithoutDefiner = false;
                            if (whatexport == 1)
                            {
                                file = "Export ad blocks " + DateTime.Now.ToLocalTime().ToString().Replace(":", ".") + ".sql";
                                mb.ExportInfo.TablesToBeExportedList = new List<string> {
                                    "AddsSite"
                                };
                            }
                            if (whatexport == 2)
                            {
                                file = "Export categories " + DateTime.Now.ToLocalTime().ToString().Replace(":", ".") + ".sql";
                                mb.ExportInfo.TablesToBeExportedList = new List<string> {
                                    "Categories"
                                };
                            }
                            if (whatexport == 3)
                            {
                                file = "Export filials " + DateTime.Now.ToLocalTime().ToString().Replace(":", ".") + ".sql";
                                mb.ExportInfo.TablesToBeExportedList = new List<string> {
                                    "Filials"
                                };
                            }
                            if (whatexport == 4)
                            {
                                file = "Export goods " + DateTime.Now.ToLocalTime().ToString().Replace(":", ".") + ".sql";
                                mb.ExportInfo.TablesToBeExportedList = new List<string> {
                                    "Goods"
                                };
                            }
                            if (whatexport == 5)
                            {
                                file = "Export availability of goods at filials " + DateTime.Now.ToLocalTime().ToString().Replace(":", ".") + ".sql";
                                mb.ExportInfo.TablesToBeExportedList = new List<string> {
                                    "Goods_Filial"
                                };
                            }
                            if (whatexport == 6)
                            {
                                file = "Export shopping baskets " + DateTime.Now.ToLocalTime().ToString().Replace(":", ".") + ".sql";
                                mb.ExportInfo.TablesToBeExportedList = new List<string> {
                                    "Korzina"
                                };
                            }
                            if (whatexport == 7)
                            {
                                file = "Export main categories " + DateTime.Now.ToLocalTime().ToString().Replace(":", ".") + ".sql";
                                mb.ExportInfo.TablesToBeExportedList = new List<string> {
                                    "MainCategories"
                                };
                            }
                            if (whatexport == 8)
                            {
                                file = "Export manufactures " + DateTime.Now.ToLocalTime().ToString().Replace(":", ".") + ".sql";
                                mb.ExportInfo.TablesToBeExportedList = new List<string> {
                                    "Manufactures"
                                };
                            }
                            if (whatexport == 9)
                            {
                                file = "Export composition of customer orders " + DateTime.Now.ToLocalTime().ToString().Replace(":", ".") + ".sql";
                                mb.ExportInfo.TablesToBeExportedList = new List<string> {
                                    "OrderContainer"
                                };
                            }
                            if (whatexport == 10)
                            {
                                file = "Export orders " + DateTime.Now.ToLocalTime().ToString().Replace(":", ".") + ".sql";
                                mb.ExportInfo.TablesToBeExportedList = new List<string> {
                                    "Orders"
                                };
                            }
                            if (whatexport == 11)
                            {
                                file = "Export roles " + DateTime.Now.ToLocalTime().ToString().Replace(":", ".") + ".sql";
                                mb.ExportInfo.TablesToBeExportedList = new List<string> {
                                    "Roles"
                                };
                            }
                            if (whatexport == 12)
                            {
                                file = "Export users " + DateTime.Now.ToLocalTime().ToString().Replace(":", ".") + ".sql";
                                mb.ExportInfo.TablesToBeExportedList = new List<string> {
                                    "Users"
                                };
                            }
                            if (whatexport == 13)
                            {
                                file = "Export roles of users " + DateTime.Now.ToLocalTime().ToString().Replace(":", ".") + ".sql";
                                mb.ExportInfo.TablesToBeExportedList = new List<string> {
                                    "Users_Role"
                                };
                            }
                            if (whatexport == 14)
                            {
                                file = "Export workers " + DateTime.Now.ToLocalTime().ToString().Replace(":", ".") + ".sql";
                                mb.ExportInfo.TablesToBeExportedList = new List<string> {
                                    "Workers"
                                };
                            }
                            if (whatexport == 15)
                            {
                                file = "Export workers filials " + DateTime.Now.ToLocalTime().ToString().Replace(":", ".") + ".sql";
                                mb.ExportInfo.TablesToBeExportedList = new List<string> {
                                    "Workers_Filial"
                                };
                            }
                            mb.ExportToMemoryStream(content);
                            conn.Close();
                        }
                    }
                }
                var contentType = "application/octet-stream";
                return File(content.ToArray(), contentType, file);
            }

            return File(content.ToArray(), "text/csv", "Export.csv");
        }
    }
}
