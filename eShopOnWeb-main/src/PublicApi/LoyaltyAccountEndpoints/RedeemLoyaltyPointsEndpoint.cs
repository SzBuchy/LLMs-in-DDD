using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.PublicApi.LoyaltyAccountEndpoints;

public class RedeemLoyaltyPointsEndpoint(ILoyaltyAccountService loyaltyAccountService)
    : Endpoint<RedeemLoyaltyPointsRequest, RedeemLoyaltyPointsResponse>
{
    public override void Configure()
    {
        Post("api/loyalty-account/redemptions");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Description(d =>
            d.Produces<RedeemLoyaltyPointsResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .WithTags("LoyaltyAccountEndpoints"));
    }

    public override async Task HandleAsync(RedeemLoyaltyPointsRequest request, CancellationToken ct)
    {
        var buyerId = User.FindFirstValue(ClaimTypes.Name) ?? User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(buyerId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var redemption = await loyaltyAccountService.RedeemPointsAsync(
            buyerId,
            request.PointsToRedeem,
            now,
            ct);
        var account = await loyaltyAccountService.GetOrCreateAccountAsync(buyerId, ct);

        await SendAsync(new RedeemLoyaltyPointsResponse(request.CorrelationId())
        {
            PointsRedeemed = redemption.PointsRedeemed,
            DiscountAmount = redemption.DiscountAmount,
            LoyaltyAccount = LoyaltyAccountMapper.ToDto(account, now)
        }, cancellation: ct);
    }
}
