using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KursovoiProject_ElShop;
using System.Diagnostics;
using KursovoiProject_ElShop.Models;

namespace KursovoiProject_ElShop.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ElShopContext _context;
        public UsersController(ElShopContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            return await _context.Users.ToListAsync();
        }

        [HttpGet("GetRolesUser/{idUser}")]
        public async Task<ActionResult<IEnumerable<RolesWithBoolean>>> GetRoles(int idUser)
        {
            if(idUser > 0)
                idUser = _context.Workers.Where(p => p.IdWorker == idUser).First().UserId;
            var listRoles = _context.Roles.ToList();
            List<UsersRole> UsersRoles = new List<UsersRole>();
            if (idUser > 0)
                UsersRoles = _context.UsersRoles.Where(p=>p.UserId == idUser).ToList();
            List<RolesWithBoolean> result = new List<RolesWithBoolean>();
            if(idUser == 0)
            {
                foreach(var a in listRoles)
                {
                    RolesWithBoolean h = new RolesWithBoolean();
                    h.ID_Role = a.IdRole;
                    h.NameRole = a.NameRole;
                    h.IsSelected = false;
                    result.Add(h);
                }
            }
            else
            {
                foreach (var a in listRoles)
                {
                    RolesWithBoolean h = new RolesWithBoolean();
                    h.ID_Role = a.IdRole;
                    h.NameRole = a.NameRole;
                    if(UsersRoles.Where(p=>p.RoleId == a.IdRole).Count() > 0)
                        h.IsSelected = true;
                    else
                        h.IsSelected = false;
                    result.Add(h);
                }
            }
            return result;
        }


        [HttpGet("CheckLogin/{id}/{login}")]
        public bool CheckEmail(int id, string login)
        {
            bool result = false;
            using (var context = new ElShopContext())
            {
                var conn = context.Database.GetDbConnection();
                conn.Open();
                var command = conn.CreateCommand();
                string query = @$"SET @p0='{id}'; SET @p1='{login}'; SELECT `CheckLoginExist`(@p0, @p1) AS `CheckLoginExist`;";
                command.CommandText = query;
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var title = reader.GetBoolean(0);
                    result = title;
                }
                conn.Close();
            }
            return result;
        }

        [HttpGet("CheckPhone/{id}/{phone}")]
        public bool CheckPhone(int id, string phone)
        {
            bool result = false;
            using (var context = new ElShopContext())
            {
                var conn = context.Database.GetDbConnection();
                conn.Open();
                var command = conn.CreateCommand();
                string query = @$"SET @p0='{id}'; SET @p1='{phone}'; SELECT `CheckPhoneExist`(@p0, @p1) AS `CheckPhoneExist`;";
                command.CommandText = query;
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var title = reader.GetBoolean(0);
                    result = title;
                }
                Debug.WriteLine(result.ToString());
                conn.Close();
            }
            return result;
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<User>> GetUserByEmail(string email)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.Where(p=>p.Login == email).FirstAsync();

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.IdUser)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
          if (_context.Users == null)
          {
              return Problem("Entity set 'ElShopContext.Users'  is null.");
          }
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.IdUser }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.IdUser == id)).GetValueOrDefault();
        }
    }
}
