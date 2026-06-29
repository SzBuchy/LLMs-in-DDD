using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using VOEConsulting.Flame.BasketContext.Application.Abstractions;
using VOEConsulting.Flame.BasketContext.Application.Loyalty.Dtos;
using VOEConsulting.Flame.BasketContext.Domain.Baskets;
using VOEConsulting.Flame.BasketContext.Domain.Loyalty;
using VOEConsulting.Flame.BasketContext.Domain.Loyalty.Services;
using VOEConsulting.Flame.Common.Domain;
using VOEConsulting.Flame.Common.Domain.Errors;

namespace VOEConsulting.Flame.BasketContext.Application.Loyalty.Queries.GetLoyaltyAccount
{
    public class GetLoyaltyAccountQueryHandler : IQueryHandler<GetLoyaltyAccountQuery, LoyaltyAccountDto>
    {
        private readonly ILoyaltyAccountRepository _loyaltyAccountRepository;
        private readonly IMapper _mapper;

        public GetLoyaltyAccountQueryHandler(ILoyaltyAccountRepository loyaltyAccountRepository, IMapper mapper)
        {
            _loyaltyAccountRepository = loyaltyAccountRepository;
            _mapper = mapper;
        }

        public async Task<Result<LoyaltyAccountDto, IDomainError>> Handle(GetLoyaltyAccountQuery request, CancellationToken cancellationToken)
        {
            var account = await _loyaltyAccountRepository.GetByCustomerIdAsync(Id<Customer>.FromGuid(request.CustomerId), cancellationToken);
            if (account == null)
            {
                return Result.Failure<LoyaltyAccountDto, IDomainError>(DomainError.NotFound("Loyalty account not found for the given customer."));
            }

            var dto = _mapper.Map<LoyaltyAccountDto>(account);
            return Result.Success<LoyaltyAccountDto, IDomainError>(dto);
        }
    }
}
