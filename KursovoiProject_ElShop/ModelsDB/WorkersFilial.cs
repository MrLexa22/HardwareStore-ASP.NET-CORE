using System;
using System.Collections.Generic;

namespace KursovoiProject_ElShop
{
    public partial class WorkersFilial
    {
        public int IdWorkerFilial { get; set; }
        public int FilialId { get; set; }
        public int UserId { get; set; }
        public bool CanLookAddressOrders { get; set; }

        public virtual Filial Filial { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
