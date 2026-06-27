using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.PublicApi.LoyaltyAccountEndpoints;

public class GetLoyaltyAccountEndpoint(ILoyaltyAccountService loyaltyAccountService)
    : EndpointWithoutRequest<GetLoyaltyAccountResponse>
{
    public override void Configure()
    {
        Get("api/loyalty-account");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Description(d =>
            d.Produces<GetLoyaltyAccountResponse>()
                .Produces(StatusCodes.Status401Unauthorized)
                .WithTags("LoyaltyAccountEndpoints"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var buyerId = User.FindFirstValue(ClaimTypes.Name) ?? User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(buyerId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var account = await loyaltyAccountService.GetOrCreateAccountAsync(buyerId, ct);

        await SendAsync(new GetLoyaltyAccountResponse
        {
            LoyaltyAccount = LoyaltyAccountMapper.ToDto(account, now)
        }, cancellation: ct);
    }
}
