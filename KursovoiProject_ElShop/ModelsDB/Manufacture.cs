using System;
using System.Collections.Generic;

namespace KursovoiProject_ElShop
{
    public partial class Manufacture
    {
        public Manufacture()
        {
            Goods = new HashSet<Good>();
        }

        public int IdManufacture { get; set; }
        public string NameManufacture { get; set; } = null!;

        public virtual ICollection<Good> Goods { get; set; }
    }
}
