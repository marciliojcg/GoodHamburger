using GoodHamburger.Blazor.Models;
using GoodHamburger.Blazor.Services.Interfaces;

namespace GoodHamburger.Blazor.Services;

public class PedidoService : IPedidoService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly string _apiUrl;

    public PedidoService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _apiUrl = _configuration["ApiBaseAddress"] ?? "https://localhost:7089";
        _httpClient.BaseAddress = new Uri(_apiUrl);
    }

    public async Task<List<PedidoResponse>> GetPedidosAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/Pedidos");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<PedidoResponse>>() ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar pedidos: {ex.Message}");
            return new();
        }
    }

    public async Task<PedidoResponse> GetPedidoByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/Pedidos/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PedidoResponse>() ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar pedido {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<PedidoResponse> CreatePedidoAsync(CriarPedidoRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Pedidos", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PedidoResponse>() ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao criar pedido: {ex.Message}");
            throw;
        }
    }

    public async Task<PedidoResponse> UpdatePedidoAsync(AtualizarPedidoRequest request)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Pedidos/{request.Id}", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PedidoResponse>() ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar pedido: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> DeletePedidoAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/Pedidos/{id}");
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao deletar pedido: {ex.Message}");
            return false;
        }
    }
}