using GoodHamburger.Blazor.Components;
using GoodHamburger.Blazor.Services;
using GoodHamburger.Blazor.Services.Interfaces;

namespace GoodHamburger.Blazor;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        // Registrar serviços
        builder.Services.AddScoped<IPedidoService, PedidoService>();
        builder.Services.AddScoped<ICardapioService, CardapioService>();

        // Configurar HttpClient corretamente
        builder.Services.AddHttpClient();
        builder.Services.AddScoped(sp => 
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(builder.Configuration["ApiBaseAddress"] ?? "https://localhost:5001");
            return httpClient;
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
