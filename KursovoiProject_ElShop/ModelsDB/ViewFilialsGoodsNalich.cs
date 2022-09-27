using System;
using System.Collections.Generic;

namespace KursovoiProject_ElShop
{
    public partial class ViewFilialsGoodsNalich
    {
        public int IdGood { get; set; }
        public int IdFilial { get; set; }
        public int CountSklad { get; set; }
        public string NameFilial { get; set; } = null!;
        public string AddressFilial { get; set; } = null!;
        public string? Nalich { get; set; }
    }
}
