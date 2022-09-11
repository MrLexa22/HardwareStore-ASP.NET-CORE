using System;
using System.Collections.Generic;

namespace KursovoiProject_ElShop
{
    public partial class User
    {
        public User()
        {
            Korzinas = new HashSet<Korzina>();
            UsersRoles = new HashSet<UsersRole>();
            WorkersBankCadris = new HashSet<WorkersBankCadri>();
            WorkersFilials = new HashSet<WorkersFilial>();
            WorkersPosts = new HashSet<WorkersPost>();
        }

        public int IdUser { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public double IsAvalible { get; set; }

        public virtual ICollection<Korzina> Korzinas { get; set; }
        public virtual ICollection<UsersRole> UsersRoles { get; set; }
        public virtual ICollection<WorkersBankCadri> WorkersBankCadris { get; set; }
        public virtual ICollection<WorkersFilial> WorkersFilials { get; set; }
        public virtual ICollection<WorkersPost> WorkersPosts { get; set; }
    }
}
