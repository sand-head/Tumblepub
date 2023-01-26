using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Tumblepub.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.RegisterCustomElement<TestButton>("test-button");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
});

await builder.Build().RunAsync();