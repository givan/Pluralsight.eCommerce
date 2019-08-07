using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;

[assembly: FabricTransportActorRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]
namespace UserActor.Interfaces
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IUserActor : IActor
    {
        /// <summary>
        /// Adds or updates the product count to the user basket for a given product (dentified by its guid)
        /// </summary>
        Task AddToBasket(Guid productId, int quantity);

        /// <summary>
        /// Retrieves the current user basket
        /// </summary>
        /// <returns>Dictionary with product guid with the associated product count from the basket</returns>
        Task<BasketItem[]> GetBasket();

        /// <summary>
        /// Clears user basket
        /// </summary>
        /// <returns></returns>
        Task ClearBasketAsync();
    }
}
