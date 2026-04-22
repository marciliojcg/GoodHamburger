using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;
using GoodHamburger.Domain.Exceptions;

namespace GoodHamburger.Tests.Domain.Entities;

public class PedidoTests
{
    private readonly Pedido _pedido;

    public PedidoTests()
    {
        _pedido = new Pedido
        {
            Id = Guid.NewGuid(),
            DataCriacao = DateTime.Now,
            Status = "Pendente",
            Itens = new List<ItemPedido>()
        };
    }

    #region Testes de Cálculo de Totais

    [Fact]
    public void DeveCalcularSubtotalCorretamente_QuandoPedidoPossuiUnicSanduiche()
    {
        // Arrange
        var sanduiche = CriarItemPedido("Hambúrguer Clássico", TipoItem.Sanduiche, 25.00m, 1);
        _pedido.Itens.Add(sanduiche);

        // Act
        _pedido.CalcularTotais();

        // Assert
        Assert.Equal(25.00m, _pedido.Subtotal);
    }

    [Fact]
    public void DeveCalcularSubtotalCorretamente_QuandoPedidoPossuiMultiplosItens()
    {
        // Arrange
        var sanduiche = CriarItemPedido("Hambúrguer Clássico", TipoItem.Sanduiche, 25.00m, 1);
        var batata = CriarItemPedido("Batata frita", TipoItem.Acompanhamento, 10.00m, 1);
        var refrigerante = CriarItemPedido("Refrigerante", TipoItem.Acompanhamento, 5.00m, 1);

        _pedido.Itens.AddRange(new[] { sanduiche, batata, refrigerante });

        // Act
        _pedido.CalcularTotais();

        // Assert
        Assert.Equal(40.00m, _pedido.Subtotal);
    }

    [Fact]
    public void DeveCalcularSubtotalComQuantidadesMaioresQueUm_QuandoPedidoPossuiMultiplosQuantidades()
    {
        // Arrange
        var sanduiche = CriarItemPedido("Hambúrguer Clássico", TipoItem.Sanduiche, 25.00m, 2);
        var batata = CriarItemPedido("Batata frita", TipoItem.Acompanhamento, 10.00m, 3);

        _pedido.Itens.AddRange(new[] { sanduiche, batata });

        // Act
        _pedido.CalcularTotais();

        // Assert
        Assert.Equal(80.00m, _pedido.Subtotal);
    }

    [Fact]
    public void DeveCalcularDescontoComVinte_QuandoPedidoPossuiSanduicheBatataERefrigerante()
    {
        // Arrange
        var sanduiche = CriarItemPedido("Hambúrguer Clássico", TipoItem.Sanduiche, 25.00m, 1);
        var batata = CriarItemPedido("Batata frita", TipoItem.Acompanhamento, 10.00m, 1);
        var refrigerante = CriarItemPedido("Refrigerante", TipoItem.Acompanhamento, 5.00m, 1);

        _pedido.Itens.AddRange(new[] { sanduiche, batata, refrigerante });

        // Act
        _pedido.CalcularTotais();

        // Assert
        Assert.Equal(8.00m, _pedido.Desconto); // 20% de R$ 40,00
    }

    [Fact]
    public void DeveCalcularDescontoComQuinze_QuandoPedidoPossuiSanduicheERefrigerante()
    {
        // Arrange
        var sanduiche = CriarItemPedido("Hambúrguer Clássico", TipoItem.Sanduiche, 25.00m, 1);
        var refrigerante = CriarItemPedido("Refrigerante", TipoItem.Acompanhamento, 5.00m, 1);

        _pedido.Itens.AddRange(new[] { sanduiche, refrigerante });

        // Act
        _pedido.CalcularTotais();

        // Assert
        Assert.Equal(4.50m, _pedido.Desconto); // 15% de R$ 30,00
    }

    [Fact]
    public void DeveCalcularDescontoComDez_QuandoPedidoPossuiSanduicheEBatata()
    {
        // Arrange
        var sanduiche = CriarItemPedido("Hambúrguer Clássico", TipoItem.Sanduiche, 25.00m, 1);
        var batata = CriarItemPedido("Batata frita", TipoItem.Acompanhamento, 10.00m, 1);

        _pedido.Itens.AddRange(new[] { sanduiche, batata });

        // Act
        _pedido.CalcularTotais();

        // Assert
        Assert.Equal(3.50m, _pedido.Desconto); // 10% de R$ 35,00
    }

    [Fact]
    public void DeveCalcularDescontoZero_QuandoPedidoPossuiApenasUmSanduiche()
    {
        // Arrange
        var sanduiche = CriarItemPedido("Hambúrguer Clássico", TipoItem.Sanduiche, 25.00m, 1);
        _pedido.Itens.Add(sanduiche);

        // Act
        _pedido.CalcularTotais();

        // Assert
        Assert.Equal(0m, _pedido.Desconto);
    }

    [Fact]
    public void DeveCalcularTotalCorretamente_QuandoPedidoPossuiDescontoDeVinte()
    {
        // Arrange
        var sanduiche = CriarItemPedido("Hambúrguer Clássico", TipoItem.Sanduiche, 25.00m, 1);
        var batata = CriarItemPedido("Batata frita", TipoItem.Acompanhamento, 10.00m, 1);
        var refrigerante = CriarItemPedido("Refrigerante", TipoItem.Acompanhamento, 5.00m, 1);

        _pedido.Itens.AddRange(new[] { sanduiche, batata, refrigerante });

        // Act
        _pedido.CalcularTotais();

        // Assert
        Assert.Equal(32.00m, _pedido.Total); // 40 - 8 = 32
    }

