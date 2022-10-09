using System.ComponentModel.DataAnnotations;

namespace KursovoiProject_ElShop.Models
{
    public class ListAdds
    {
        public List<AddsSite> AddsSite { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public FilterViewModel_AddsSites FilterViewModel { get; set; }
    }

    public class AddEditAdds
    {
        public List<Category> categories { get; set; }
        public int ID { get; set; }

        [Display(Name = "Наименование")]
        [StringLength(50, MinimumLength = 3)]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Name { get; set; }

        [Display(Name = "Расположение на сайте")]
        public int TypeWhere { get; set; }

        [Display(Name = "Картинка рекламы")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string FTPImage { get; set; }

        [Display(Name = "Ссылка на рекламу")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string href { get; set; }

        [Display(Name = "Срок действия")]
        [DataType(DataType.Date)]
        public DateTime dateEnd { get; set; }
        public bool IsEnd { get; set; }
    }
}
