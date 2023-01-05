using Newtonsoft.Json;

namespace SmartCalcApp.CoinGeckoAPI
{
    public class CoinMarkets 
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("circulating_supply")]
        public decimal? CirculatingSupply { get; set; }

        [JsonProperty("total_supply")]
        public decimal? TotalSupply { get; set; }

        [JsonProperty("max_supply")]
        public decimal? MaxSupply { get; set; }

        [JsonProperty("current_price")]
        public decimal? CurrentPrice { get; set; }

        [JsonProperty("market_cap")]
        public decimal? MarketCap { get; set; }

        [JsonProperty("ath")]
        public decimal? Ath { get; set; }

        [JsonProperty("ath_change_percentage")]
        public decimal? AthChangePercentage { get; set; }

        [JsonProperty("atl")]
        public decimal? Atl { get; set; }

        [JsonProperty("atl_change_percentage")]
        public decimal? AtlChangePercentage { get; set; }

        public override string ToString()
        {
            return $"{Name}[{Symbol}]";
        }
    }
}
