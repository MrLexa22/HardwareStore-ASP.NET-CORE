using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace KursovoiProject_ElShop.Models
{
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
