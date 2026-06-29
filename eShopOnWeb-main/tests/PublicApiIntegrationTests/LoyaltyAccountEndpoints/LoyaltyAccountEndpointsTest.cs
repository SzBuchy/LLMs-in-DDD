using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.eShopWeb;
using Microsoft.eShopWeb.PublicApi.LoyaltyAccountEndpoints;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PublicApiIntegrationTests.Helpers;

namespace PublicApiIntegrationTests.LoyaltyAccountEndpoints;

[TestClass]
public class LoyaltyAccountEndpointsTest
{
    private async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new Exception($"Request failed with status {response.StatusCode} and body {content}");
        }
    }

    [TestMethod]
    public async Task GetLoyaltyAccountReturnsBalanceZeroInitially()
    {
        var client = HttpClientHelper.GetNormalUserClient();
        var response = await client.GetAsync("api/loyalty-account");
        
        await EnsureSuccessAsync(response);
        var stringResponse = await response.Content.ReadAsStringAsync();
        var model = stringResponse.FromJson<GetLoyaltyAccountResponse>();

        Assert.IsNotNull(model);
        Assert.IsNotNull(model.LoyaltyAccount);
    }

    [TestMethod]
    public async Task AwardPointsIncreasesBalance()
    {
        var client = HttpClientHelper.GetNormalUserClient();
        
        // Award points
        var awardRequest = new AwardPointsRequest { Points = 150, OrderId = "order-test-1" };
        var awardContent = new StringContent(JsonSerializer.Serialize(awardRequest), Encoding.UTF8, "application/json");
        var awardResponse = await client.PostAsync("api/loyalty-account/award", awardContent);
        
        await EnsureSuccessAsync(awardResponse);
        var awardString = await awardResponse.Content.ReadAsStringAsync();
        var awardModel = awardString.FromJson<AwardPointsResponse>();

        Assert.IsNotNull(awardModel);
        Assert.IsTrue(awardModel.LoyaltyAccount.Balance >= 150);
    }

    [TestMethod]
    public async Task SpendPointsDecreasesBalance()
    {
        var client = HttpClientHelper.GetNormalUserClient();

        // 1. Get current balance
        var getRes = await client.GetAsync("api/loyalty-account");
        await EnsureSuccessAsync(getRes);
        var getStr = await getRes.Content.ReadAsStringAsync();
        var getModel = getStr.FromJson<GetLoyaltyAccountResponse>();
        int initialBalance = getModel!.LoyaltyAccount.Balance;

        // 2. Award points first
        var awardRequest = new AwardPointsRequest { Points = 300, OrderId = "order-test-2" };
        var awardContent = new StringContent(JsonSerializer.Serialize(awardRequest), Encoding.UTF8, "application/json");
        var awardResponse = await client.PostAsync("api/loyalty-account/award", awardContent);
        await EnsureSuccessAsync(awardResponse);

        // 3. Spend points
        var spendRequest = new SpendPointsRequest { Points = 100, OrderId = "order-test-3" };
        var spendContent = new StringContent(JsonSerializer.Serialize(spendRequest), Encoding.UTF8, "application/json");
        var spendResponse = await client.PostAsync("api/loyalty-account/spend", spendContent);

        await EnsureSuccessAsync(spendResponse);
        var spendString = await spendResponse.Content.ReadAsStringAsync();
        var spendModel = spendString.FromJson<SpendPointsResponse>();

        Assert.IsNotNull(spendModel);
        Assert.AreEqual(initialBalance + 300 - 100, spendModel.LoyaltyAccount.Balance);
    }

    [TestMethod]
    public async Task SpendPointsReturnsBadRequestGivenInsufficientPoints()
    {
        var client = HttpClientHelper.GetNormalUserClient();

        // 1. Get current balance
        var response = await client.GetAsync("api/loyalty-account");
        await EnsureSuccessAsync(response);
        var stringResponse = await response.Content.ReadAsStringAsync();
        var model = stringResponse.FromJson<GetLoyaltyAccountResponse>();
        int balance = model!.LoyaltyAccount.Balance;

        // 2. Try to spend balance + 100 points
        var spendRequest = new SpendPointsRequest { Points = balance + 100, OrderId = "order-test-insufficient" };
        var spendContent = new StringContent(JsonSerializer.Serialize(spendRequest), Encoding.UTF8, "application/json");
        var spendResponse = await client.PostAsync("api/loyalty-account/spend", spendContent);

        Assert.AreEqual(HttpStatusCode.BadRequest, spendResponse.StatusCode);
    }
}
