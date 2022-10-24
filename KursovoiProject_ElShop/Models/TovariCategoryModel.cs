using ElSt.Models;
using System.ComponentModel.DataAnnotations;

namespace KursovoiProject_ElShop.Models
{
    public class TovariUsersModel
    {
        public TovariUsersModel()
        {
        }

        public TovariUsersModel(int idGood, double cost, string name, string description, string? ftppathImage, int categoriId, int manufactureId, bool isAvaliable, int idManufacture, string nameManufacture, string? nameWithManufacture, int isInCart, string nameCategory)
        {
            IdGood = idGood;
            Cost = cost;
            Name = name;
            Description = description;
            FtppathImage = ftppathImage;
            CategoriId = categoriId;
            ManufactureId = manufactureId;
            IsAvaliable = isAvaliable;
            IdManufacture = idManufacture;
            NameManufacture = nameManufacture;
            NameWithManufacture = nameWithManufacture;
            IsInCart = isInCart;
        }

        public TovariUsersModel(string nameCategory, int idGood, double cost, string name, string description, string? ftppathImage, int categoriId, int manufactureId, bool isAvaliable, int idManufacture, string nameManufacture, string? nameWithManufacture, int isInCart, List<AddsSite> right1Adds, List<AddsSite> right2Adds)
        {
            NameCategory = nameCategory;
            IdGood = idGood;
            Cost = cost;
            Name = name;
            Description = description;
            FtppathImage = ftppathImage;
            CategoriId = categoriId;
            ManufactureId = manufactureId;
            IsAvaliable = isAvaliable;
            IdManufacture = idManufacture;
            NameManufacture = nameManufacture;
            NameWithManufacture = nameWithManufacture;
            IsInCart = isInCart;
            Right1Adds = right1Adds;
            Right2Adds = right2Adds;
        }
        public string NameCategory { get; set; }

        public int IdGood { get; set; }
        public double Cost { get; set; }
        public string Name { get; set; } = null!;

        [DataType(DataType.MultilineText)]
        public string Description { get; set; } = null!;
        public string? FtppathImage { get; set; }
        public int CategoriId { get; set; }
        public int ManufactureId { get; set; }
        public bool IsAvaliable { get; set; }
        public int IdManufacture { get; set; }
        public string NameManufacture { get; set; } = null!;
        public string? NameWithManufacture { get; set; }
        public int IsInCart { get; set; }
        public List<AddsSite> Right1Adds { get; set; }
        public List<AddsSite> Right2Adds { get; set; }
        public List<FilialsNalichTovar> filials { get; set; }

    }

    public class ManufacturesList
    {
        public int ManufactureId { get; set; }
        public string NameManufacture { get; set; }
        public bool Selected { get; set; }
    }
    public class TovariCategoryModel
    {
        public List<AddsSite> Right1Adds { get; set; }
        public List<AddsSite> Right2Adds { get; set; }
        public string NameCategory { get; set; }
        public string CountTovar { get; set; }
        public List<ManufacturesList> list_manufactures { get; set; }
        public int IDCategory { get; set; }
        public List<TovariUsersModel> list_tovaris { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public FilterViewModel_Tovari FilterViewModel { get; set; }
        public string search { get; set; }
        public int? Manufacture_id { get; set; }
    }
}
