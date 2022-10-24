using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace KursovoiProject_ElShop.Models
{
    public class changeImageTovar
    {
        [ValidateNever]
        public int ID_Tovar { get; set; }
        [ValidateNever]
        public string NameTovar { get; set; }

        [ValidateNever]
        [Display(Name = "Картинка товара")]
        public string? FTTPPathImage { get; set; }
        public IFormFile uploadedImage { get; set; }
    }
    public class AddEditTovar
    {
        [ValidateNever]
        public int ID_Tovar { get; set; }

        [Display(Name = "Категория товара")]
        public int Category_ID { get; set; }

        [Display(Name = "Производитель товара")]
        public int Proizvoditel_ID { get; set; }

        [Display(Name = "Наименование товара")]
        [StringLength(150, MinimumLength = 3)]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Name_Tovar { get; set; }

        [Display(Name = "Описание товара")]
        [StringLength(1000, MinimumLength = 10)]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Description { get; set; }

        [ValidateNever]
        [Display(Name = "Картинка товара")]
        public string? FTTPPathImage { get; set; }

        [Display(Name = "Цена товара")]
        public string Cost { get; set; }
        public List<NalichFilialManageTovari> Nalichielist { get; set; }
        public List<Manufacture> manufactures { get; set; }
        public List<Category> categories { get; set; }
    }
    public class NalichFilialManageTovari
    {
        public int ID_Filial { get; set; }
        public string NameFilial { get; set; }
        public int CountInFilial { get; set; }
    }
    public class ListTovariManageTovari
    {
        public int ID_Good { get; set; }
        public string NameGood { get; set; }
        public int ManufactID { get; set; }
        public string NameCategory { get; set; }
        public int ID_Category { get; set; }
        public string FTTPPathImage { get; set; }
        public Double Cost { get; set; }
        public List<NalichFilialManageTovari> Nalichielist { get; set; }
        public bool IsAvalib { get; set; } 
    }
    public class IndexManageTovari
    {
        public List<Manufacture> manufactures { get; set; }
        public List<Category> categories { get; set; }
        public List<ListTovariManageTovari> tovari {get;set;}
        public PageViewModel PageViewModel { get; set; }
        public FilterViewModel_ManageTovari FilterViewModel_ManageTovari { get; set; }
    }

    public class FilterViewModel_ManageTovari
    {
        public FilterViewModel_ManageTovari(string? search, int? typesort, int? selcategory, int? proizvoditel, int? visibal)
        {
            Search = search;
            Typesort = typesort;
            Selcategory = selcategory;
            Proizvoditel = proizvoditel;
            Visibal = visibal;
        }
        public string? Search { get; private set; }
        public int? Typesort { get; private set; }
        public int? Selcategory { get; private set; }
        public int? Proizvoditel { get; private set; }
        public int? Visibal { get; private set; }
    }
}
