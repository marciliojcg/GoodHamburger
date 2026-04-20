using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Domain.Entities;

public class ItemPedido
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public TipoItem Tipo { get; set; }
    public decimal PrecoUnitario { get; set; }
    public int Quantidade { get; set; }
    public Guid PedidoId { get; set; }
}
