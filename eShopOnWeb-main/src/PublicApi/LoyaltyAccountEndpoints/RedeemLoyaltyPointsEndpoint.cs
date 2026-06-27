using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.PublicApi.LoyaltyAccountEndpoints;

/// <summary>
/// Redeems loyalty points for a discount on the customer's next order
/// </summary>
public class RedeemLoyaltyPointsEndpoint(ILoyaltyAccountService loyaltyAccountService)
    : Endpoint<RedeemLoyaltyPointsRequest, RedeemLoyaltyPointsResponse>
{
    public override void Configure()
    {
        Post("api/loyalty-account/redeem");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Description(d =>
            d.Produces<RedeemLoyaltyPointsResponse>()
             .WithTags("LoyaltyAccountEndpoints"));
    }

    public override async Task HandleAsync(RedeemLoyaltyPointsRequest request, CancellationToken ct)
    {
        var buyerId = User.FindFirstValue(ClaimTypes.Name);

        var discountAmount = await loyaltyAccountService.RedeemPointsAsync(buyerId, request.Points);

        await SendAsync(new RedeemLoyaltyPointsResponse(request.CorrelationId())
        {
            RedeemedPoints = request.Points,
            DiscountAmount = discountAmount
        }, cancellation: ct);
    }
}
