namespace KursovoiProject_ElShop.Models
{
    public class InfoCadri
    {
        public int ID_Worker { get; set; } 
        public string Familia { get; set; }
        public string Ima { get; set; }
        public string phoneNumber { get; set; }
        public string PrikazOPrieme { get; set; }
        public string DateOfBirth { get; set; }
        public string? PrikazObUval { get; set; }
        public List<Filial> listFilial { get; set; }
        public List<Role> listRoles { get; set; }
        public bool avail { get; set; }
    }
    public class IndexPageCadri
    {
        public int statusSite { get; set; }
        public List<Filial> filials { get; set; }
        public List<InfoCadri> listCadri { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public FilterViewModel_AddsSites FilterViewModel { get; set; }
    }
}
