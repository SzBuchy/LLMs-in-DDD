using ApplicationCore.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

public class UriComposer : IUriComposer
{
    private readonly string _baseUrl;

    public UriComposer(IConfiguration configuration)
    {
        _baseUrl = configuration["BaseUrl"] ?? string.Empty;
    }

    public string ComposePicUri(string uriTemplate)
    {
        if (string.IsNullOrWhiteSpace(uriTemplate))
        {
            return uriTemplate;
        }

        return uriTemplate.StartsWith("http", StringComparison.OrdinalIgnoreCase)
            ? uriTemplate
            : _baseUrl.TrimEnd('/') + "/" + uriTemplate.TrimStart('/');
    }
}
