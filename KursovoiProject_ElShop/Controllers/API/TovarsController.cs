using KursovoiProject_ElShop.Controllers.Validation;
using KursovoiProject_ElShop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KursovoiProject_ElShop.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class TovarsController : ControllerBase
    {
        private readonly ElShopContext _context;
        public TovarsController(ElShopContext context)
        {
            _context = context;
        }

        [HttpGet("{email}")]
        public async Task<ActionResult<IEnumerable<ListTovariManageTovari>>> GetTovari(string email)
        {
            int ID_User = _context.Users.Where(p => p.Login == email).First().IdUser;
            var filialsWork = _context.WorkersFilials.Where(p => p.UserId == ID_User).ToList();
            List<ListTovariManageTovari> list = new List<ListTovariManageTovari>();
            var filial = _context.Filials.ToList();
            var listTovars = _context.Goods.Include(p => p.Manufacture).Include(p => p.Categori).Include(p => p.GoodsFilials).ToList();
            foreach (var item in listTovars)
            {
                ListTovariManageTovari h = new ListTovariManageTovari();
                h.ID_Good = item.IdGood;
                h.NameGood = item.Manufacture.NameManufacture + ". " + item.Name;
                h.NameCategory = item.Categori.NameCategori;
                h.ID_Category = item.Categori.IdCategori;
                h.ManufactID = item.ManufactureId;
                h.Cost = item.Cost;
                if (item.FtppathImage == null)
                    h.FTTPPathImage = BaseAddresse.Server + "TovariImages/NoImageTovar.jpg";
                else
                    h.FTTPPathImage = BaseAddresse.Server + "TovariImages/" + item.FtppathImage;
                h.IsAvalib = item.IsAvaliable;
                h.Nalichielist = new List<NalichFilialManageTovari>();
                foreach(var a in filialsWork)
                {
                    NalichFilialManageTovari j = new NalichFilialManageTovari();
                    j.ID_Filial = a.FilialId;
                    j.CountInFilial = listTovars.Where(p => p.IdGood == item.IdGood).First().GoodsFilials.Where(p => p.FilialId == a.FilialId).First().CountSklad;
                    j.NameFilial = filial.Where(p => p.IdFilial == a.FilialId).First().NameFilial;
                    h.Nalichielist.Add(j);
                }
                list.Add(h);
            }
            return list;
        }

        [HttpGet("GetTovarByID/{id}")]
        public async Task<ActionResult<Good>> GetTovarByID(int id)
        {
            return _context.Goods.Where(p => p.IdGood == id).First();
        }

        [HttpGet("GetTovarByIDToChangeImage/{id}")]
        public async Task<ActionResult<Good>> GetTovarByIDToChangeImage(int id)
        {
            Good good = _context.Goods.Where(p => p.IdGood == id).First();
            if (good.FtppathImage == null)
                good.FtppathImage = BaseAddresse.Server + "TovariImages/NoImageTovar.jpg";
            else
                good.FtppathImage = BaseAddresse.Server + "TovariImages/" + good.FtppathImage;
            return _context.Goods.Where(p => p.IdGood == id).First();
        }

        [HttpGet("GetToDelete/{id}")]
        public async Task<ActionResult<ListTovariManageTovari>> GetTovarToDelete(int id)
        {
            var tovar = _context.Goods.Include(p=>p.Manufacture).Where(p => p.IdGood == id).First();
            ListTovariManageTovari result = new ListTovariManageTovari();
            result.ID_Good = tovar.IdGood;
            result.NameGood = tovar.Manufacture.NameManufacture+". "+tovar.Name;
            return result;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CloseTovar(int id)
        {
            var tovar = _context.Goods.Where(p => p.IdGood == id).First();
            tovar.IsAvaliable = false;
            _context.Update(tovar);
            var GoodsInCart = _context.Korzinas.Where(p => p.GoodsId == id).ToList();
            foreach(var a in GoodsInCart)
            {
                _context.Korzinas.Remove(a);
            }
            _context.SaveChanges();
            return NoContent();
        }

        [HttpGet("ReturnTovar/{id}")]
        public async Task<IActionResult> ReturnTovar(int id)
        {
            var tovar = _context.Goods.Where(p => p.IdGood == id).First();
            tovar.IsAvaliable = true;
            _context.Update(tovar);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpGet("GetFilialsCount/{email}/{idTovar}")]
        public async Task<ActionResult<IEnumerable<NalichFilialManageTovari>>> GetFilialsCount(string email, int idTovar)
        {
            List<NalichFilialManageTovari> result = new List<NalichFilialManageTovari>();
            if(idTovar == 0)
            {
                int idUser = _context.Users.Where(p => p.Login == email).First().IdUser;
                var list = _context.WorkersFilials.Include(p => p.Filial).Where(p => p.UserId == idUser).ToList();
                foreach(var a in list)
                {
                    NalichFilialManageTovari h = new NalichFilialManageTovari();
                    h.ID_Filial = a.Filial.IdFilial;
                    h.NameFilial = a.Filial.NameFilial;
                    h.CountInFilial = 0;
                    result.Add(h);
                }
            }
            else
            {
                int idUser = _context.Users.Where(p => p.Login == email).First().IdUser;
                var list = _context.GoodsFilials.Include(p=>p.Filial).Where(p=>p.GoodsId == idTovar).ToList();
                var list2 = _context.WorkersFilials.Where(p => p.UserId == idUser).ToList();
                foreach(var a in list)
                {
                    if(list2.Where(p=>p.FilialId == a.FilialId).Count() > 0)
                    {
                        NalichFilialManageTovari h = new NalichFilialManageTovari();
                        h.ID_Filial = a.Filial.IdFilial;
                        h.NameFilial = a.Filial.NameFilial;
                        h.CountInFilial = a.CountSklad;
                        result.Add(h);
                    }
                }

            }
            return result;
        }

        [HttpGet("CheckExistTovar/{nameTovar}/{idTovar}")]
        public async Task<ActionResult<string>> CheckExistTovar(string nameTovar, int idTovar)
        {
            string result = "false";
            var list = _context.Goods.ToList();
            if (list.Where(p => p.Name.ToLower().Replace('/', ' ').Replace('\\', ' ').Trim() == nameTovar.ToLower().Trim() && p.IdGood != idTovar).Count() > 0)
                result = "false";
            else
                result = "true";
            return result;
        }

        [HttpPost("CreateOrEditTovar")]
        public async Task<ActionResult<Good>> CreateOrEditTovar(AddEditTovar model)
        {
            Good good = new Good();
            good.CategoriId = model.Category_ID;
            good.ManufactureId = model.Proizvoditel_ID;
            good.Name = model.Name_Tovar;
            good.Description = model.Description;
            good.Cost = Convert.ToDouble(model.Cost);
            if(model.ID_Tovar == 0)
            {
                good.FtppathImage = null;
                good.IsAvaliable = true;
                _context.Goods.Add(good);
                _context.SaveChanges();
                var listGoodsFilials = _context.GoodsFilials.Where(p => p.GoodsId == good.IdGood).ToList();
                foreach(var a in model.Nalichielist)
                {
                    GoodsFilial goodsFilial = listGoodsFilials.Where(p => p.FilialId == a.ID_Filial && p.GoodsId == good.IdGood).First(); ;
                    goodsFilial.CountSklad = a.CountInFilial;
                    _context.Update(goodsFilial);
                }
                _context.SaveChanges();
            }
            else
            {
                good.IdGood = model.ID_Tovar;
            }
            return good;
        }
    }
}
