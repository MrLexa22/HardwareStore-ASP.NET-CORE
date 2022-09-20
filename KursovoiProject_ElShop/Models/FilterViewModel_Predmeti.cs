using KursovoiProject_ElShop.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ElSt.Models
{
    public class FilterViewModel_Tovari
    {
        public FilterViewModel_Tovari(List<ManufacturesList> manufactries, int? manufacte, int IDCategory, double? min, double? max, int? typesort)
        {
            // устанавливаем начальный элемент, который позволит выбрать всех
            manufactries.Insert(0, new ManufacturesList { NameManufacture = "Любой", ManufactureId = 0, Selected = false });
            Manufacturies = new SelectList(manufactries, "ManufactureId", "NameManufacture", manufacte);
            SelectedManufacture = manufacte;
            SelectedIDCategory = IDCategory;
            Min = min;
            Max = max;
            TypeSort = typesort;
        }
        public SelectList Manufacturies { get; private set; } // список компаний
        public int? SelectedManufacture { get; private set; }   // выбранная компания
        public double? Min { get; private set; }
        public double? Max { get; private set; }
        public int? TypeSort { get; private set; }
        public int SelectedIDCategory { get; private set; }
    }
}
