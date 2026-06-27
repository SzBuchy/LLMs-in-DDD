using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.PublicApi.LoyaltyAccountEndpoints;

/// <summary>
/// Gets the loyalty account and available points balance for the currently authenticated customer
/// </summary>
public class GetLoyaltyAccountEndpoint(ILoyaltyAccountService loyaltyAccountService)
    : EndpointWithoutRequest<GetLoyaltyAccountResponse>
{
    public override void Configure()
    {
        Get("api/loyalty-account");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Description(d =>
            d.Produces<GetLoyaltyAccountResponse>()
             .WithTags("LoyaltyAccountEndpoints"));
    }

    public override async Task<GetLoyaltyAccountResponse> ExecuteAsync(CancellationToken ct)
    {
        var buyerId = User.FindFirstValue(ClaimTypes.Name);

        var account = await loyaltyAccountService.GetOrCreateAccountAsync(buyerId);

        return new GetLoyaltyAccountResponse
        {
            BuyerId = account.BuyerId,
            AvailablePoints = account.GetAvailablePoints(DateTimeOffset.UtcNow)
        };
    }
}
