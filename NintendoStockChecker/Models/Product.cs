namespace NintendoStockChecker.Models
{
    public class Product
    {
        public bool IsSalableQty { get; set; }
        public string Name { get; set; } = string.Empty;
        public Prices Prices { get; set; } = new Prices();
        public string Sku { get; set; } = string.Empty;
        public string UrlKey { get; set; } = string.Empty;
    }
}
