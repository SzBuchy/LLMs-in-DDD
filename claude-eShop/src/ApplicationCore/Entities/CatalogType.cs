using ApplicationCore.Interfaces;

namespace ApplicationCore.Entities;

public class CatalogType : BaseEntity, IAggregateRoot
{
    public string Type { get; private set; }

    public CatalogType(string type)
    {
        Type = !string.IsNullOrWhiteSpace(type) ? type : throw new ArgumentException("Type is required", nameof(type));
    }

    public void UpdateType(string type)
    {
        Type = !string.IsNullOrWhiteSpace(type) ? type : throw new ArgumentException("Type is required", nameof(type));
    }
}
