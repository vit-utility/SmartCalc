using SmartCalcApp.CoinGeckoAPI;
using SmartCalcApp.Deals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCalcApp
{
    public static class Context
    {
        public static string DealsFilePath { get; set; }
        public static string InfoFilePath { get; set; }

        public static Dictionary<string, DealsTable> DealsTables { get; set; } = new();
        public static Dictionary<string, string> IdMappingData { get; set; } = new();

        public static string GetCoinIdBySymbol(string symbol)
        {
            var key = IdMappingData.Keys.FirstOrDefault(x => x.IsTheSame(symbol));

            if (key is null)
            {
                Console.WriteLine($"Can not find id for {symbol}");
                return string.Empty;
            }

            if (IdMappingData[key].IsTheSame(CoinInfo.NoIdMarker))
            {
                Console.WriteLine($"{symbol} has not id");
                return string.Empty;
            }

            return IdMappingData[key];
        }        
    }
}
