using Xunit;
using Moq;
using GoodHamburger.Application.UseCases;
using GoodHamburger.Application.Ports;
using GoodHamburger.Application.DTOs;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;
using GoodHamburger.Domain.Exceptions;

namespace GoodHamburger.Tests.Application;

public class PedidoUseCasesTests
{
    private readonly Mock<IPedidoRepository> _pedidoRepositoryMock;
    private readonly Mock<ICardapioRepository> _cardapioRepositoryMock;
    private readonly PedidoUseCases _pedidoUseCases;

    public PedidoUseCasesTests()
    {
        _pedidoRepositoryMock = new Mock<IPedidoRepository>();
        _cardapioRepositoryMock = new Mock<ICardapioRepository>();
        _pedidoUseCases = new PedidoUseCases(_pedidoRepositoryMock.Object, _cardapioRepositoryMock.Object);
    }

    #region Criar Pedido

    [Fact]
    public async Task DeveCreatePedidoComSucesso_QuandoTodosOsItensExistemNoCardapio()
    {
        // Arrange
        var hamburguer = new CardapioItem
        {
            Id = 1,
            Nome = "Hambúrguer Clássico",
            Preco = 25.00m,
            Tipo = TipoItem.Sanduiche,
            Categoria = "Sanduíches",
            Ativo = true
        };

        var criarPedidoDto = new CriarPedidoDto
        {
            Itens = new List<ItemPedidoDto>
            {
                new ItemPedidoDto { Nome = "Hambúrguer Clássico", Quantidade = 1 }
            }
        };

        var pedidoEsperado = new Pedido
        {
            Id = Guid.NewGuid(),
            DataCriacao = DateTime.UtcNow,
            Status = "Pendente",
            Itens = new List<ItemPedido>
            {
                new ItemPedido
                {
                    Id = Guid.NewGuid(),
                    Nome = "Hambúrguer Clássico",
                    Tipo = TipoItem.Sanduiche,
                    PrecoUnitario = 25.00m,
                    Quantidade = 1
                }
            }
        };

        _cardapioRepositoryMock
            .Setup(x => x.ObterPorNomeAsync("Hambúrguer Clássico"))
            .ReturnsAsync(hamburguer);

        _pedidoRepositoryMock
            .Setup(x => x.CriarAsync(It.IsAny<Pedido>()))
            .ReturnsAsync(pedidoEsperado);

        // Act
        var resultado = await _pedidoUseCases.CriarPedidoAsync(criarPedidoDto);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Pendente", resultado.Status);
        Assert.Single(resultado.Itens);

        _cardapioRepositoryMock.Verify(x => x.ObterPorNomeAsync("Hambúrguer Clássico"), Times.Once);
        _pedidoRepositoryMock.Verify(x => x.CriarAsync(It.IsAny<Pedido>()), Times.Once);
    }

