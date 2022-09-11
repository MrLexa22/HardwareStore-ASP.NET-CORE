using System;
using System.Collections.Generic;

namespace KursovoiProject_ElShop
{
    public partial class WorkersBankCadri
    {
        public int IdBankWorker { get; set; }
        public int UserId { get; set; }
        public int? BankId { get; set; }
        public string Innworker { get; set; } = null!;
        public string? SchetWorker { get; set; }
        public string Snils { get; set; } = null!;
        public string PrikazOprieme { get; set; } = null!;
        public string SeriaPasporta { get; set; } = null!;
        public string NomerPasporta { get; set; } = null!;

        public virtual Bank? Bank { get; set; }
        public virtual User User { get; set; } = null!;
    }
}
