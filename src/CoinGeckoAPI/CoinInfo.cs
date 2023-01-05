using Newtonsoft.Json;

namespace SmartCalcApp.CoinGeckoAPI
{
    public class CoinInfo
    {
        public const string NoIdMarker = "no id";

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
