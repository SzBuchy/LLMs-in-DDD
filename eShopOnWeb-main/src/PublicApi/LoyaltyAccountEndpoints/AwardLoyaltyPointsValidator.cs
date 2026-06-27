using FastEndpoints;
using FluentValidation;

namespace Microsoft.eShopWeb.PublicApi.LoyaltyAccountEndpoints;

public class AwardLoyaltyPointsValidator : Validator<AwardLoyaltyPointsRequest>
{
    public AwardLoyaltyPointsValidator()
    {
        RuleFor(request => request.OrderId)
            .GreaterThan(0);

        RuleFor(request => request.OrderTotal)
            .GreaterThan(0);
    }
}
