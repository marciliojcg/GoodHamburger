using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Ports;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Exceptions;

namespace GoodHamburger.Application.UseCases;

public class PedidoUseCases
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly ICardapioRepository _cardapioRepository;

    public PedidoUseCases(IPedidoRepository pedidoRepository, ICardapioRepository cardapioRepository)
    {
        _pedidoRepository = pedidoRepository;
        _cardapioRepository = cardapioRepository;
    }

    public async Task<PedidoResponseDto> CriarPedidoAsync(CriarPedidoDto dto)
    {
        var itensPedido = await ConstruirItensPedido(dto.Itens);

        var pedido = new Pedido
        {
            Id = Guid.NewGuid(),
            DataCriacao = DateTime.UtcNow,
            Status = "Pendente",
            Itens = itensPedido
        };

        pedido.ValidarItensDuplicados();
        pedido.CalcularTotais();

        var teste = pedido;

        var pedidoCriado = await _pedidoRepository.CriarAsync(pedido);
        return MapToResponse(pedidoCriado);
    }

    public async Task<PedidoResponseDto> ObterPedidoAsync(Guid id)
    {
        var pedido = await _pedidoRepository.ObterPorIdAsync(id);
        if (pedido == null)
            throw new DomainException($"Pedido {id} não encontrado");

        return MapToResponse(pedido);
    }

    public async Task<IEnumerable<PedidoResponseDto>> ListarPedidosAsync()
    {
        var pedidos = await _pedidoRepository.ListarTodosAsync();
        return pedidos.Select(MapToResponse);
    }

    public async Task<PedidoResponseDto> AtualizarPedidoAsync(AtualizarPedidoDto dto)
    {
        var pedidoExistente = await _pedidoRepository.ObterPorIdAsync(dto.Id);
        if (pedidoExistente == null)
            throw new DomainException($"Pedido {dto.Id} não encontrado");

        var itensPedido = await ConstruirItensPedido(dto.Itens);

        pedidoExistente.Itens = itensPedido;
        pedidoExistente.DataAtualizacao = DateTime.UtcNow;
        pedidoExistente.ValidarItensDuplicados();
        pedidoExistente.CalcularTotais();

        var pedidoAtualizado = await _pedidoRepository.AtualizarAsync(pedidoExistente);
        return MapToResponse(pedidoAtualizado);
    }

    public async Task RemoverPedidoAsync(Guid id)
    {
        var existe = await _pedidoRepository.RemoverAsync(id);
        if (!existe)
            throw new DomainException($"Pedido {id} não encontrado");
    }

    private async Task<List<ItemPedido>> ConstruirItensPedido(List<ItemPedidoDto> itensDto)
    {
        var itensPedido = new List<ItemPedido>();

        foreach (var itemDto in itensDto)
        {
            var cardapioItem = await _cardapioRepository.ObterPorNomeAsync(itemDto.Nome);
            if (cardapioItem == null)
                throw new DomainException($"Item {itemDto.Nome} não encontrado no cardápio");

            itensPedido.Add(new ItemPedido
            {
                Id = Guid.NewGuid(),
                Nome = cardapioItem.Nome,
                Tipo = cardapioItem.Tipo,
                PrecoUnitario = cardapioItem.Preco,
                Quantidade = itemDto.Quantidade
            });
        }

        return itensPedido;
    }

    private PedidoResponseDto MapToResponse(Pedido pedido)
    {
        return new PedidoResponseDto
        {
            Id = pedido.Id,
            DataCriacao = pedido.DataCriacao,
            Status = pedido.Status,
            Subtotal = pedido.Subtotal,
            Desconto = pedido.Desconto,
            Total = pedido.Total,
            Itens = pedido.Itens.Select(i => new ItemPedidoResponseDto
            {
                Nome = i.Nome,
                PrecoUnitario = i.PrecoUnitario,
                Quantidade = i.Quantidade,
                Subtotal = i.PrecoUnitario * i.Quantidade
            }).ToList()
        };
    }
}