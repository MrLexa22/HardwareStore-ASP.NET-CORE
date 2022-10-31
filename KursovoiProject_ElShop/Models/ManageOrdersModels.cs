namespace KursovoiProject_ElShop.Models
{
    public class OderInformationForIndexPage
    {
        public int ID_Order { get; set; }
        public string FI { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOrder { get; set; }
        public DateTime? DateReadyToExtradition { get; set; }
        public DateTime? DateExtradition { get; set; }
        public string FilialName { get; set; }
        public int Filial_ID { get; set; }
        public string SummaOrder { get; set; }
        public int OrdersPositions { get; set; }
        public int StatusOrder { get; set; }
    }
    public class IdexPageManageOrders
    {
        public int HowMuch_Status0 { get; set; }
        public int HowMuch_Status1 { get; set; }
        public int HowMuch_Status2 { get; set; }
        public int HowMuch_Status34 { get; set; }
        public List<Filial> filialsWork { get; set; }
        public List<Filial> filialsAll { get; set; }
        public List<OderInformationForIndexPage> listOrders { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public FilterViewModel_ManageOrders FilterViewModel { get; set; }
    }

    public class OrderAboutManageOrders
    {
        public int ID_Order { get; set; }
        public int StatusOrder { get; set; }
        public string FI { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOrder { get; set; }
        public DateTime? DateReadyToExtradition { get; set; }
        public string? FI_ReadyToExtradition { get; set; }
        public DateTime? DateExtradition { get; set; }
        public string? FI_Extradition { get; set; }
        public string FilialName { get; set; }
        public string FilialAddress { get; set; }
        public int Filial_ID { get; set; }
        public string SummaOrder { get; set; }
        public List<availibleTovari> listTovari { get; set; }
    }

    public class FilterViewModel_ManageOrders
    {
        public FilterViewModel_ManageOrders(string? search, int? typesort, int? filial, int? statusOrder)
        {
            Search = search;
            TypeSort = typesort;
            Filial = filial;
            StatusOrder = statusOrder;
        }
        public string? Search { get; private set; }
        public int? TypeSort { get; private set; }
        public int? Filial { get; private set; }
        public int? StatusOrder { get; private set; }
    }
}
