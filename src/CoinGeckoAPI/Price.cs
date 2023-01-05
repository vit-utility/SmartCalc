using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartCalcApp.CoinGeckoAPI
{
    public class Price : Dictionary<string, Dictionary<string, decimal?>>
    {
        public decimal GetCurrentPriceInUsdFor(string coinId)
        {
            var coinKey = this.Keys.FirstOrDefault(x => x.IsTheSame(coinId));
            if (coinKey is null)
            {
                Console.WriteLine($"Can not find price for {coinId}");
                return 0m;
            }

            var coin = this[coinKey];
            var usdKey = coin.Keys.FirstOrDefault(x => x.IsTheSame("USD"));

            if (usdKey is null)
            {
                Console.WriteLine($"Can not find price for {coinId} in USD");
                return 0m;
            }

            var value = coin[usdKey];

            if (!value.HasValue)
            {
                Console.WriteLine($"Server did not send price for {coinId} in USD");
                return 0m;
            }

            return value.Value;
        }
    }
}
