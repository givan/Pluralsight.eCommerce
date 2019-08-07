using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerce.API.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using UserActor.Interfaces;

namespace eCommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        [HttpGet("{userId}")]
        public async Task<ApiBasket> GetAsync(string userId)
        {
            IUserActor actor = GetActor(userId);

            BasketItem[] basketItems = await actor.GetBasket();

            ApiBasket result = new ApiBasket()
            {
                UserId = userId,
                Items = basketItems.Select(p => new ApiBasketItem()
                {
                    ProductId = p.ProductId.ToString(),
                    Quantity = p.Quantity
                }).ToArray()
            };

            return result;
        }

        [HttpPost("{userId}")]
        public async Task AddAsync(string userId, [FromBody] ApiBasketAddRequest request)
        {
            IUserActor actor = GetActor(userId);

            await actor.AddToBasket(request.ProductId, request.Quantity);
        }

        [HttpDelete("{userId}")]
        public async Task DeleteAsync(string userId)
        {
            IUserActor actor = GetActor(userId);
            await actor.ClearBasketAsync();
        }

        IUserActor GetActor(string userId)
        {
            return ActorProxy.Create<IUserActor>(
                new ActorId(userId),
                new Uri("fabric:/Pluralsight.eCommerce/UserActorService"));
        }
    }
}