namespace KursovoiProject_ElShop.Models
{
    public class FilialsNalichTovar
    {
        public int ID_Good { get; set; }
        public int ID_Filial { get; set; }
        public int CountSklad { get; set; }
        public string NameFilial { get; set; }
        public string AddressFilial { get; set; }
        public string Nalich { get; set; }
    }
}
