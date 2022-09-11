using System;
using System.Collections.Generic;

namespace KursovoiProject_ElShop
{
    public partial class Order
    {
        public int OrderNumber { get; set; }
        public int UserId { get; set; }
        public int GoodsId { get; set; }
        public int Count { get; set; }
        public int TypeDelivery { get; set; }
        public string? Address { get; set; }
        public int? FilialId { get; set; }
        public int Status { get; set; }
        public DateTime DateOrder { get; set; }
        public DateTime? DateDostavki { get; set; }

        public virtual Filial? Filial { get; set; }
        public virtual Good Goods { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
