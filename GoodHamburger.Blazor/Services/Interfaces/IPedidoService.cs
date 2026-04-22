using GoodHamburger.Blazor.Models;

namespace GoodHamburger.Blazor.Services.Interfaces;

public interface IPedidoService
{
    Task<List<PedidoResponse>> GetPedidosAsync();
    Task<PedidoResponse> GetPedidoByIdAsync(Guid id);
    Task<PedidoResponse> CreatePedidoAsync(CriarPedidoRequest request);
    Task<PedidoResponse> UpdatePedidoAsync(AtualizarPedidoRequest request);
    Task<bool> DeletePedidoAsync(Guid id);
}
