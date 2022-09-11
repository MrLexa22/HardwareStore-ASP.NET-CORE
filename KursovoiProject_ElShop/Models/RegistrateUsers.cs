using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace KursovoiProject_ElShop.Models
{
    public class RegistrateUsers
    {
        //public string Id { get; set; }


        [Display(Name = "Email пользователя")]
        [Remote(action: "CheckLogin", controller: "ValidationUsers", AdditionalFields ="Id", ErrorMessage = "Email уже используется")]
        [EmailAddress(ErrorMessage = "Здесь нужно ввести корректный Email")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Email { get; set; }
        
        
        [Display(Name = "Пароль пользователя")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(30, MinimumLength = 8, ErrorMessage = "Минимум 8 символов")]
        public string Password { get; set; }

        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Пароль и подтверждение пароля должны совпадать!")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string ConfirmPassword { get; set; }

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


/*        [Display(Name = "Роль пользователя")]
        public string Role { get; set; }*/

        [Display(Name = "Телефон")]
        [Remote(action: "CheckPhoneNumber", controller: "ValidationUsers", ErrorMessage = "Данный номер телефона уже есть в базе")]
        [RegularExpression(@"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?[0-9]{3}-[0-9]{2}-[0-9]{2})", ErrorMessage = "Некорректный телефон")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [DataType(DataType.PhoneNumber)]
        public string Telefon { get; set; }
    }
}
