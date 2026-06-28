using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BlazorShared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.eShopWeb;
using Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;
using Microsoft.eShopWeb.Infrastructure.Data;
using Microsoft.eShopWeb.PublicApi.ReviewEndpoints;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PublicApiIntegrationTests.Helpers;

namespace PublicApiIntegrationTests.ReviewEndpoints;

[TestClass]
public class UpdateReviewEndpointTest
{
    private const int ExistingCatalogItemId = 1;
    private const int Rating = 4;
    private const string Content = "Produkt po edycji nadal jest wart polecenia.";

    [TestMethod]
    public async Task ReturnsUnauthorizedGivenAnonymousUser()
    {
        var reviewId = await AddPublishedReviewAsync("demouser@microsoft.com");
        var client = ProgramTest.NewClient;

        var response = await client.PutAsync(
            $"api/catalog-items/{ExistingCatalogItemId}/reviews/{reviewId}",
            GetValidUpdateReviewJson());

        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task ReturnsNotFoundGivenMissingReview()
    {
        var client = HttpClientHelper.GetNormalUserClient();

        var response = await client.PutAsync(
            $"api/catalog-items/{ExistingCatalogItemId}/reviews/999999",
            GetValidUpdateReviewJson());

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task ReturnsForbiddenGivenOtherCustomerReview()
    {
        var reviewId = await AddPublishedReviewAsync("otheruser@microsoft.com");
        var client = HttpClientHelper.GetNormalUserClient();

        var response = await client.PutAsync(
            $"api/catalog-items/{ExistingCatalogItemId}/reviews/{reviewId}",
            GetValidUpdateReviewJson());

        Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [TestMethod]
    public async Task ReturnsBadRequestGivenInvalidReview()
    {
        var reviewId = await AddPublishedReviewAsync("demouser@microsoft.com");
        var client = HttpClientHelper.GetNormalUserClient();

        var response = await client.PutAsync(
            $"api/catalog-items/{ExistingCatalogItemId}/reviews/{reviewId}",
            GetInvalidUpdateReviewJson());

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task ReturnsSuccessAndSendsPublishedReviewBackToModerationGivenOwner()
    {
        var reviewId = await AddPublishedReviewAsync("demouser@microsoft.com");
        var client = HttpClientHelper.GetNormalUserClient();

        var response = await client.PutAsync(
            $"api/catalog-items/{ExistingCatalogItemId}/reviews/{reviewId}",
            GetValidUpdateReviewJson());
        response.EnsureSuccessStatusCode();
        var stringResponse = await response.Content.ReadAsStringAsync();
        var model = stringResponse.FromJson<UpdateReviewResponse>();

        Assert.IsNotNull(model);
        Assert.AreEqual(reviewId, model.Review.Id);
        Assert.AreEqual("demouser@microsoft.com", model.Review.BuyerId);
        Assert.AreEqual(ExistingCatalogItemId, model.Review.CatalogItemId);
        Assert.AreEqual(Rating, model.Review.Rating);
        Assert.AreEqual(Content, model.Review.Content);
        Assert.AreEqual(ReviewStatus.PendingModeration, model.Review.Status);
    }

    private static async Task<int> AddPublishedReviewAsync(string buyerId)
    {
        using var scope = ProgramTest.Services.CreateScope();
        var catalogContext = scope.ServiceProvider.GetRequiredService<CatalogContext>();
        var review = new Review(buyerId, ExistingCatalogItemId, 5, "Produkt jest bardzo dobry i spełnia oczekiwania.");
        review.Publish();

        catalogContext.Reviews.Add(review);
        await catalogContext.SaveChangesAsync();
        catalogContext.Entry(review).State = EntityState.Detached;

        return review.Id;
    }

    private static StringContent GetValidUpdateReviewJson()
    {
        var request = new UpdateReviewRequest
        {
            Rating = Rating,
            Content = Content
        };

        return new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
    }

    private static StringContent GetInvalidUpdateReviewJson()
    {
        var request = new UpdateReviewRequest
        {
            Rating = 6,
            Content = "Krótka"
        };

        return new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
    }
}
