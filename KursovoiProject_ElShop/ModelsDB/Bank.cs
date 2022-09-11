using System;
using System.Collections.Generic;

namespace KursovoiProject_ElShop
{
    public partial class Bank
    {
        public Bank()
        {
            WorkersBankCadris = new HashSet<WorkersBankCadri>();
        }

        public int IdBank { get; set; }
        public string NameBank { get; set; } = null!;
        public string BikBank { get; set; } = null!;
        public string KorrSchetBank { get; set; } = null!;
        public string InnBank { get; set; } = null!;
        public string KppBank { get; set; } = null!;

        public virtual ICollection<WorkersBankCadri> WorkersBankCadris { get; set; }
    }
}
