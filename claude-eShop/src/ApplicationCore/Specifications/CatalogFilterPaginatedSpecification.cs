using ApplicationCore.Entities;

namespace ApplicationCore.Specifications;

public class CatalogFilterPaginatedSpecification : BaseSpecification<CatalogItem>
{
    public CatalogFilterPaginatedSpecification(int skip, int take, int? brandId, int? typeId)
        : base(ci =>
            (!brandId.HasValue || ci.CatalogBrandId == brandId) &&
            (!typeId.HasValue || ci.CatalogTypeId == typeId))
    {
        AddInclude(ci => ci.CatalogBrand!);
        AddInclude(ci => ci.CatalogType!);
        ApplyPaging(skip, take);
        ApplyOrderBy(ci => ci.Name);
    }
}
