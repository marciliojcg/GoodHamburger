using GoodHamburger.Domain.Entities;

namespace GoodHamburger.Application.Ports;

public interface ICardapioRepository
{
    Task<IEnumerable<CardapioItem>> ObterTodosAsync();
    Task<CardapioItem> ObterPorNomeAsync(string nome);
}