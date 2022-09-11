using System;
using System.Collections.Generic;

namespace KursovoiProject_ElShop
{
    public partial class Korzina
    {
        public int IdKorzinaGood { get; set; }
        public int GoodsId { get; set; }
        public int UserId { get; set; }
        public int Count { get; set; }

        public virtual Good Goods { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
