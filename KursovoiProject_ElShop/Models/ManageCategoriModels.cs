using System.ComponentModel.DataAnnotations;

namespace KursovoiProject_ElShop.Models
{
    public class CategoriesSite
    {
        public int CategoryId { get; set; }

        [Display(Name = "Наименование категории")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Минимум 2 символа")]
        public string CategoryName { get; set; }
        public bool IsMain { get; set; }
    }
    public class IndexPageCategory
    {
        public List<CategoriesSite> CategoriesSiteList { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public FilterViewModel_AddsSites FilterViewModel { get; set; }
    }
}
