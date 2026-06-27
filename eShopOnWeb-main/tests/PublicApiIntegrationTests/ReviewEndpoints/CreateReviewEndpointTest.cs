using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BlazorShared.Models;
using Microsoft.eShopWeb;
using Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;
using Microsoft.eShopWeb.PublicApi.ReviewEndpoints;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PublicApiIntegrationTests.Helpers;

namespace PublicApiIntegrationTests.ReviewEndpoints;

[TestClass]
public class CreateReviewEndpointTest
{
    private const int ExistingCatalogItemId = 1;
    private const int MissingCatalogItemId = 999999;
    private const int Rating = 5;
    private const string Content = "Produkt jest bardzo dobry i spełnia oczekiwania.";

    [TestMethod]
    public async Task ReturnsUnauthorizedGivenAnonymousUser()
    {
        var client = ProgramTest.NewClient;

        var response = await client.PostAsync(
            $"api/catalog-items/{ExistingCatalogItemId}/reviews",
            GetValidNewReviewJson());

        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task ReturnsNotFoundGivenMissingCatalogItem()
    {
        var client = HttpClientHelper.GetNormalUserClient();

        var response = await client.PostAsync(
            $"api/catalog-items/{MissingCatalogItemId}/reviews",
            GetValidNewReviewJson());

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task ReturnsBadRequestGivenInvalidReview()
    {
        var client = HttpClientHelper.GetNormalUserClient();

        var response = await client.PostAsync(
            $"api/catalog-items/{ExistingCatalogItemId}/reviews",
            GetInvalidNewReviewJson());

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task ReturnsSuccessGivenValidReviewAndNormalUserToken()
    {
        var client = HttpClientHelper.GetNormalUserClient();

        var response = await client.PostAsync(
            $"api/catalog-items/{ExistingCatalogItemId}/reviews",
            GetValidNewReviewJson());
        response.EnsureSuccessStatusCode();
        var stringResponse = await response.Content.ReadAsStringAsync();
        var model = stringResponse.FromJson<CreateReviewResponse>();

        Assert.IsNotNull(model);
        Assert.AreEqual("demouser@microsoft.com", model.Review.BuyerId);
        Assert.AreEqual(ExistingCatalogItemId, model.Review.CatalogItemId);
        Assert.AreEqual(Rating, model.Review.Rating);
        Assert.AreEqual(Content, model.Review.Content);
        Assert.AreEqual(ReviewStatus.PendingModeration, model.Review.Status);
    }

    private static StringContent GetValidNewReviewJson()
    {
        var request = new CreateReviewRequest
        {
            Rating = Rating,
            Content = Content
        };

        return new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
    }

    private static StringContent GetInvalidNewReviewJson()
    {
        var request = new CreateReviewRequest
        {
            Rating = 6,
            Content = "Krótka"
        };

        return new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
    }
}
