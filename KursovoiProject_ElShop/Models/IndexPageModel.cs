namespace KursovoiProject_ElShop.Models
{
    public class IndexPageModel
    {
        public List<AddsSite> MainAdds { get; set; }
        public List<AddsSite> Right1Adds { get; set; }
        public List<AddsSite> Right2Adds { get; set; }
        public List<Category> Categories { get; set; }
        public List<ViewGoodsWithManufacture> goodsRandom { get; set; }
    }
}
