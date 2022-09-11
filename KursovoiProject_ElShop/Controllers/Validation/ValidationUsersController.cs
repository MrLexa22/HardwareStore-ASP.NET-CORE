using KursovoiProject_ElShop.API;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace KursovoiProject_ElShop.Controllers.Validation
{
    public class BaseAddresse
    {
        public static string Address = @$"http://localhost:7188/";
        public static string Server = @$"http://89.108.64.223/";
    }
    public class ValidationUsersController : Controller
    {
        HttpClient client = new HttpClient();
        public async Task<IActionResult> CheckLogin(string Email, int id)
        {
            client.BaseAddress = new Uri(BaseAddresse.Address);
            HttpResponseMessage response = await client.GetAsync(@$"api/Users/CheckLogin/{id}/{Email}");
            var responseContent = response.Content;
            string responseBody = await responseContent.ReadAsStringAsync();
            bool res = Convert.ToBoolean(responseBody);
            return Json(res);
        }

        public async Task<IActionResult> CheckPhoneNumber(string Telefon, int id)
        {
            client.BaseAddress = new Uri(BaseAddresse.Address);
            HttpResponseMessage response = await client.GetAsync(@$"api/Users/CheckPhone/{id}/{Telefon}");
            var responseContent = response.Content;
            string responseBody = await responseContent.ReadAsStringAsync();
            bool res = Convert.ToBoolean(responseBody);
            return Json(res);
        }
    }
}
