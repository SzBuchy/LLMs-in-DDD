using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorAdmin;
using System;
using System.Net.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Map HttpClient to the backend PublicApi URL
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri("https://localhost:7175/") 
});

await builder.Build().RunAsync();
