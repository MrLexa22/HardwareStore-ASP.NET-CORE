using System;
using System.Collections.Generic;

namespace KursovoiProject_ElShop
{
    public partial class Category
    {
        public Category()
        {
            Goods = new HashSet<Good>();
            MainCategories = new HashSet<MainCategory>();
        }

        public int IdCategori { get; set; }
        public string NameCategori { get; set; } = null!;

        public virtual ICollection<Good> Goods { get; set; }
        public virtual ICollection<MainCategory> MainCategories { get; set; }
    }
}
