using Newtonsoft.Json;

namespace Data
{
    public class Item
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("icon")]
        public string IconUrl { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }
}