    [Fact]
    public async Task DeveAplicarDescontoDeVintePercento_QuandoPedidoTemSanduicheBatataERefrigerante()
    {
        // Arrange
        var hamburguer = new CardapioItem
        {
            Id = 1,
            Nome = "Hambúrguer Clássico",
            Preco = 25.00m,
            Tipo = TipoItem.Sanduiche,
            Categoria = "Sanduíches",
            Ativo = true
        };

        var batataFrita = new CardapioItem
        {
            Id = 2,
            Nome = "Batata frita",
            Preco = 12.00m,
            Tipo = TipoItem.Acompanhamento,
            Categoria = "Acompanhamentos",
            Ativo = true
        };

        var refrigerante = new CardapioItem
        {
            Id = 3,
            Nome = "Refrigerante",
            Preco = 8.00m,
            Tipo = TipoItem.Acompanhamento,
            Categoria = "Bebidas",
            Ativo = true
        };

        var criarPedidoDto = new CriarPedidoDto
        {
            Itens = new List<ItemPedidoDto>
            {
                new ItemPedidoDto { Nome = "Hambúrguer Clássico", Quantidade = 1 },
                new ItemPedidoDto { Nome = "Batata frita", Quantidade = 1 },
                new ItemPedidoDto { Nome = "Refrigerante", Quantidade = 1 }
            }
        };

        var pedidoEsperado = new Pedido
        {
            Id = Guid.NewGuid(),
            DataCriacao = DateTime.UtcNow,
            Status = "Pendente",
            Itens = new List<ItemPedido>
            {
                new ItemPedido { Id = Guid.NewGuid(), Nome = "Hambúrguer Clássico", Tipo = TipoItem.Sanduiche, PrecoUnitario = 25.00m, Quantidade = 1 },
                new ItemPedido { Id = Guid.NewGuid(), Nome = "Batata frita", Tipo = TipoItem.Acompanhamento, PrecoUnitario = 12.00m, Quantidade = 1 },
                new ItemPedido { Id = Guid.NewGuid(), Nome = "Refrigerante", Tipo = TipoItem.Acompanhamento, PrecoUnitario = 8.00m, Quantidade = 1 }
            }
          
        };

        _cardapioRepositoryMock
            .Setup(x => x.ObterPorNomeAsync(It.IsAny<string>()))
            .Returns((string nome) => Task.FromResult(new[] { hamburguer, batataFrita, refrigerante }
                .FirstOrDefault(x => x.Nome == nome)));

        _pedidoRepositoryMock
            .Setup(x => x.CriarAsync(It.IsAny<Pedido>()))
            .ReturnsAsync(pedidoEsperado);

        // Act
        var resultado = await _pedidoUseCases.CriarPedidoAsync(criarPedidoDto);

        // Assert
        Assert.Equal(45.00m, resultado.Subtotal);
        Assert.Equal(9.00m, resultado.Desconto);
        Assert.Equal(36.00m, resultado.Total);
    }

    [Fact]
    public async Task DeveLancarExcecao_QuandoTentarCriarPedidoComItemNaoExistenteNoCardapio()
    {
        // Arrange
        var criarPedidoDto = new CriarPedidoDto
        {
            Itens = new List<ItemPedidoDto>
            {
                new ItemPedidoDto { Nome = "ItemInexistente", Quantidade = 1 }
            }
        };

        _cardapioRepositoryMock
            .Setup(x => x.ObterPorNomeAsync("ItemInexistente"))
            .ReturnsAsync((CardapioItem)null);

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => 
            _pedidoUseCases.CriarPedidoAsync(criarPedidoDto));

