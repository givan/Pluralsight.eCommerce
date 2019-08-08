using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eCommerce.API.Model
{
    public class ApiCheckoutProduct
    {
        [JsonProperty("productId")]
        public Guid ProductId { get; set; }

        [JsonProperty("productName")]
        public string ProductDescription { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }
    }
}
