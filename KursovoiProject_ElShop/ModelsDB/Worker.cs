using System;
using System.Collections.Generic;

namespace KursovoiProject_ElShop
{
    public partial class Worker
    {
        public int IdWorker { get; set; }
        public int UserId { get; set; }
        public string Innworker { get; set; } = null!;
        public string Snils { get; set; } = null!;
        public string PrikazOprieme { get; set; } = null!;
        public string SeriaPasporta { get; set; } = null!;
        public string NomerPasporta { get; set; } = null!;
        public DateOnly DateOfBirth { get; set; }
        public string AddressRegistration { get; set; } = null!;
        public string? PrikazObUvolnenii { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
