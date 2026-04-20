using Dapper;
using GoodHamburger.Application.Ports;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Infrastructure.Data;

namespace GoodHamburger.Infrastructure.Repositories;

public class CardapioRepository : ICardapioRepository
{
    private readonly DapperContext _context;

    public CardapioRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CardapioItem>> ObterTodosAsync()
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<CardapioItem>(
            "SELECT * FROM Cardapio WHERE Ativo = 1 ORDER BY Tipo, Nome");
    }

    public async Task<CardapioItem> ObterPorNomeAsync(string nome)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<CardapioItem>(
            "SELECT * FROM Cardapio WHERE Nome = @Nome AND Ativo = 1",
            new { Nome = nome });
    }
}