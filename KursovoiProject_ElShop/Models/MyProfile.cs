using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace KursovoiProject_ElShop.Models
{
    public class ChangeInformation
    {
        public int Id { get; set; }

        [Display(Name = "Email пользователя")]
        [Remote(action: "CheckLogin", controller: "ValidationUsers", AdditionalFields = "Id", ErrorMessage = "Email уже используется")]
        [EmailAddress(ErrorMessage = "Здесь нужно ввести корректный Email")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Email { get; set; }

        [Display(Name = "Имя пользователя")]
        [RegularExpression(@"^[А-Яа-яЁё]+$", ErrorMessage = "Некорректное имя")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Неккоректное имя")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Ima { get; set; }

        [Display(Name = "Фамилия пользователя")]
        [RegularExpression(@"^[А-Яа-яЁё]+$", ErrorMessage = "Некорректная фамилия")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Неккоректная фамилия")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Familia { get; set; }

        [Display(Name = "Телефон")]
        [Remote(action: "CheckPhoneNumber", controller: "ValidationUsers", AdditionalFields = "Id", ErrorMessage = "Данный номер телефона уже есть в базе")]
        [RegularExpression(@"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?[0-9]{3}-[0-9]{2}-[0-9]{2})", ErrorMessage = "Некорректный телефон")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [DataType(DataType.PhoneNumber)]
        public string Telefon { get; set; }
    }
    public class ChangePassword
    {
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Old_password { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(30, MinimumLength = 8, ErrorMessage = "Минимум 8 символов")]
        public string New_password { get; set; }

        [Compare("New_password", ErrorMessage = "Пароль и подтверждение пароля должны совпадать!")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Repeat_password { get; set; }
    }
    public class MyProfile
    {
        public List<AddsSite> Right1Adds { get; set; }
        public List<AddsSite> Right2Adds { get; set; }
        public User user { get; set; }
        public List<UsersRole> Roles { get; set; }
    }
}
