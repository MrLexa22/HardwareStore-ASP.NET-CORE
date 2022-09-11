using System;
using System.Collections.Generic;

namespace KursovoiProject_ElShop
{
    public partial class ViewGoodsWithManufacture
    {
        public int IdGood { get; set; }
        public double Cost { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? FtppathImage { get; set; }
        public int CategoriId { get; set; }
        public int ManufactureId { get; set; }
        public bool IsAvaliable { get; set; }
        public int IdManufacture { get; set; }
        public string NameManufacture { get; set; } = null!;
        public string? NameWithManufacture { get; set; }
    }
}
