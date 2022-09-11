using System;
using System.Collections.Generic;

namespace KursovoiProject_ElShop
{
    public partial class Filial
    {
        public Filial()
        {
            GoodsFilials = new HashSet<GoodsFilial>();
            WorkersFilials = new HashSet<WorkersFilial>();
        }

        public int IdFilial { get; set; }
        public string NameFilial { get; set; } = null!;
        public string AddressFilial { get; set; } = null!;
        public bool Availeble { get; set; }

        public virtual ICollection<GoodsFilial> GoodsFilials { get; set; }
        public virtual ICollection<WorkersFilial> WorkersFilials { get; set; }
    }
}
