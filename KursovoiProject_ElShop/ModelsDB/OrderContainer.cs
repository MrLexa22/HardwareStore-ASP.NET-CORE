using System;
using System.Collections.Generic;

namespace KursovoiProject_ElShop
{
    public partial class OrderContainer
    {
        public int ContainerId { get; set; }
        public int OrderNumberId { get; set; }
        public int GoodsId { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }

        public virtual Good Goods { get; set; } = null!;
        public virtual Order OrderNumber { get; set; } = null!;
    }
}
