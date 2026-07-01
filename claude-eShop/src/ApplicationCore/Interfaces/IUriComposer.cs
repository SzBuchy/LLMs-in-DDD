namespace ApplicationCore.Interfaces;

// Turns the relative picture path stored on a CatalogItem into a browsable, fully-qualified URI.
public interface IUriComposer
{
    string ComposePicUri(string uriTemplate);
}
