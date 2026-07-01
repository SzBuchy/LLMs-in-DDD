using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Interfaces;

public interface IBasketViewModelService
{
    Task<BasketViewModel> GetOrCreateBasketForUser(string userName);
}
