using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.PublicApi.LoyaltyAccountEndpoints;

public class AwardLoyaltyPointsEndpoint(ILoyaltyAccountService loyaltyAccountService)
    : Endpoint<AwardLoyaltyPointsRequest, AwardLoyaltyPointsResponse>
{
    public override void Configure()
    {
        Post("api/loyalty-account/points");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Description(d =>
            d.Produces<AwardLoyaltyPointsResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .WithTags("LoyaltyAccountEndpoints"));
    }

    public override async Task HandleAsync(AwardLoyaltyPointsRequest request, CancellationToken ct)
    {
        var buyerId = User.FindFirstValue(ClaimTypes.Name) ?? User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(buyerId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var awardedPoints = await loyaltyAccountService.AwardPointsForOrderAsync(
            buyerId,
            request.OrderId,
            request.OrderTotal,
            now,
            ct);
        var account = await loyaltyAccountService.GetOrCreateAccountAsync(buyerId, ct);

        await SendAsync(new AwardLoyaltyPointsResponse(request.CorrelationId())
        {
            AwardedPoints = awardedPoints,
            LoyaltyAccount = LoyaltyAccountMapper.ToDto(account, now)
        }, cancellation: ct);
    }
}
