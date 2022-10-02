using System;
using System.Collections.Generic;

namespace KursovoiProject_ElShop
{
    public partial class User
    {
        public User()
        {
            AddsSites = new HashSet<AddsSite>();
            Korzinas = new HashSet<Korzina>();
            OrderSborshikUsers = new HashSet<Order>();
            OrderSellerUsers = new HashSet<Order>();
            OrderUsers = new HashSet<Order>();
            UsersRoles = new HashSet<UsersRole>();
            Workers = new HashSet<Worker>();
            WorkersFilials = new HashSet<WorkersFilial>();
        }

        public int IdUser { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public double IsAvalible { get; set; }

        public virtual ICollection<AddsSite> AddsSites { get; set; }
        public virtual ICollection<Korzina> Korzinas { get; set; }
        public virtual ICollection<Order> OrderSborshikUsers { get; set; }
        public virtual ICollection<Order> OrderSellerUsers { get; set; }
        public virtual ICollection<Order> OrderUsers { get; set; }
        public virtual ICollection<UsersRole> UsersRoles { get; set; }
        public virtual ICollection<Worker> Workers { get; set; }
        public virtual ICollection<WorkersFilial> WorkersFilials { get; set; }
    }
}
