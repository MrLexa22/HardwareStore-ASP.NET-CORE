using System;
using System.Collections.Generic;

namespace KursovoiProject_ElShop
{
    public partial class GoodsFilial
    {
        public int IdGoodsFilial { get; set; }
        public int GoodsId { get; set; }
        public int FilialId { get; set; }
        public int CountSklad { get; set; }

        public virtual Filial Filial { get; set; } = null!;
        public virtual Good Goods { get; set; } = null!;
    }
}
