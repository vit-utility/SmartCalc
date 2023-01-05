using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartCalcApp.CoinGeckoAPI
{
    public static class CoinGeckoAPIClient
    {
        private static readonly Uri CoinsList = new Uri("https://api.coingecko.com/api/v3/coins/list");
        private static readonly string SimplePrice = @"https://api.coingecko.com/api/v3/simple/price?ids={0}&vs_currencies=usd";
        private static readonly string CoinsMarketData = @"https://api.coingecko.com/api/v3/coins/markets?vs_currency=usd&ids={0}&order=market_cap_desc&per_page=200&page=1&sparkline=false";

        private static readonly HttpClient httpClient;

        static CoinGeckoAPIClient()
        {
            httpClient = new HttpClient();
        }

        public static IReadOnlyList<CoinInfo> GetCoinList()
        {
            return GetAsync<IReadOnlyList<CoinInfo>>(CoinsList).Result;
        }

        public static Price GetPrice(IReadOnlyList<string> coinIds)
        {
            var ids = WebUtility.UrlEncode(string.Join(",", coinIds));
            var uri = new Uri(string.Format(SimplePrice, ids));

            return GetAsync<Price>(uri).Result;
        }

        public static List<CoinMarkets> GetCoinsMarketData(IReadOnlyList<string> coinIds)
        {
            var ids = WebUtility.UrlEncode(string.Join(",", coinIds));
            var uri = new Uri(string.Format(CoinsMarketData, ids));

            return GetAsync<List<CoinMarkets>>(uri).Result;
        }

        private static async Task<T> GetAsync<T>(Uri resourceUri)
        {
            var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, resourceUri))
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            try
            {
                return JsonConvert.DeserializeObject<T>(responseContent);
            }
            catch (Exception e)
            {
                throw new HttpRequestException(e.Message);
            }
        }
    }
}
