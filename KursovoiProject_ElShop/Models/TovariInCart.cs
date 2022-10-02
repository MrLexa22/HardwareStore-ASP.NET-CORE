using System.ComponentModel.DataAnnotations;

namespace KursovoiProject_ElShop.Models
{
    public class FilialsWithBoll
    {
        public int IdFilial { get; set; }
        public string NameFilial { get; set; } = null!;
        public string AddressFilial { get; set; } = null!;
        public bool Availeble { get; set; }
        public bool Selected { get; set; }
    }

    public class ContactInformation
    {
        [Display(Name = "Имя")]
        [RegularExpression(@"^[А-Яа-яЕё., ]+$", ErrorMessage = "Некорректное имя")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Имя не должно содержать так много символов")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Contact_Name { get; set; }

        [Display(Name = "Электронная почта")]
        [EmailAddress(ErrorMessage = "Здесь нужно ввести корректный Email")]
        [StringLength(60, MinimumLength = 1, ErrorMessage = "Email не должно содержать так много символов")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Contact_Email { get; set; }

        [Display(Name = "Телефон")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [RegularExpression(@"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?[0-9]{3}-[0-9]{2}-[0-9]{2})", ErrorMessage = "Некорректный телефон")]
        public string Contact_Telefon { get; set; }
    }

    public class availibleTovari
    {
        public int GoodsId { get; set; }
        public int Count { get; set; }
        public int count_filial { get; set; }
        public virtual Good Goods { get; set; } = null!;
    }

    public class informationPreOformleine
    {
        public List<AddsSite> Right1Adds { get; set; }
        public List<AddsSite> Right2Adds { get; set; }
        public List<availibleTovari> tovaris_true { get; set; }
        public List<availibleTovari> tovaris_false { get; set; }
        public ContactInformation contacts { get; set; }
        public Filial Filial { get; set; }
        public double summa { get; set; }
    }
    public class TovariInCart
    {
        public List<Korzina> list_tovari { get; set; }
        public List<FilialsWithBoll> list_filial { get; set; }
        public int SelectedFilial { get; set; }
        public List<AddsSite> Right1Adds { get; set; }
        public List<AddsSite> Right2Adds { get; set; }
        public double summa { get; set; }
        public int IDUser { get; set; }
        public ContactInformation  contacts { get; set; }
    }
    public class ModelOrder
    {
        public List<AddsSite> Right1Adds { get; set; }
        public List<AddsSite> Right2Adds { get; set; }
        public double summa { get; set; }
        public ContactInformation contacts { get; set; }
        public Order OrderInformation { get; set; }
    }
}
