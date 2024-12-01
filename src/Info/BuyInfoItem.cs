namespace SmartCalcApp.Info
{
    public class BuyInfoItem
    {
        public const string SymbolCol = "A";

        public const string LastBuyPriceCol = "B";
        public const string LastSellPriceCol = "C";
        public const string AveragePriceCol = "D";

        public const string DiffLastBuyPriceCol = "F";
        public const string DiffLastSellPriceCol = "G";
        public const string DiffAveragePriceCol = "H";

        public const string CurentPriceCol = "J";
        public const string MarketcapCol = "L";
        public const string FDVCol = "M";

        public const string SupplyCol = "N";
        public const string TotalSupplyCol = "O";
        public const string SupplyPercentCol = "P";

        public const string ATHCol = "R";
        public const string ATHPercentCol = "S";
        public const string ATLCol = "T";
        public const string ATLPercentCol = "U";

        public BuyInfoItem()
        {
        }

        public int Index { get; set; }
        public string Symbol { get; set; }
        public string CoinId { get; set; }

        public decimal LastBuyPrice { get; set; }
        public decimal LastSellPrice { get; set; }
        public decimal AveragePrice { get; set; }
    }
}
