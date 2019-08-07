using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using UserActor.Interfaces;

namespace UserActor
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class UserActor : Actor, IUserActor
    {
        /// <summary>
        /// Initializes a new instance of UserActor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public UserActor(ActorService actorService, ActorId actorId) 
            : base(actorService, actorId)
        {
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");
            return Task.CompletedTask;

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see https://aka.ms/servicefabricactorsstateserialization

            //await this.AddToBasket(Guid.NewGuid(), 10);
            //Dictionary<Guid, int> basket = await this.GetBasket();
            //await this.ClearBasketAsync();

            //return this.StateManager.TryAddStateAsync("count", 0);
        }

        /// <summary>
        /// TODO: Replace with your own actor method.
        /// </summary>
        /// <returns></returns>
        public async Task AddToBasket(Guid productId, int quantity)
        {
            // Requests are not guaranteed to be processed in order nor at most once.
            // The update function here verifies that the incoming count is greater than the current count to preserve order.
            await this.StateManager.AddOrUpdateStateAsync(productId.ToString(), quantity, (id, oldQuantity) => oldQuantity + quantity);
        }

        /// <summary>
        /// TODO: Replace with your own actor method.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<BasketItem[]> GetBasket()
        {
            List<BasketItem> result = new List<BasketItem>();

            IEnumerable<string> productIds = await StateManager.GetStateNamesAsync();

            foreach (string productId in productIds)
            {
                int quantity = await StateManager.GetStateAsync<int>(productId);
                BasketItem item = new BasketItem() { ProductId = new Guid(productId), Quantity = quantity };
                result.Add(item);
            }

            return result.ToArray();
        }

        public async Task ClearBasketAsync()
        {
            IEnumerable<string> productIds = await StateManager.GetStateNamesAsync();

            foreach (string productId in productIds)
            {
                await this.StateManager.RemoveStateAsync(productId);
            }
        }
    }
}
