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
                h.FI = order.ContactName;
                h.PhoneNumber = order.ContactTelefon;
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

        [HttpGet("GetInformationForIndexPage/{id}")]
        public async Task<ActionResult<OrderAboutManageOrders>> GetInformationForIndexPage(int id)
        {
            OrderAboutManageOrders model = new OrderAboutManageOrders();
            var a = await _context.Orders.Include(p => p.Filial).Include(p => p.SborshikUser).Include(p=>p.SellerUser).Where(p=>p.OrderNumber == id).FirstAsync();
            model.ID_Order = a.OrderNumber;
            model.StatusOrder = a.Status;
            model.FilialAddress = a.Filial.AddressFilial;
            model.FilialName = a.Filial.NameFilial;
            model.DateOrder = a.DateOrder;
            return model;
        }

        [HttpGet("GetInformationForOrderPage/{id}/{email}")]
        public async Task<ActionResult<OrderAboutManageOrders>> GetInformationForOrderPage(int id, string email)
        {
            OrderAboutManageOrders model = new OrderAboutManageOrders();
            var a = await _context.Orders.Include(p => p.Filial).Include(p => p.SborshikUser).Include(p => p.SellerUser).Where(p => p.OrderNumber == id).FirstAsync();
            model.ID_Order = a.OrderNumber;
            model.WorkerUserID = _context.Users.Where(p => p.Login == email).First().IdUser;
            model.StatusOrder = a.Status;
            model.FilialAddress = a.Filial.AddressFilial;
            model.FilialName = a.Filial.NameFilial;
            model.DateOrder = a.DateOrder;
            if (a.DateReadyToExtradition != null)
            {
                model.DateReadyToExtradition = a.DateReadyToExtradition;
                model.Sborshik = a.SborshikUser.FirstName + " " + a.SborshikUser.LastName;
            }
            if (a.DateExtradition != null)
            {
                model.DateExtradition = a.DateExtradition;
                model.Vidal = a.SellerUser.FirstName + " " + a.SellerUser.LastName;
            }
            return model;
        }

        [HttpDelete("CancelOrder/{id}")]
        public async Task<ActionResult> CancelOrder(int id)
        {
            var Order = await _context.Orders.FindAsync(id);
            Order.Status = 4;
            _context.Orders.Update(Order);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPut("NextStatus/{idOrder}/{idWorker}")]
        public async Task<ActionResult> NextStatus(int idOrder, int idWorker)
        {
            var order = await _context.Orders.FindAsync(idOrder);
            order.Status = order.Status + 1;
            if(order.Status-1 == 1)
            {
                order.DateReadyToExtradition = DateTime.Now;
                order.SborshikUserId = idWorker;
            }
            if(order.Status-1 == 2)
            {
                order.DateExtradition = DateTime.Now;
                order.SellerUserId = idWorker;
            }
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("BackStatus/{idOrder}")]
        public async Task<ActionResult> BackStatus(int idOrder)
        {
            var order = await _context.Orders.FindAsync(idOrder);
            order.Status = order.Status - 1;
            if (order.Status + 1 == 2)
            {
                order.DateReadyToExtradition = null;
                order.SborshikUserId = null;
            }
            if (order.Status + 1 == 3)
            {
                order.DateExtradition = null;
                order.SellerUserId = null;
            }
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
