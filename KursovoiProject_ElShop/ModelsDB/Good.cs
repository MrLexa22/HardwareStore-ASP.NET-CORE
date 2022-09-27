using System;
using System.Collections.Generic;

namespace KursovoiProject_ElShop
{
    public partial class Good
    {
        public Good()
        {
            GoodsFilials = new HashSet<GoodsFilial>();
            Korzinas = new HashSet<Korzina>();
            OrderContainers = new HashSet<OrderContainer>();
        }

        public int IdGood { get; set; }
        public double Cost { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? FtppathImage { get; set; }
        public int CategoriId { get; set; }
        public int ManufactureId { get; set; }
        public bool IsAvaliable { get; set; }

        public virtual Category Categori { get; set; } = null!;
        public virtual Manufacture Manufacture { get; set; } = null!;
        public virtual ICollection<GoodsFilial> GoodsFilials { get; set; }
        public virtual ICollection<Korzina> Korzinas { get; set; }
        public virtual ICollection<OrderContainer> OrderContainers { get; set; }
    }
}
