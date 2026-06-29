using System;
using System.Linq;
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
        {
            d.Produces<GetLoyaltyAccountResponse>(StatusCodes.Status200OK);
            d.Produces(StatusCodes.Status401Unauthorized);
            d.WithTags("LoyaltyAccountEndpoints");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var customerId = User.Identity?.Name;
        if (string.IsNullOrEmpty(customerId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var account = await loyaltyAccountService.GetOrCreateAccountAsync(customerId, ct);
        var now = DateTimeOffset.Now;

        var response = new GetLoyaltyAccountResponse
        {
            LoyaltyAccount = new LoyaltyAccountDto
            {
                Id = account.Id,
                CustomerId = account.CustomerId,
                Balance = account.GetActivePointsBalance(now),
                DiscountValue = account.GetDiscountValue(account.GetActivePointsBalance(now)),
                Entries = account.Entries.Select(e => new LoyaltyPointsEntryDto
                {
                    Id = e.Id,
                    Quantity = e.Quantity,
                    SpentQuantity = e.SpentQuantity,
                    AvailableQuantity = e.AvailableQuantity,
                    CreatedDate = e.CreatedDate,
                    OrderId = e.OrderId,
                    IsExpired = e.IsExpired(now)
                }).ToList(),
                Transactions = account.Transactions.Select(t => new LoyaltyTransactionDto
                {
                    Id = t.Id,
                    Amount = t.Amount,
                    Type = t.Type.ToString(),
                    CreatedDate = t.CreatedDate,
                    OrderId = t.OrderId
                }).ToList()
            }
        };

        await SendAsync(response, statusCode: 200, cancellation: ct);
    }
}
