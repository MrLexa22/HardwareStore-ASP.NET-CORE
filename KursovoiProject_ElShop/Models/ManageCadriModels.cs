using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace KursovoiProject_ElShop.Models
{
    public class BeforeDeleteCadri
    {
        public int ID_Worker { get; set; }

        [Display(Name = "Фамилия Имя сотрудника")]
        public string FI { get; set; }

        [Display(Name = "Приказ об увольнении")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Минимум 3 символа, максимум 30")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string PrikazObUval { get; set; }
    }
    public class RolesWithBoolean
    {
        public int ID_Role { get; set; }
        public string NameRole { get; set; }
        public bool IsSelected { get; set; }
    }
    public class FilialsWithBoolean
    {
        public int ID_Filial { get; set; }
        public string NameFilial { get; set; }
        public bool IsSelected { get; set; }
        public bool IsAvalib { get; set; }
    }

    public class AddEditCadri
    {
        [ValidateNever]
        public int ID_Worker { get; set; }

        [Display(Name = "Фамилия сотрудника")]
        [RegularExpression(@"^[А-Яа-яЁё ]+$", ErrorMessage = "Некорректное имя")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Неккоректное имя")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string First_Name { get; set; }

        [Display(Name = "Имя сотрудника")]
        [RegularExpression(@"^[А-Яа-яЁё ]+$", ErrorMessage = "Некорректное имя")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Неккоректное имя")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Last_Name { get; set; }

        [Display(Name = "ИНН сотрудника")]
        [RegularExpression(@"[0-9]{12}", ErrorMessage = "Неккоректный ИНН")]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "Неккоректный ИНН")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string INN { get; set; }

        [Display(Name = "СНИЛС сотрудника")]
        [RegularExpression(@"[0-9]{3}-[0-9]{3}-[0-9]{3} [0-9]{2}", ErrorMessage = "Неккоректный СНИЛС")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "Неккоректный СНИЛС")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string SNILS { get; set; }

        [Display(Name = "Серия паспорта сотрудника")]
        [RegularExpression(@"[0-9]{4}", ErrorMessage = "Неккоректная серия паспорта")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Неккоректная серия паспорта")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string SeriaPasport { get; set; }

        [Display(Name = "Номер паспорта сотрудника")]
        [RegularExpression(@"[0-9]{6}", ErrorMessage = "Неккоректный номер паспорта")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Неккоректный номер паспорта")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string NomerPasport { get; set; }

        [Display(Name = "Дата рождения сотрудника")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Адрес регистрации сотрудника")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string AddressRegistration { get; set; }

        [Display(Name = "Email пользователя")]
        [EmailAddress(ErrorMessage = "Здесь нужно ввести корректный Email")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Login { get; set; }

        [Display(Name = "Приказ о принятии на работы")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string PrikazOPrinat { get; set; }

        [Display(Name = "Приказ об увольнении")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string? PrikazObUval { get; set; }

        [ValidateNever]
        public string Password { get; set; }

        [Display(Name = "Номер телефона сотрудника")]
        [RegularExpression(@"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?[0-9]{3}-[0-9]{2}-[0-9]{2})", ErrorMessage = "Некорректный телефон")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [DataType(DataType.PhoneNumber)]
        public string Telefon { get; set; }

        [ValidateNever]
        public List<RolesWithBoolean> roles { get; set; }

        [ValidateNever]
        public List<FilialsWithBoolean> filials { get; set; }
    }
    public class InfoCadri
    {
        public int ID_Worker { get; set; } 
        public string Familia { get; set; }
        public string Ima { get; set; }
        public string phoneNumber { get; set; }
        public string PrikazOPrieme { get; set; }
        public string DateOfBirth { get; set; }
        public string? PrikazObUval { get; set; }
        public List<Filial> listFilial { get; set; }
        public List<Role> listRoles { get; set; }
        public bool avail { get; set; }
    }
    public class IndexPageCadri
    {
        public int statusSite { get; set; }
        public List<Filial> filials { get; set; }
        public List<InfoCadri> listCadri { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public FilterViewModel_AddsSites FilterViewModel { get; set; }
    }
}
