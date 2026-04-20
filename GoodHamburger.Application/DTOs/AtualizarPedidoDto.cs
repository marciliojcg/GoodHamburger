public class AtualizarPedidoDto
{
    public Guid Id { get; set; }
    public List<ItemPedidoDto> Itens { get; set; }
}