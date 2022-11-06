namespace KursovoiProject_ElShop.Models
{
    public class StatModelFilial
    {
        public string NameFilial { get; set; }
        public string StatistickByYear { get; set; }
    }
    public class StatisticsModels
    {
        public List<StatModelFilial> list { get; set; }
    }
}
