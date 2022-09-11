using System;
using System.Collections.Generic;

namespace KursovoiProject_ElShop
{
    public partial class Role
    {
        public Role()
        {
            UsersRoles = new HashSet<UsersRole>();
        }

        public int IdRole { get; set; }
        public string NameRole { get; set; } = null!;
        public bool OnlyRead { get; set; }

        public virtual ICollection<UsersRole> UsersRoles { get; set; }
    }
}
