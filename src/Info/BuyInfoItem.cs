namespace SmartCalcApp.Info
{
    public class BuyInfoItem
    {
        public const string SymbolCol = "A";
        public const string CurentPriceCol = "E";
        public const string MarketcapCol = "F";
        public const string SupplyCol = "G";
        public const string MaxSuplyCol = "H";
        public const string ATHCol = "I";
        public const string ATHPercentCol = "J";
        public const string ATLCol = "K";
        public const string ATLPercentCol = "L";
                
        public BuyInfoItem()
        {
        }

        public int Index { get; set; }
        public string Symbol { get; set; }
        public string CoinId { get; set; }        
    }
}
