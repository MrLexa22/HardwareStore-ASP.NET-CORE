using System;
using System.Collections.Generic;

namespace KursovoiProject_ElShop
{
    public partial class Order
    {
        public Order()
        {
            OrderContainers = new HashSet<OrderContainer>();
        }

        public int OrderNumber { get; set; }
        public int UserId { get; set; }
        public int FilialId { get; set; }
        public int Status { get; set; }
        public DateTime DateOrder { get; set; }
        public DateTime? DateReadyToExtradition { get; set; }
        public DateTime? DateExtradition { get; set; }
        public decimal ItogSumma { get; set; }

        public virtual Filial Filial { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual ICollection<OrderContainer> OrderContainers { get; set; }
    }
}
