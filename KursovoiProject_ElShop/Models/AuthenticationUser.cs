using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace KursovoiProject_ElShop.Models
{
    public class ActivateAccount1
    {
        [Display(Name = "Email пользователя")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Email { get; set; }

        [ValidateNever]
        [Display(Name = "Код с почты")]
        public string Code { get; set; }
    }
    public class AuthenticationUser
    {
        public string Id { get; set; }


        [Display(Name = "Email пользователя")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Email { get; set; }
        
        
        [Display(Name = "Пароль пользователя")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Password { get; set; }
    }
}
