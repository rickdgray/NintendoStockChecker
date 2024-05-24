namespace NintendoStockChecker.Models
{
    public class Minimum
    {
        public string Currency { get; set; } = string.Empty;
        public bool Discounted { get; set; }
        public float FinalPrice { get; set; }
        public float RegularPrice { get; set; }
    }
}
