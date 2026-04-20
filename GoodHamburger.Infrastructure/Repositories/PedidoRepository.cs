using Dapper;
using GoodHamburger.Application.Ports;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Infrastructure.Data;

namespace GoodHamburger.Infrastructure.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly DapperContext _context;

    public PedidoRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<Pedido> CriarAsync(Pedido pedido)
    {
        using var connection = _context.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            // Inserir pedido
            var sqlPedido = @"
                INSERT INTO Pedidos (Id, DataCriacao, DataAtualizacao, Status, Subtotal, Desconto, Total)
                VALUES (@Id, @DataCriacao, @DataAtualizacao, @Status, @Subtotal, @Desconto, @Total)";

            await connection.ExecuteAsync(sqlPedido, new
            {
                pedido.Id,
                pedido.DataCriacao,
                pedido.DataAtualizacao,
                pedido.Status,
                pedido.Subtotal,
                pedido.Desconto,
                pedido.Total
            }, transaction);

            // Inserir itens
            foreach (var item in pedido.Itens)
            {
                var sqlItem = @"
                    INSERT INTO ItensPedido (Id, PedidoId, Nome, Tipo, PrecoUnitario, Quantidade)
                    VALUES (@Id, @PedidoId, @Nome, @Tipo, @PrecoUnitario, @Quantidade)";

                await connection.ExecuteAsync(sqlItem, new
                {
                    item.Id,
                    PedidoId = pedido.Id,
                    item.Nome,
                    item.Tipo,
                    item.PrecoUnitario,
                    item.Quantidade
                }, transaction);
            }

            transaction.Commit();
            return pedido;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<Pedido> ObterPorIdAsync(Guid id)
    {
        using var connection = _context.CreateConnection();

        var sqlPedido = "SELECT * FROM Pedidos WHERE Id = @Id";
        var pedido = await connection.QueryFirstOrDefaultAsync<Pedido>(sqlPedido, new { Id = id });

        if (pedido != null)
        {
            var sqlItens = "SELECT * FROM ItensPedido WHERE PedidoId = @PedidoId";
            pedido.Itens = (await connection.QueryAsync<ItemPedido>(sqlItens, new { PedidoId = id })).ToList();
        }

        return pedido;
    }

    public async Task<IEnumerable<Pedido>> ListarTodosAsync()
    {
        using var connection = _context.CreateConnection();

        var sqlPedidos = "SELECT * FROM Pedidos ORDER BY DataCriacao DESC";
        var pedidos = await connection.QueryAsync<Pedido>(sqlPedidos);

        foreach (var pedido in pedidos)
        {
            var sqlItens = "SELECT * FROM ItensPedido WHERE PedidoId = @PedidoId";
            pedido.Itens = (await connection.QueryAsync<ItemPedido>(sqlItens, new { PedidoId = pedido.Id })).ToList();
        }

        return pedidos;
    }

    public async Task<Pedido> AtualizarAsync(Pedido pedido)
    {
        using var connection = _context.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            // Atualizar pedido
            var sqlPedido = @"
                UPDATE Pedidos 
                SET DataAtualizacao = @DataAtualizacao, Status = @Status, 
                    Subtotal = @Subtotal, Desconto = @Desconto, Total = @Total
                WHERE Id = @Id";

            await connection.ExecuteAsync(sqlPedido, new
            {
                pedido.Id,
                pedido.DataAtualizacao,
                pedido.Status,
                pedido.Subtotal,
                pedido.Desconto,
                pedido.Total
            }, transaction);

            // Remover itens antigos
            await connection.ExecuteAsync("DELETE FROM ItensPedido WHERE PedidoId = @PedidoId",
                new { PedidoId = pedido.Id }, transaction);

            // Inserir novos itens
            foreach (var item in pedido.Itens)
            {
                var sqlItem = @"
                    INSERT INTO ItensPedido (Id, PedidoId, Nome, Tipo, PrecoUnitario, Quantidade)
                    VALUES (@Id, @PedidoId, @Nome, @Tipo, @PrecoUnitario, @Quantidade)";

                await connection.ExecuteAsync(sqlItem, new
                {
                    item.Id,
                    PedidoId = pedido.Id,
                    item.Nome,
                    item.Tipo,
                    item.PrecoUnitario,
                    item.Quantidade
                }, transaction);
            }

            transaction.Commit();
            return pedido;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<bool> RemoverAsync(Guid id)
    {
        using var connection = _context.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            await connection.ExecuteAsync("DELETE FROM ItensPedido WHERE PedidoId = @PedidoId",
                new { PedidoId = id }, transaction);

            var rowsAffected = await connection.ExecuteAsync("DELETE FROM Pedidos WHERE Id = @Id",
                new { Id = id }, transaction);

            transaction.Commit();
            return rowsAffected > 0;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}