using Microsoft.AspNetCore.Mvc.Rendering;

namespace KursovoiProject_ElShop.Models
{
    public class FilterViewModel_AddsSites
    {
        public FilterViewModel_AddsSites(string? search, int? typesort, int? type, int? status)
        {
            Search = search;
            TypeSort = typesort;
            Type = type;
            Status = status;
        }
        public string? Search { get; private set; }
        public int? TypeSort { get; private set; }
        public int? Type { get; private set; }
        public int? Status { get; private set; }
    }
}
