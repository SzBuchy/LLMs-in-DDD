using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.eShopWeb;
using Microsoft.eShopWeb.ApplicationCore.Entities.ReviewAggregate;
using Microsoft.eShopWeb.Infrastructure.Data;
using Microsoft.eShopWeb.PublicApi.ProductReviewEndpoints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PublicApiIntegrationTests.Helpers;

namespace PublicApiIntegrationTests.ProductReviewEndpoints;

[TestClass]
public class ProductReviewEndpointsTest
{
    private async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new Exception($"Request failed with status {response.StatusCode} and body {content}");
        }
    }

    [TestMethod]
    public async Task CreateProductReviewSucceedsAndCanBeEditedAfterPublishing()
    {
        var client = HttpClientHelper.GetNormalUserClient();

        // 1. Create a product review
        var createRequest = new CreateProductReviewRequest
        {
            CatalogItemId = 1,
            Rating = 5,
            TextContent = "This is a very good product review with sufficient length."
        };
        var createContent = new StringContent(JsonSerializer.Serialize(createRequest), Encoding.UTF8, "application/json");
        var createResponse = await client.PostAsync("api/product-reviews", createContent);
        await EnsureSuccessAsync(createResponse);

        var createString = await createResponse.Content.ReadAsStringAsync();
        var createModel = createString.FromJson<CreateProductReviewResponse>();

        Assert.IsNotNull(createModel);
        Assert.AreEqual(1, createModel.ProductReview.CatalogItemId);
        Assert.AreEqual(5, createModel.ProductReview.Rating);
        Assert.AreEqual("This is a very good product review with sufficient length.", createModel.ProductReview.TextContent);
        Assert.AreEqual(ReviewStatus.PendingModeration.ToString(), createModel.ProductReview.Status);

        var reviewId = createModel.ProductReview.Id;

        // 2. Approve the review via DB context
        using (var scope = ProgramTest.App.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CatalogContext>();
            var review = await context.ProductReviews.FindAsync(reviewId);
            Assert.IsNotNull(review);
            review.Approve();
            await context.SaveChangesAsync();
        }

        // 3. Edit the review
        var editRequest = new EditProductReviewRequest
        {
            ReviewId = reviewId,
            Rating = 3,
            TextContent = "Updated review content with sufficient length."
        };
        var editContent = new StringContent(JsonSerializer.Serialize(editRequest), Encoding.UTF8, "application/json");
        var editResponse = await client.PutAsync("api/product-reviews", editContent);
        await EnsureSuccessAsync(editResponse);

        var editString = await editResponse.Content.ReadAsStringAsync();
        var editModel = editString.FromJson<EditProductReviewResponse>();

        Assert.IsNotNull(editModel);
        Assert.AreEqual(reviewId, editModel.ProductReview.Id);
        Assert.AreEqual(3, editModel.ProductReview.Rating);
        Assert.AreEqual("Updated review content with sufficient length.", editModel.ProductReview.TextContent);
        Assert.AreEqual(ReviewStatus.PendingModeration.ToString(), editModel.ProductReview.Status);
    }

    [TestMethod]
    public async Task EditProductReviewReturnsBadRequestWhenNotPublished()
    {
        var client = HttpClientHelper.GetNormalUserClient();

        // 1. Create a product review (starts as PendingModeration)
        var createRequest = new CreateProductReviewRequest
        {
            CatalogItemId = 2,
            Rating = 4,
            TextContent = "This is a very good product review for testing edit failure."
        };
        var createContent = new StringContent(JsonSerializer.Serialize(createRequest), Encoding.UTF8, "application/json");
        var createResponse = await client.PostAsync("api/product-reviews", createContent);
        await EnsureSuccessAsync(createResponse);

        var createString = await createResponse.Content.ReadAsStringAsync();
        var createModel = createString.FromJson<CreateProductReviewResponse>();
        var reviewId = createModel!.ProductReview.Id;

        // 2. Attempt to edit the pending review (should fail with 400 Bad Request)
        var editRequest = new EditProductReviewRequest
        {
            ReviewId = reviewId,
            Rating = 2,
            TextContent = "Updated review content for pending review."
        };
        var editContent = new StringContent(JsonSerializer.Serialize(editRequest), Encoding.UTF8, "application/json");
        var editResponse = await client.PutAsync("api/product-reviews", editContent);

        Assert.AreEqual(HttpStatusCode.BadRequest, editResponse.StatusCode);
    }
}
