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
        private static readonly string CoinsList = @"https://api.coingecko.com/api/v3/coins/list?x_cg_demo_api_key={0}";
        private static readonly string SimplePrice = @"https://api.coingecko.com/api/v3/simple/price?ids={0}&vs_currencies=usd&x_cg_demo_api_key={1}";
        private static readonly string CoinsMarketData = @"https://api.coingecko.com/api/v3/coins/markets?vs_currency=usd&ids={0}&x_cg_demo_api_key={1}";

        private static readonly HttpClient httpClient;
        private static string ApiKey;
        
        static CoinGeckoAPIClient()
        {
            httpClient = new HttpClient();   // does not work

            CookieContainer cookies = new CookieContainer(); // work
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = cookies;
            handler.UseCookies = true;

            httpClient = new HttpClient(handler);

            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.97 Safari/537.36");
        }

        public static void SetApiKey(string apiKey)
        {
            ApiKey = apiKey;
        }

        public static IReadOnlyList<CoinInfo> GetCoinList()
        {
            var request = string.Format(CoinsList, ApiKey);

            return GetAsync<IReadOnlyList<CoinInfo>>(request).Result;
        }

        public static Price GetPrice(IReadOnlyList<string> coinIds)
        {
            var ids = WebUtility.UrlEncode(string.Join(",", coinIds));
            var request = string.Format(SimplePrice, ids, ApiKey);

            return GetAsync<Price>(request).Result;
        }

        public static List<CoinMarkets> GetCoinsMarketData(IReadOnlyList<string> coinIds)
        {
            var ids = WebUtility.UrlEncode(string.Join(",", coinIds));
            var request = string.Format(CoinsMarketData, ids, ApiKey);

            return GetAsync<List<CoinMarkets>>(request).Result;
        }

        private static async Task<T> GetAsync<T>(string request)
        {
            var response = await httpClient.GetAsync(request).ConfigureAwait(false);

            var responseContent = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();

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
