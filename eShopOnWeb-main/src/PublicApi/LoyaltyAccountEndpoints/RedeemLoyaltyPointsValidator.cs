using FastEndpoints;
using FluentValidation;
using Microsoft.eShopWeb.ApplicationCore.Entities.LoyaltyAccountAggregate;

namespace Microsoft.eShopWeb.PublicApi.LoyaltyAccountEndpoints;

public class RedeemLoyaltyPointsValidator : Validator<RedeemLoyaltyPointsRequest>
{
    public RedeemLoyaltyPointsValidator()
    {
        RuleFor(request => request.PointsToRedeem)
            .GreaterThan(0)
            .LessThanOrEqualTo(LoyaltyAccount.MaxPointsPerRedemption);
    }
}
