using GoodHamburger.Blazor.Models;
using GoodHamburger.Blazor.Services.Interfaces;

namespace GoodHamburger.Blazor.Services;

public class CardapioService : ICardapioService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly string _apiUrl;

    public CardapioService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        //_apiUrl = _configuration["ApiBaseAddress"] ?? "https://localhost:7089";
        //_httpClient.BaseAddress = new Uri(_apiUrl);
    }

    public async Task<List<CardapioItemResponse>> GetCardapioAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/Cardapio");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<CardapioItemResponse>>() ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar cardápio: {ex.Message}");
            return new();
        }
    }

    public async Task<List<CardapioItemResponse>> GetSanduichesAsync()
    {
        var cardapio = await GetCardapioAsync();
        return cardapio.Where(x => x.Tipo == "Sanduiche").ToList();
    }

    public async Task<List<CardapioItemResponse>> GetAcompanhamentosAsync()
    {
        var cardapio = await GetCardapioAsync();
        return cardapio.Where(x => x.Tipo == "Acompanhamento").ToList();
    }
}