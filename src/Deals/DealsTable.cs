using System.Collections.Generic;

namespace SmartCalcApp.Deals
{
    public class DealsTable
    {
        public string Key { get; set; }
        public List<DealItem> Deals { get; } = new List<DealItem>();

        public decimal TotalAmount { get; set; }
        public decimal TotalValue { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal LastBuyPrice { get; set; }
        public decimal LastSellPrice { get; set; }
        public decimal Result { get; set; }
    }
}
