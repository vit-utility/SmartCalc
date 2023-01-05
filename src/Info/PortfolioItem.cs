using System;

namespace SmartCalcApp.Info
{
    public class PortfolioItem
    {
        public PortfolioItem()
        {
        }

        public int Index { get; set; }
        public string Symbol { get; set; }
        public string CoinId { get; set; }
        public decimal LastBuyPrice { get; set; }
        public decimal LastSellPrice { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal CurrentPrice { get; set; } = 1;
        public string LastBuyDiff { get; set; }
        public string LastSellDiff { get; set; }
        public string AverageDiff { get; set; }
        public decimal Amount { get; set; }
        public decimal Value { get; set; }
        public decimal CurrentValue { get; set; }
    }
}
