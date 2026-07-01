using Core.Interfaces;

namespace Core.Services;

public class UriComposer : IUriComposer
{
    private readonly string _catalogBaseUrl;

    public UriComposer(string catalogBaseUrl)
    {
        _catalogBaseUrl = catalogBaseUrl;
    }

    public string ComposePicUri(string templateUri)
    {
        if (string.IsNullOrEmpty(templateUri)) return templateUri;
        
        if (templateUri.StartsWith("http://catalogbaseurl/"))
        {
            return templateUri.Replace("http://catalogbaseurl/", _catalogBaseUrl);
        }
        
        return templateUri;
    }
}
