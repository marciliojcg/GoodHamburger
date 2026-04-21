using Xunit;
using Moq;
using GoodHamburger.Application.UseCases;
using GoodHamburger.Application.Ports;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Tests.Application;

public class CardapioUseCasesTests
{
    private readonly Mock<ICardapioRepository> _cardapioRepositoryMock;
    private readonly CardapioUseCases _cardapioUseCases;

    public CardapioUseCasesTests()
    {
        _cardapioRepositoryMock = new Mock<ICardapioRepository>();
        _cardapioUseCases = new CardapioUseCases(_cardapioRepositoryMock.Object);
    }

    [Fact]
    public async Task DeveListarTodosItensDoCardapio_QuandoExistemItensDisponiveis()
    {
        // Arrange
        var itensCardapio = new List<CardapioItem>
        {
            new CardapioItem
            {
                Id = 1,
                Nome = "Hambúrguer Clássico",
                Preco = 25.00m,
                Tipo = TipoItem.Sanduiche,
                Categoria = "Sanduíches",
                Ativo = true
            },
            new CardapioItem
            {
                Id = 2,
                Nome = "Batata Frita",
                Preco = 12.00m,
                Tipo = TipoItem.Acompanhamento,
                Categoria = "Acompanhamentos",
                Ativo = true
            },
            new CardapioItem
            {
                Id = 3,
                Nome = "Refrigerante",
                Preco = 8.00m,
                Tipo = TipoItem.Acompanhamento,
                Categoria = "Bebidas",
                Ativo = true
            }
        };

        _cardapioRepositoryMock
            .Setup(x => x.ObterTodosAsync())
            .ReturnsAsync(itensCardapio);

        // Act
        var resultado = await _cardapioUseCases.ObterCardapioAsync();

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(3, resultado.Count());

        var listaResultado = resultado.ToList();
        Assert.Equal("Hambúrguer Clássico", listaResultado[0].Nome);
        Assert.Equal(25.00m, listaResultado[0].Preco);
        Assert.Equal("Sanduiche", listaResultado[0].Tipo);
        Assert.Equal("Sanduíches", listaResultado[0].Categoria);

        _cardapioRepositoryMock.Verify(x => x.ObterTodosAsync(), Times.Once);
    }

    [Fact]
    public async Task DeveRetornarCardapioVazio_QuandoNaoExistemItensDisponiveis()
    {
        // Arrange
        var itensVazios = new List<CardapioItem>();

        _cardapioRepositoryMock
            .Setup(x => x.ObterTodosAsync())
            .ReturnsAsync(itensVazios);

        // Act
        var resultado = await _cardapioUseCases.ObterCardapioAsync();

        // Assert
        Assert.NotNull(resultado);
        Assert.Empty(resultado);
        _cardapioRepositoryMock.Verify(x => x.ObterTodosAsync(), Times.Once);
    }

    [Fact]
    public async Task DeveMapearCorretamenteOsItens_QuandoConvertendoParaDto()
    {
        // Arrange
        var itemCardapio = new CardapioItem
        {
            Id = 1,
            Nome = "Hambúrguer Premium",
            Preco = 35.50m,
            Tipo = TipoItem.Sanduiche,
            Categoria = "Especiais",
            Ativo = true
        };

        _cardapioRepositoryMock
            .Setup(x => x.ObterTodosAsync())
            .ReturnsAsync(new List<CardapioItem> { itemCardapio });

        // Act
        var resultado = await _cardapioUseCases.ObterCardapioAsync();
        var dtoResultado = resultado.First();

        // Assert
        Assert.Equal("Hambúrguer Premium", dtoResultado.Nome);
        Assert.Equal(35.50m, dtoResultado.Preco);
        Assert.Equal("Sanduiche", dtoResultado.Tipo);
        Assert.Equal("Especiais", dtoResultado.Categoria);
    }

    [Fact]
    public async Task DeveRetornarTodosOsAcompanhamentos_QuandoFiltrandoPorTipo()
    {
        // Arrange
        var itensCardapio = new List<CardapioItem>
        {
            new CardapioItem
            {
                Id = 1,
                Nome = "Hambúrguer",
                Preco = 25.00m,
                Tipo = TipoItem.Sanduiche,
                Categoria = "Sanduíches",
                Ativo = true
            },
            new CardapioItem
            {
                Id = 2,
                Nome = "Batata Frita",
                Preco = 12.00m,
                Tipo = TipoItem.Acompanhamento,
                Categoria = "Acompanhamentos",
                Ativo = true
            },
            new CardapioItem
            {
                Id = 3,
                Nome = "Onion Rings",
                Preco = 14.00m,
                Tipo = TipoItem.Acompanhamento,
                Categoria = "Acompanhamentos",
                Ativo = true
            }
        };

        _cardapioRepositoryMock
            .Setup(x => x.ObterTodosAsync())
            .ReturnsAsync(itensCardapio);

        // Act
        var resultado = await _cardapioUseCases.ObterCardapioAsync();
        var acompanhamentos = resultado.Where(x => x.Tipo == "Acompanhamento").ToList();

        // Assert
        Assert.Equal(2, acompanhamentos.Count);
        Assert.All(acompanhamentos, item => Assert.Equal("Acompanhamento", item.Tipo));
    }
}