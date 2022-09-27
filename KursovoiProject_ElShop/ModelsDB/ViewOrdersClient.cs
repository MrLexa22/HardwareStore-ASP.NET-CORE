using System;
using System.Collections.Generic;

namespace KursovoiProject_ElShop
{
    public partial class ViewOrdersClient
    {
        public int OrderNumber { get; set; }
        public int UserId { get; set; }
        public int FilialId { get; set; }
        public int Status { get; set; }
        public DateTime DateOrder { get; set; }
        public DateTime? DateReadyToExtradition { get; set; }
        public DateTime? DateExtradition { get; set; }
        public decimal ItogSumma { get; set; }
        public int IdFilial { get; set; }
        public string NameFilial { get; set; } = null!;
        public string AddressFilial { get; set; } = null!;
        public bool Availeble { get; set; }
        public string? Category { get; set; }
        public decimal? Nds { get; set; }
    }
}