    [Fact]
    public void DeveCalcularTotalCorretamente_QuandoPedidoPossuiDescontoDeQuinze()
    {
        // Arrange
        var sanduiche = CriarItemPedido("Hambúrguer Clássico", TipoItem.Sanduiche, 25.00m, 1);
        var refrigerante = CriarItemPedido("Refrigerante", TipoItem.Acompanhamento, 5.00m, 1);

        _pedido.Itens.AddRange(new[] { sanduiche, refrigerante });

        // Act
        _pedido.CalcularTotais();

        // Assert
        Assert.Equal(25.50m, _pedido.Total); // 30 - 4.50 = 25.50
    }

    [Fact]
    public void DeveCalcularTotalCorretamente_QuandoPedidoPossuiDescontoDeDez()
    {
        // Arrange
        var sanduiche = CriarItemPedido("Hambúrguer Clássico", TipoItem.Sanduiche, 25.00m, 1);
        var batata = CriarItemPedido("Batata frita", TipoItem.Acompanhamento, 10.00m, 1);

        _pedido.Itens.AddRange(new[] { sanduiche, batata });

        // Act
        _pedido.CalcularTotais();

        // Assert
        Assert.Equal(31.50m, _pedido.Total); // 35 - 3.50 = 31.50
    }

    [Fact]
    public void DeveCalcularTotalIgualSubtotal_QuandoNaoHaDesconto()
    {
        // Arrange
        var sanduiche = CriarItemPedido("Hambúrguer Clássico", TipoItem.Sanduiche, 25.00m, 1);
        _pedido.Itens.Add(sanduiche);

        // Act
        _pedido.CalcularTotais();

        // Assert
        Assert.Equal(_pedido.Subtotal, _pedido.Total);
    }

    #endregion

    #region Testes de Validação de Itens Duplicados

    [Fact]
    public void DeveLancarException_QuandoPedidoPossuiMaisDeUmSanduiche()
    {
        // Arrange
        var sanduiche = CriarItemPedido("Hambúrguer Clássico", TipoItem.Sanduiche, 25.00m, 2);
        _pedido.Itens.Add(sanduiche);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => _pedido.ValidarItensDuplicados());
        Assert.Equal("Cada pedido pode conter apenas um sanduíche", exception.Message);
    }

    [Fact]
    public void DeveLancarException_QuandoPedidoPossuiMaisDeUmaBatataFrita()
    {
        // Arrange
        var batata = CriarItemPedido("Batata frita", TipoItem.Acompanhamento, 10.00m, 2);
        _pedido.Itens.Add(batata);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => _pedido.ValidarItensDuplicados());
        Assert.Equal("Cada pedido pode conter apenas uma batata frita", exception.Message);
    }

    [Fact]
    public void DeveLancarException_QuandoPedidoPossuiMaisDeUmRefrigerante()
    {
        // Arrange
        var refrigerante = CriarItemPedido("Refrigerante", TipoItem.Acompanhamento, 5.00m, 2);
        _pedido.Itens.Add(refrigerante);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => _pedido.ValidarItensDuplicados());
        Assert.Equal("Cada pedido pode conter apenas um refrigerante", exception.Message);
    }

    [Fact]
    public void NaoDeveLancarException_QuandoPedidoPossuiApenasUmSanduiche()
    {
        // Arrange
        var sanduiche = CriarItemPedido("Hambúrguer Clássico", TipoItem.Sanduiche, 25.00m, 1);
        _pedido.Itens.Add(sanduiche);

        // Act & Assert
        _pedido.ValidarItensDuplicados();
    }

    [Fact]
    public void NaoDeveLancarException_QuandoPedidoPossuiValidacaoComItensUnicos()
    {
        // Arrange
        var sanduiche = CriarItemPedido("Hambúrguer Clássico", TipoItem.Sanduiche, 25.00m, 1);
        var batata = CriarItemPedido("Batata frita", TipoItem.Acompanhamento, 10.00m, 1);
        var refrigerante = CriarItemPedido("Refrigerante", TipoItem.Acompanhamento, 5.00m, 1);

        _pedido.Itens.AddRange(new[] { sanduiche, batata, refrigerante });

        // Act & Assert
        _pedido.ValidarItensDuplicados();
    }

    [Fact]
    public void NaoDeveLancarException_QuandoPedidoNaoContemItensAlemDoLimite()
    {
        // Arrange
        var refrigerante = CriarItemPedido("Refrigerante", TipoItem.Acompanhamento, 5.00m, 1);
        _pedido.Itens.Add(refrigerante);

        // Act & Assert
        _pedido.ValidarItensDuplicados();
    }

    #endregion

    #region Métodos Auxiliares

    private static ItemPedido CriarItemPedido(
        string nome,
        TipoItem tipo,
        decimal precoUnitario,
        int quantidade)
    {
        return new ItemPedido
        {
            Id = Guid.NewGuid(),
            Nome = nome,
            Tipo = tipo,
            PrecoUnitario = precoUnitario,
            Quantidade = quantidade,
            PedidoId = Guid.NewGuid()
        };
    }

    #endregion
}