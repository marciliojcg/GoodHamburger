using GoodHamburger.Domain.Enums;
using GoodHamburger.Domain.Exceptions;

namespace GoodHamburger.Domain.Entities;

public class Pedido
{
    public Guid Id { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    public string Status { get; set; }
    public List<ItemPedido> Itens { get; set; }
    public decimal Subtotal { get; private set; }
    public decimal Desconto { get; private set; }
    public decimal Total { get; private set; }

    public void CalcularTotais()
    {
        Subtotal = Itens.Sum(i => i.PrecoUnitario * i.Quantidade);
        CalcularDesconto();
        Total = Subtotal - Desconto;
    }

    private void CalcularDesconto()
    {
        var temSanduiche = Itens.Any(i => i.Tipo == TipoItem.Sanduiche);
        var temBatata = Itens.Any(i => i.Tipo == TipoItem.Acompanhamento && i.Nome == "Batata frita");
        var temRefrigerante = Itens.Any(i => i.Tipo == TipoItem.Acompanhamento && i.Nome == "Refrigerante");

        if (temSanduiche && temBatata && temRefrigerante)
            Desconto = Subtotal * 0.20m;
        else if (temSanduiche && temRefrigerante)
            Desconto = Subtotal * 0.15m;
        else if (temSanduiche && temBatata)
            Desconto = Subtotal * 0.10m;
        else
            Desconto = 0;
    }

    public void ValidarItensDuplicados()
    {
        var sanduiches = Itens.Count(i => i.Tipo == TipoItem.Sanduiche);
        var batatas = Itens.Count(i => i.Nome == "Batata frita");
        var refrigerantes = Itens.Count(i => i.Nome == "Refrigerante");

        if (sanduiches > 1)
            throw new DomainException("Cada pedido pode conter apenas um sanduíche");
        if (batatas > 1)
            throw new DomainException("Cada pedido pode conter apenas uma batata frita");
        if (refrigerantes > 1)
            throw new DomainException("Cada pedido pode conter apenas um refrigerante");
    }
}


