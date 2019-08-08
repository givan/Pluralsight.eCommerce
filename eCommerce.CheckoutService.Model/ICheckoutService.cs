using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;

namespace eCommerce.CheckoutService.Model
{
    public interface ICheckoutService : IService
    {
        Task<CheckoutSummary> CheckoutAsync(string userId);

        Task<CheckoutSummary[]> GetOrderHistoryAsync(string userId);
    }
}
