using System;
using System.Collections.Generic;

namespace KursovoiProject_ElShop
{
    public partial class WorkersPost
    {
        public int IdWorker { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
        public double Rate { get; set; }

        public virtual Post Post { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
