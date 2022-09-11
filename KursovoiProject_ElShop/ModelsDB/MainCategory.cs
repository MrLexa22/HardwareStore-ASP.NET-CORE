using System;
using System.Collections.Generic;

namespace KursovoiProject_ElShop
{
    public partial class MainCategory
    {
        public int IdMainCategori { get; set; }
        public int CategoriId { get; set; }

        public virtual Category Categori { get; set; } = null!;
    }
}
