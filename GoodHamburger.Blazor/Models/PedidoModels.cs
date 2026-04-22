namespace GoodHamburger.Blazor.Models;

public class CriarPedidoRequest
{
    public List<ItemPedidoRequest> Itens { get; set; } = new();
}

public class AtualizarPedidoRequest
{
    public Guid Id { get; set; }
    public List<ItemPedidoRequest> Itens { get; set; } = new();
}

public class ItemPedidoRequest
{
    public string Nome { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}

public class PedidoResponse
{
    public Guid Id { get; set; }
    public DateTime DataCriacao { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<ItemPedidoResponse> Itens { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal Desconto { get; set; }
    public decimal Total { get; set; }
}

public class ItemPedidoResponse
{
    public string Nome { get; set; } = string.Empty;
    public decimal PrecoUnitario { get; set; }
    public int Quantidade { get; set; }
    public decimal Subtotal { get; set; }
}

public class CardapioItemResponse
{
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
}