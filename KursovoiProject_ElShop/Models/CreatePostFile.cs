using System.ComponentModel.DataAnnotations;

namespace KursovoiProject_ElShop.Models
{
    public class CreatePostFile
    {
        [Required(ErrorMessage = "Выберите файл.")]
        [DataType(DataType.Upload)]
        public IFormFile MyFile { set; get; }
    }
}
