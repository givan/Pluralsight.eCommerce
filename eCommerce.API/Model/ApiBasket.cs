using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eCommerce.API.Model
{
    public class ApiBasket
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("items")]
        public ApiBasketItem[] Items { get; set; }
    }
}
