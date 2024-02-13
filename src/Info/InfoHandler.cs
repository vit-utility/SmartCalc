using ClosedXML.Excel;
using SmartCalcApp.CoinGeckoAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCalcApp.Info
{
    public static class InfoHandler
    {
        private const string IdMappingSheet = "IdMapping";
        private const string PortfolioSheet = "Portfolio";
        private const string BagSheet = "Bag";
        private const string BuyInfoSheet = "Buy info";

        public const string SymbolCol = "A";
        public const string LastBuyPriceCol = "B";
        public const string LastSellPriceCol = "C";
        public const string AveragePriceCol = "D";
        public const string CurrentPriceCol = "F";
        public const string DiffLastBuyCol = "H";
        public const string DifLastSellCol = "I";
        public const string DiffAverageCol = "J";
        public const string AmountCol = "L";
        public const string ValueCol = "M";
        public const string CurrentValueCol = "N";
        public const string ResultCol = "O";
        public const string MarketcapCol = "Q";
        public const string FDVCol = "R";

        public const string BagSymbolCol = "A";
        public const string BagCurrentPriceCol = "B";
        public const string BagAmountCol = "C";
        public const string BagCurrentValueCol = "D";
        public const string BagPercentCol = "E";
        public const string BagDatatCol = "G";
        public const int USDCRow = 4;
        public const int PTVRow = 6;
        public const int CurrentTotalRow = 8;

        public static void LoadIdMapping()
        {
            using var infoWB = new XLWorkbook(Context.InfoFilePath);
            var infoWS = infoWB.Worksheet(IdMappingSheet);

            var needCheck = infoWS.Cell(1, "C").Value.ToString();
            if (!string.IsNullOrEmpty(needCheck))
            {
                Console.WriteLine("You should manually check IdMappingData. Press Enter to exit");
                Console.ReadLine();
                throw new InvalidOperationException();
            }

            Context.IdMappingData.Clear();

            var index = 1;
            while (true)
            {
                var symbol = infoWS.Cell(index, "A").Value.ToString();

                if (string.IsNullOrEmpty(symbol))
                {
                    break;
                }

                var id = infoWS.Cell(index, "B").Value.ToString();

                Context.IdMappingData[symbol] = id;

                ++index;
            }
        }

        public static void UpdateIdMapping()
        {
            LoadIdMapping();

            using var workbook = new XLWorkbook(Context.DealsFilePath);
            var sheetsNames = workbook.Worksheets.Where(x => !x.Name.IsTheSame("Template"))
                .Select(y => y.Name).OrderBy(a => a).ToList();

            var symbolsWithoutId = new List<string>();

            foreach (var item in sheetsNames)
            {
                var notExistedKey = Context.IdMappingData.Keys.FirstOrDefault(x => x.IsTheSame(item));

                if (notExistedKey is null)
                {
                    symbolsWithoutId.Add(item);
                }
                else
                {
                    // symbol without id in last response
                    if (Context.IdMappingData[notExistedKey].IsTheSame(CoinInfo.NoIdMarker))
                    {
                        symbolsWithoutId.Add(item);
                    }
                }
            }

            if (symbolsWithoutId.Count == 0)
            {
                return;
            }

            var data = new Dictionary<string, List<string>>();

            foreach (var item in symbolsWithoutId)
            {
                data[item] = new List<string>();
            }

            var idData = CoinGeckoAPIClient.GetCoinList();

            foreach (var item in idData)
            {
                foreach (var symbol in symbolsWithoutId)
                {
                    if (item.Symbol.IsTheSame(symbol))
                    {
                        data[symbol].Add(item.Id);
                    }
                }
            }

            var needCheck = false;

            foreach (var item in data)
            {
                if (item.Value.Count == 0)
                {
                    Context.IdMappingData[item.Key] = CoinInfo.NoIdMarker;
                }
                else if (item.Value.Count == 1)
                {
                    Context.IdMappingData[item.Key] = item.Value[0];
                }
                else
                {
                    needCheck = true;
                    Context.IdMappingData[item.Key] = string.Join(", ", item.Value);
                }
            }

            using var infoWB = new XLWorkbook(Context.InfoFilePath);
            var infoWS = infoWB.Worksheet(IdMappingSheet);

            if (needCheck)
            {
                infoWS.Cell(1, "C").Value = "need check";
            }

            var index = 1;
            foreach (var key in Context.IdMappingData.Keys)
            {
                infoWS.Cell(index, "A").Value = key;
                infoWS.Cell(index, "B").Value = Context.IdMappingData[key];

                ++index;
            }

            infoWB.Save();

            if (needCheck)
            {
                Console.WriteLine("You should manually check IdMappingData. Press Enter to exit");
                Console.ReadLine();
                throw new InvalidOperationException();
            }
        }

        public static void RebuildPortfolioList()
        {
            using var wb = new XLWorkbook(Context.InfoFilePath);
            var ws = wb.Worksheet(PortfolioSheet);

            using var workbook = new XLWorkbook(Context.DealsFilePath);
            var sheetsNames = workbook.Worksheets.Where(x => !x.Name.IsTheSame("Template"))
                .Select(y => y.Name).OrderBy(a => a).ToList();

            int index = 3;
            foreach (var item in sheetsNames)
            {
                ws.Cell(index, SymbolCol).Value = item;
                ++index;
            }

            wb.Save();
        }

        public static void ApplyDealsData()
        {
            using var wb = new XLWorkbook(Context.InfoFilePath);
            var ws = wb.Worksheet(PortfolioSheet);

            var symbols = new List<PortfolioItem>();
            int index = 3;

            while (true)
            {
                var symbol = ws.Cell(index, SymbolCol).Value.ToString();

                if (string.IsNullOrEmpty(symbol))
                {
                    break;
                }

                symbols.Add(new PortfolioItem { Index = index, Symbol = symbol });
                ++index;
            }

            foreach (var item in symbols)
            {
                var dt = Context.DealsTables[item.Symbol];

                ws.Cell(item.Index, LastBuyPriceCol).Value = dt.LastBuyPrice;
                ws.Cell(item.Index, LastSellPriceCol).Value = dt.LastSellPrice;
                ws.Cell(item.Index, AveragePriceCol).Value = dt.AveragePrice;

                ws.Cell(item.Index, AmountCol).Value = dt.TotalAmount;
                ws.Cell(item.Index, ValueCol).Value = dt.TotalValue;

                ws.Cell(item.Index, ResultCol).Value = dt.Result;
            }

            wb.Save();
        }

        public static void UpdatePortfolio()
        {
            LoadIdMapping();

            using var wb = new XLWorkbook(Context.InfoFilePath);
            var ws = wb.Worksheet(PortfolioSheet);

            var symbols = new List<PortfolioItem>();
            int index = 3;
            decimal totalValue = 0m;

            // read porfolio data
            while (true)
            {
                var symbol = ws.Cell(index, SymbolCol).Value.ToString();

                if (string.IsNullOrEmpty(symbol))
                {
                    break;
                }

                var lbp = ws.Cell(index, LastBuyPriceCol).ReadAsDecimal();
                var lsp = ws.Cell(index, LastSellPriceCol).ReadAsDecimal();
                var avg = ws.Cell(index, AveragePriceCol).ReadAsDecimal();
                var amount = ws.Cell(index, AmountCol).ReadAsDecimal();

                // for bag
                totalValue += ws.Cell(index, ValueCol).ReadAsDecimal();

                symbols.Add(new PortfolioItem
                {
                    Index = index,
                    Symbol = symbol,
                    LastBuyPrice = lbp,
                    LastSellPrice = lsp,
                    AveragePrice = avg,
                    Amount = amount,
                    CoinId = Context.GetCoinIdBySymbol(symbol)
                }); ;

                ++index;
            }

            var coinsIds = symbols.Select(x => x.CoinId).Where(y => !string.IsNullOrEmpty(y)).ToList();

            // var coinsIdsForMarketData = symbols.Where(x => x.Amount > 0).Select(x => x.CoinId).ToList();

            // var priceData = CoinGeckoAPIClient.GetPrice(coinsIds);

            var marketData = CoinGeckoAPIClient.GetCoinsMarketData(coinsIds);

            // calculate
            foreach (var item in symbols)
            {
             //   item.CurrentPrice = priceData.GetCurrentPriceInUsdFor(item.CoinId);

                var md = marketData.FirstOrDefault(x => x.Symbol.IsTheSame(item.Symbol));

                if (md is null)
                {
                    item.Marketcap = "-";
                    item.FDV = "-";
                    item.CurrentPrice = 0m;
                }
                else
                {
                    item.Marketcap = md.MarketCap.HasValue ? md.MarketCap.Value.ToString("N0") : "-";
                    item.FDV = md.FDV.HasValue ? md.FDV.Value.ToString("N0") : "-";
                    item.CurrentPrice = md.CurrentPrice ?? 0m;
                }

                if (item.CurrentPrice == 0m)
                {
                    item.LastBuyDiff = "-";
                    item.LastSellDiff = "-";
                    item.AverageDiff = "-";

                    continue;
                }

                item.CurrentValue = item.CurrentPrice * item.Amount;

                decimal diff;
                if (item.LastBuyPrice == 0m)
                {
                    item.LastBuyDiff = "-";
                }
                else
                {
                    diff = Math.Round((item.CurrentPrice - item.LastBuyPrice) / item.LastBuyPrice, 3) * 100;
                    item.LastBuyDiff = $"{(diff > 0 ? "+" : "")}{diff:N1} %";
                }

                if (item.LastSellPrice == 0m)
                {
                    item.LastSellDiff = "-";
                }
                else
                {
                    diff = Math.Round((item.CurrentPrice - item.LastSellPrice) / item.LastSellPrice, 3) * 100;
                    item.LastSellDiff = $"{(diff > 0 ? "+" : "")}{diff:N1} %";
                }

                if (item.AveragePrice == 0m)
                {
                    item.AverageDiff = "-";
                }
                else
                {
                    diff = Math.Round((item.CurrentPrice - item.AveragePrice) / item.AveragePrice, 3) * 100;
                    item.AverageDiff = $"{(diff > 0 ? "+" : "")}{diff:N1} %";
                }

            }

            // write porfolio
            foreach (var item in symbols)
            {
                ws.Cell(item.Index, CurrentPriceCol).Value = item.CurrentPrice;

                ws.Cell(item.Index, DiffLastBuyCol).Value = item.LastBuyDiff;
                ws.Cell(item.Index, DifLastSellCol).Value = item.LastSellDiff;
                ws.Cell(item.Index, DiffAverageCol).Value = item.AverageDiff;

                ws.Cell(item.Index, CurrentValueCol).Value = item.CurrentValue;

                ws.Cell(item.Index, MarketcapCol).Value = item.Marketcap;
                ws.Cell(item.Index, FDVCol).Value = item.FDV;
            }

            // bag section

            var bagWS = wb.Worksheet(BagSheet);
            var usdc = bagWS.Cell(USDCRow, BagDatatCol).ReadAsDecimal();
            var ptv = bagWS.Cell(PTVRow, BagDatatCol).ReadAsDecimal();

            usdc -= totalValue - ptv;
            bagWS.Cell(USDCRow, BagDatatCol).Value = usdc;
            bagWS.Cell(PTVRow, BagDatatCol).Value = totalValue;

            symbols.Add(new PortfolioItem { Symbol = "USDC", Amount = usdc, CurrentPrice = 1, CurrentValue = usdc });

            var totalCurrentValue = symbols.Sum(x => x.CurrentValue);
            bagWS.Cell(CurrentTotalRow, BagDatatCol).Value = totalCurrentValue;

            foreach (var item in symbols)
            {
                // use like storage for percent 
                item.LastBuyPrice = item.CurrentValue / totalCurrentValue * 100m;
            }

            symbols = symbols.OrderByDescending(x => x.LastBuyPrice).ToList();

            // write bag data
            index = 3;

            foreach (var item in symbols)
            {
                bagWS.Cell(index, BagSymbolCol).Value = item.Symbol;
                bagWS.Cell(index, BagCurrentPriceCol).Value = item.CurrentPrice;
                bagWS.Cell(index, BagAmountCol).Value = item.Amount;
                bagWS.Cell(index, BagCurrentValueCol).Value = item.CurrentValue;
                bagWS.Cell(index, BagPercentCol).Value = item.LastBuyPrice;

                ++index;
            }

            wb.Save();
        }

        public static void UpdateBuyInfo()
        {
            LoadIdMapping();

            using var infoWB = new XLWorkbook(Context.InfoFilePath);
            var buyWS = infoWB.Worksheet(BuyInfoSheet);
            var buyItems = new List<BuyInfoItem>();

            #region Read Buy items 

            int index = 3;
            while (true)
            {
                var symbol = buyWS.Cell(index, BuyInfoItem.SymbolCol).Value.ToString();

                if (string.IsNullOrEmpty(symbol))
                {
                    break;
                }

                buyItems.Add(new BuyInfoItem { Index = index, Symbol = symbol });

                ++index;
            }

            #endregion

            #region Check exist symbol without id

            var symbolsWithoutId = new List<string>();

            foreach (var item in buyItems)
            {
                var notExistedKey = Context.IdMappingData.Keys.FirstOrDefault(x => x.IsTheSame(item.Symbol));

                if (notExistedKey is null)
                {
                    symbolsWithoutId.Add(item.Symbol);
                }
            }

            if (symbolsWithoutId.Count > 0)
            {
                var coinIdData = new Dictionary<string, List<string>>();

                foreach (var item in symbolsWithoutId)
                {
                    coinIdData[item] = new List<string>();
                }

                var idData = CoinGeckoAPIClient.GetCoinList();

                foreach (var item in idData)
                {
                    foreach (var symbol in symbolsWithoutId)
                    {
                        if (item.Symbol.IsTheSame(symbol))
                        {
                            coinIdData[symbol].Add(item.Id);
                        }
                    }
                }

                var needCheck = false;

                foreach (var item in coinIdData)
                {
                    if (item.Value.Count == 0)
                    {
                        Context.IdMappingData[item.Key] = CoinInfo.NoIdMarker;
                    }
                    else if (item.Value.Count == 1)
                    {
                        Context.IdMappingData[item.Key] = item.Value[0];
                    }
                    else
                    {
                        needCheck = true;
                        Context.IdMappingData[item.Key] = string.Join(", ", item.Value);
                    }
                }

                var idMapping = infoWB.Worksheet(IdMappingSheet);

                if (needCheck)
                {
                    idMapping.Cell(1, "C").Value = "need check";
                }

                index = 1;
                foreach (var key in Context.IdMappingData.Keys)
                {
                    idMapping.Cell(index, "A").Value = key;
                    idMapping.Cell(index, "B").Value = Context.IdMappingData[key];

                    ++index;
                }

                infoWB.Save();

                if (needCheck)
                {
                    Console.WriteLine("You should manually check IdMappingData. Press Enter to exit");
                    Console.ReadLine();
                    throw new InvalidOperationException();
                }
            }

            #endregion

            var coinIds = new List<string>();
            foreach (var item in buyItems)
            {
                var coinId = Context.GetCoinIdBySymbol(item.Symbol);
                if (!string.IsNullOrEmpty(coinId))
                {
                    coinIds.Add(coinId);
                }
            }

            var marketData = CoinGeckoAPIClient.GetCoinsMarketData(coinIds);

            foreach (var item in buyItems)
            {
                var md = marketData.FirstOrDefault(x => x.Symbol.IsTheSame(item.Symbol));

                if (md is null)
                {
                    continue;
                }

                buyWS.Cell(item.Index, BuyInfoItem.CurentPriceCol).Value = md.CurrentPrice ?? 1m;
                buyWS.Cell(item.Index, BuyInfoItem.MarketcapCol).Value = md.MarketCap ?? 0m;
                buyWS.Cell(item.Index, BuyInfoItem.SupplyCol).Value = md.CirculatingSupply ?? 0m;
                buyWS.Cell(item.Index, BuyInfoItem.MaxSuplyCol).Value = md.MaxSupply ?? 0m;
                buyWS.Cell(item.Index, BuyInfoItem.ATHCol).Value = md.Ath ?? 0m;
                buyWS.Cell(item.Index, BuyInfoItem.ATHPercentCol).Value = md.AthChangePercentage ?? 0m;
                buyWS.Cell(item.Index, BuyInfoItem.ATLCol).Value = md.Atl ?? 0m;
                buyWS.Cell(item.Index, BuyInfoItem.ATLPercentCol).Value = md.AtlChangePercentage ?? 0m;
            }

            infoWB.Save();
        }
    }
}