        _cardapioRepositoryMock.Verify(x => x.ObterPorNomeAsync("ItemInexistente"), Times.Once);
        _pedidoRepositoryMock.Verify(x => x.CriarAsync(It.IsAny<Pedido>()), Times.Never);
    }

    [Fact]
    public async Task DeveLancarExcecao_QuandoTentarCriarPedidoComMaisDeSanduiche()
    {
        // Arrange
        var hamburguer = new CardapioItem
        {
            Id = 1,
            Nome = "Hambúrguer Clássico",
            Preco = 25.00m,
            Tipo = TipoItem.Sanduiche,
            Categoria = "Sanduíches",
            Ativo = true
        };

        var criarPedidoDto = new CriarPedidoDto
        {
            Itens = new List<ItemPedidoDto>
            {
                new ItemPedidoDto { Nome = "Hambúrguer Clássico", Quantidade = 1 },
                new ItemPedidoDto { Nome = "Hambúrguer Clássico", Quantidade = 1 }
            }
        };

        _cardapioRepositoryMock
            .Setup(x => x.ObterPorNomeAsync("Hambúrguer Clássico"))
            .ReturnsAsync(hamburguer);

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => 
            _pedidoUseCases.CriarPedidoAsync(criarPedidoDto));

        _cardapioRepositoryMock.Verify(x => x.ObterPorNomeAsync("Hambúrguer Clássico"), Times.Exactly(2));
        _pedidoRepositoryMock.Verify(x => x.CriarAsync(It.IsAny<Pedido>()), Times.Never);
    }

    #endregion

    #region Obter Pedido

    [Fact]
    public async Task DeveObterPedidoComSucesso_QuandoPedidoExiste()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedidoEsperado = new Pedido
        {
            Id = pedidoId,
            DataCriacao = DateTime.UtcNow,
            Status = "Pendente",
            Itens = new List<ItemPedido>
            {
                new ItemPedido
                {
                    Id = Guid.NewGuid(),
                    Nome = "Hambúrguer Clássico",
                    Tipo = TipoItem.Sanduiche,
                    PrecoUnitario = 25.00m,
                    Quantidade = 1
                }
            }
        };

        _pedidoRepositoryMock
            .Setup(x => x.ObterPorIdAsync(pedidoId))
            .ReturnsAsync(pedidoEsperado);

        // Act
        var resultado = await _pedidoUseCases.ObterPedidoAsync(pedidoId);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(pedidoId, resultado.Id);
        Assert.Equal("Pendente", resultado.Status);
  
        _pedidoRepositoryMock.Verify(x => x.ObterPorIdAsync(pedidoId), Times.Once);
    }

    [Fact]
    public async Task DeveLancarExcecao_QuandoTentarObterPedidoInexistente()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();

        _pedidoRepositoryMock
            .Setup(x => x.ObterPorIdAsync(pedidoId))
            .ReturnsAsync((Pedido)null);

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => 
            _pedidoUseCases.ObterPedidoAsync(pedidoId));

        _pedidoRepositoryMock.Verify(x => x.ObterPorIdAsync(pedidoId), Times.Once);
    }

    #endregion

    #region Listar Pedidos

    [Fact]
    public async Task DeveListarTodosPedidos_QuandoExistemPedidosDisponiveis()
    {
        // Arrange
        var pedidosEsperados = new List<Pedido>
        {
            new Pedido
            {
                Id = Guid.NewGuid(),
                DataCriacao = DateTime.UtcNow,
                Status = "Pendente",
                Itens = new List<ItemPedido>
                {
                    new ItemPedido { Id = Guid.NewGuid(), Nome = "Hambúrguer", Tipo = TipoItem.Sanduiche, PrecoUnitario = 25.00m, Quantidade = 1 }
                }
            },
            new Pedido
            {
                Id = Guid.NewGuid(),
                DataCriacao = DateTime.UtcNow,
                Status = "Entregue",
                Itens = new List<ItemPedido>
                {
                    new ItemPedido { Id = Guid.NewGuid(), Nome = "Pizza", Tipo = TipoItem.Sanduiche, PrecoUnitario = 30.00m, Quantidade = 1 }
                }
            }
        };

        _pedidoRepositoryMock
            .Setup(x => x.ListarTodosAsync())
            .ReturnsAsync(pedidosEsperados);

        // Act
        var resultado = await _pedidoUseCases.ListarPedidosAsync();

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count());

        _pedidoRepositoryMock.Verify(x => x.ListarTodosAsync(), Times.Once);
    }

    [Fact]
    public async Task DeveRetornarListaVazia_QuandoNaoExistemPedidosDisponiveis()
    {
        // Arrange
        _pedidoRepositoryMock
            .Setup(x => x.ListarTodosAsync())
            .ReturnsAsync(new List<Pedido>());

        // Act
        var resultado = await _pedidoUseCases.ListarPedidosAsync();

        // Assert
        Assert.NotNull(resultado);
        Assert.Empty(resultado);

        _pedidoRepositoryMock.Verify(x => x.ListarTodosAsync(), Times.Once);
    }

    #endregion

    #region Atualizar Pedido

    [Fact]
    public async Task DeveAtualizarPedidoComSucesso_QuandoPedidoExiste()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var hamburguer = new CardapioItem
        {
            Id = 1,
            Nome = "Hambúrguer Premium",
            Preco = 35.00m,
            Tipo = TipoItem.Sanduiche,
            Categoria = "Sanduíches",
            Ativo = true
        };

        var atualizarPedidoDto = new AtualizarPedidoDto
        {
            Id = pedidoId,
            Itens = new List<ItemPedidoDto>
            {
                new ItemPedidoDto { Nome = "Hambúrguer Premium", Quantidade = 2 }
            }
        };

        var pedidoExistente = new Pedido
        {
            Id = pedidoId,
            DataCriacao = DateTime.UtcNow,
            Status = "Pendente",
            Itens = new List<ItemPedido>
            {
                new ItemPedido { Id = Guid.NewGuid(), Nome = "Hambúrguer Clássico", Tipo = TipoItem.Sanduiche, PrecoUnitario = 25.00m, Quantidade = 1 }
            }
        };

        var pedidoAtualizado = new Pedido
        {
            Id = pedidoId,
            DataCriacao = DateTime.UtcNow,
            DataAtualizacao = DateTime.UtcNow,
            Status = "Pendente",
            Itens = new List<ItemPedido>
            {
                new ItemPedido { Id = Guid.NewGuid(), Nome = "Hambúrguer Premium", Tipo = TipoItem.Sanduiche, PrecoUnitario = 35.00m, Quantidade = 2 }
            }
        };

        _pedidoRepositoryMock
            .Setup(x => x.ObterPorIdAsync(pedidoId))
            .ReturnsAsync(pedidoExistente);

        _cardapioRepositoryMock
            .Setup(x => x.ObterPorNomeAsync("Hambúrguer Premium"))
            .ReturnsAsync(hamburguer);

        _pedidoRepositoryMock
            .Setup(x => x.AtualizarAsync(It.IsAny<Pedido>()))
            .ReturnsAsync(pedidoAtualizado);

        // Act
        var resultado = await _pedidoUseCases.AtualizarPedidoAsync(atualizarPedidoDto);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(pedidoId, resultado.Id);
        Assert.NotNull(resultado.DataCriacao);

        _pedidoRepositoryMock.Verify(x => x.ObterPorIdAsync(pedidoId), Times.Once);
        _pedidoRepositoryMock.Verify(x => x.AtualizarAsync(It.IsAny<Pedido>()), Times.Once);
    }

    [Fact]
    public async Task DeveLancarExcecao_QuandoTentarAtualizarPedidoInexistente()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var atualizarPedidoDto = new AtualizarPedidoDto
        {
            Id = pedidoId,
            Itens = new List<ItemPedidoDto>()
        };

        _pedidoRepositoryMock
            .Setup(x => x.ObterPorIdAsync(pedidoId))
            .ReturnsAsync((Pedido)null);

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => 
            _pedidoUseCases.AtualizarPedidoAsync(atualizarPedidoDto));

        _pedidoRepositoryMock.Verify(x => x.ObterPorIdAsync(pedidoId), Times.Once);
        _pedidoRepositoryMock.Verify(x => x.AtualizarAsync(It.IsAny<Pedido>()), Times.Never);
    }

    #endregion

    #region Remover Pedido

    [Fact]
    public async Task DeveRemoverPedidoComSucesso_QuandoPedidoExiste()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();

        _pedidoRepositoryMock
            .Setup(x => x.RemoverAsync(pedidoId))
            .ReturnsAsync(true);

        // Act
        await _pedidoUseCases.RemoverPedidoAsync(pedidoId);

        // Assert
        _pedidoRepositoryMock.Verify(x => x.RemoverAsync(pedidoId), Times.Once);
    }

    [Fact]
    public async Task DeveLancarExcecao_QuandoTentarRemoverPedidoInexistente()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();

        _pedidoRepositoryMock
            .Setup(x => x.RemoverAsync(pedidoId))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => 
            _pedidoUseCases.RemoverPedidoAsync(pedidoId));

        _pedidoRepositoryMock.Verify(x => x.RemoverAsync(pedidoId), Times.Once);
    }

    #endregion
}