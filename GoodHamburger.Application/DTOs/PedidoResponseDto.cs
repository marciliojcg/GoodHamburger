public class PedidoResponseDto
{
    public Guid Id { get; set; }
    public DateTime DataCriacao { get; set; }
    public string Status { get; set; }
    public List<ItemPedidoResponseDto> Itens { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Desconto { get; set; }
    public decimal Total { get; set; }
}