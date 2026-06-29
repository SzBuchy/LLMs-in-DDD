using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.PublicApi.LoyaltyAccountEndpoints;

public class SpendPointsEndpoint(ILoyaltyAccountService loyaltyAccountService)
    : Endpoint<SpendPointsRequest, SpendPointsResponse>
{
    public override void Configure()
    {
        Post("api/loyalty-account/spend");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Description(d =>
        {
            d.Produces<SpendPointsResponse>(StatusCodes.Status200OK);
            d.Produces(StatusCodes.Status400BadRequest);
            d.Produces(StatusCodes.Status401Unauthorized);
            d.WithTags("LoyaltyAccountEndpoints");
        });
    }

    public override async Task HandleAsync(SpendPointsRequest request, CancellationToken ct)
    {
        var customerId = User.Identity?.Name;
        if (string.IsNullOrEmpty(customerId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        try
        {
            var account = await loyaltyAccountService.SpendPointsAsync(customerId, request.Points, request.OrderId, ct);
            var now = DateTimeOffset.Now;

            var response = new SpendPointsResponse(request.CorrelationId())
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
        catch (ArgumentException ex)
        {
            AddError(ex.Message);
            await SendErrorsAsync(statusCode: 400, cancellation: ct);
        }
        catch (InvalidOperationException ex)
        {
            AddError(ex.Message);
            await SendErrorsAsync(statusCode: 400, cancellation: ct);
        }
    }
}
