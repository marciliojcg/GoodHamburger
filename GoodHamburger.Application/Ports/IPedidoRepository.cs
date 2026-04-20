using GoodHamburger.Domain.Entities;

namespace GoodHamburger.Application.Ports;

public interface IPedidoRepository
{
    Task<Pedido> CriarAsync(Pedido pedido);
    Task<Pedido> ObterPorIdAsync(Guid id);
    Task<IEnumerable<Pedido>> ListarTodosAsync();
    Task<Pedido> AtualizarAsync(Pedido pedido);
    Task<bool> RemoverAsync(Guid id);
}