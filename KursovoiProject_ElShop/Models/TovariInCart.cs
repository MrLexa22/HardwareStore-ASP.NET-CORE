namespace KursovoiProject_ElShop.Models
{
    public class FilialsWithBoll
    {
        public int IdFilial { get; set; }
        public string NameFilial { get; set; } = null!;
        public string AddressFilial { get; set; } = null!;
        public bool Availeble { get; set; }
        public bool Selected { get; set; }
    }
    public class TovariInCart
    {
        public List<Korzina> list_tovari { get; set; }
        public List<FilialsWithBoll> list_filial { get; set; }
        public List<AddsSite> Right1Adds { get; set; }
        public List<AddsSite> Right2Adds { get; set; }
    }
}
