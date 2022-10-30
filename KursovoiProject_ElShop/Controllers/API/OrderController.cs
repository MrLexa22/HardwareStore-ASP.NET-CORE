using KursovoiProject_ElShop.Controllers.Validation;
using KursovoiProject_ElShop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KursovoiProject_ElShop.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ElShopContext _context;
        HttpClient client = new HttpClient();

        public OrderController(ElShopContext context)
        {
            _context = context;
            client.BaseAddress = new Uri(BaseAddresse.Address);
        }

        [HttpGet("GetOrders")]
        public async Task<ActionResult<IEnumerable<OderInformationForIndexPage>>> GetOrders()
        {
            var listOrders = await _context.Orders.Include(p => p.User).Include(p => p.Filial).Include(p=>p.OrderContainers).ToListAsync();
            List<OderInformationForIndexPage> orders = new List<OderInformationForIndexPage>();
            foreach (var order in listOrders)
            {
                OderInformationForIndexPage h = new OderInformationForIndexPage();
                h.ID_Order = order.OrderNumber;
                h.FI = order.User.FirstName + " " + order.User.LastName;
                h.PhoneNumber = order.User.PhoneNumber;
                h.DateOrder = order.DateOrder;
                if(order.DateReadyToExtradition != null)
                    h.DateReadyToExtradition = order.DateReadyToExtradition;
                if(order.DateExtradition != null)
                    h.DateExtradition = order.DateExtradition;
                h.FilialName = order.Filial.NameFilial;
                h.Filial_ID = order.FilialId;
                h.SummaOrder = order.ItogSumma.ToString();
                h.OrdersPositions = order.OrderContainers.Count();
                h.StatusOrder = order.Status;
                orders.Add(h);
            }
            return orders;
        }

        [HttpGet("GetCountWithStatus/{idFilial}")]
        public async Task<ActionResult<int[]>> GetCountWithStatus(int idFilial)
        {
            int[] massiv = new int[4];
            massiv[0] = _context.Orders.Where(p => p.Status == 0 && p.FilialId == idFilial).Count();
            massiv[1] = _context.Orders.Where(p => p.Status == 1 && p.FilialId == idFilial).Count();
            massiv[2] = _context.Orders.Where(p => p.Status == 2 && p.FilialId == idFilial).Count();
            massiv[3] = _context.Orders.Where(p => p.Status == 3 || p.Status == 4 && p.FilialId == idFilial).Count();
            return massiv;
        }
    }
}
