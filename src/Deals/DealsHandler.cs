using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCalcApp.Deals
{
    public static class DealsHandler
    {
        public const string PriceCol = "B";
        public const string AmountCol = "C";
        public const string ValueCol = "D";
        public const string DataCol = "H";
        public const int StartRow = 2;
        public const int TotalAmountRow = 4;
        public const int TotalValueRow = 6;
        public const int AvgPriceRow = 8;
        public const int LastBuyPriceRow = 10;
        public const int LastSellPriceRow = 12;
        public const int ResultRow = 14;

        //public static void ReadDealsData()
        //{
        //    using var workbook = new XLWorkbook(Context.DealsFilePath);
        //    var sheets = workbook.Worksheets.Where(x => !x.Name.IsTheSame("Template")).ToList();

        //    foreach (var item in sheets)
        //    {
        //        var dt = new DealsTable { Key = item.Name };
        //        Context.DealsTables[dt.Key] = dt;

        //        dt.TotalAmount = item.Cell(TotalAmountRow, DataCol).ReadAsDecimal();
        //        dt.TotalValue = item.Cell(TotalValueRow, DataCol).ReadAsDecimal();
        //        dt.AveragePrice = item.Cell(AvgPriceRow, DataCol).ReadAsDecimal();
        //        dt.LastBuyPrice = item.Cell(LastBuyPriceRow, DataCol).ReadAsDecimal();
        //        dt.LastSellPrice = item.Cell(LastSellPriceRow, DataCol).ReadAsDecimal();
        //    }
        //}

        public static void CalculateDeals()
        {
            using var workbook = new XLWorkbook(Context.DealsFilePath);
            var sheets = workbook.Worksheets.Where(x => !x.Name.IsTheSame("Template")).ToList();

            foreach (var item in sheets)
            {
                var dt = new DealsTable { Key = item.Name };
                Context.DealsTables[dt.Key] = dt;

                var index = item.Cell(StartRow, DataCol).ReadAsInt();

                // read values before index
                dt.Result = 0m;
                for (int i = 3; i < index; i++)
                {
                    var value = item.Cell(i, ValueCol).ReadAsDecimal();
                    if (value == 0m)
                    {
                        var price = item.Cell(i, PriceCol).ReadAsDecimal();

                        if (price == 0m)
                        {
                            break;
                        }

                        var amount = item.Cell(i, AmountCol).ReadAsDecimal();
                        value = amount * price;
                        item.Cell(i, ValueCol).Value = value;
                    }

                    dt.Result += value;
                }

                // read deals
                while (true)
                {
                    var amount = item.Cell(index, AmountCol).ReadAsDecimal();

                    if (amount == 0m)
                    {
                        break;
                    }

                    var valueString = item.Cell(index, ValueCol).Value.ToString();
                    decimal value, price;

                    if (string.IsNullOrEmpty(valueString))
                    {
                        // use price to calc value
                        price = item.Cell(index, PriceCol).ReadAsDecimal();
                        value = amount * price;
                        item.Cell(index, ValueCol).Value = value;
                    }
                    else
                    {
                        // calc price
                        value = item.Cell(index, ValueCol).ReadAsDecimal();
                        price = value / amount;
                        item.Cell(index, PriceCol).Value = price;
                    }

                    var deal = new DealItem { Index = index, Amount = amount, Price = price, Value = value };

                    dt.Deals.Add(deal);

                    if (value > 0)
                    {
                        dt.LastBuyPrice = price;
                    }
                    else if (value < 0)
                    {
                        dt.LastSellPrice = price;
                    }
                    else
                    { 
                        // value is 0
                        // do nothing
                    }

                    ++index;
                }

                dt.TotalAmount = 0;
                dt.TotalValue = 0;
                foreach (var deal in dt.Deals)
                {
                    dt.TotalAmount += deal.Amount;
                    dt.TotalValue += deal.Value;

                    dt.Result += deal.Value;
                }

                dt.AveragePrice = dt.TotalAmount != 0m ? dt.TotalValue / dt.TotalAmount : 0m;
                dt.Result *= -1;

                item.Cell(TotalAmountRow, DataCol).Value = dt.TotalAmount;
                item.Cell(TotalValueRow, DataCol).Value = dt.TotalValue;
                item.Cell(AvgPriceRow, DataCol).Value = dt.AveragePrice;
                item.Cell(LastBuyPriceRow, DataCol).Value = dt.LastBuyPrice;
                item.Cell(LastSellPriceRow, DataCol).Value = dt.LastSellPrice;
                item.Cell(ResultRow, DataCol).Value = dt.Result;
                item.Cell(13, DataCol).Value = "Result";

                workbook.Save();
            }
        }

        public static void SortWorksheets()
        {
            using var workbook = new XLWorkbook(Context.DealsFilePath);
            var sheetsNames = workbook.Worksheets.Where(x => !x.Name.IsTheSame("Template"))
                .Select(y => y.Name).OrderBy(a => a).ToList();

            for (int i = 0; i < sheetsNames.Count; i++)
            {
                workbook.Worksheets.Worksheet(sheetsNames[i]).Position = i + 2;
            }

            workbook.Save();
        }
    }
}
