using Newtonsoft.Json;

namespace eCommerce.API.Model
{
    public class ApiBasketItem
    {
        [JsonProperty("productId")]
        public string ProductId { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }
    }
}