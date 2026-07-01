using ApplicationCore.Entities;

namespace ApplicationCore.Specifications;

public class CatalogFilterSpecification : BaseSpecification<CatalogItem>
{
    public CatalogFilterSpecification(int? brandId, int? typeId)
        : base(ci =>
            (!brandId.HasValue || ci.CatalogBrandId == brandId) &&
            (!typeId.HasValue || ci.CatalogTypeId == typeId))
    {
        AddInclude(ci => ci.CatalogBrand!);
        AddInclude(ci => ci.CatalogType!);
    }
}
