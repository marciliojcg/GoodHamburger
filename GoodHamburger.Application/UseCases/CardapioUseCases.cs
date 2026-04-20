using GoodHamburger.Application.Ports;

namespace GoodHamburger.Application.UseCases;

public class CardapioUseCases
{
    private readonly ICardapioRepository _cardapioRepository;

    public CardapioUseCases(ICardapioRepository cardapioRepository)
    {
        _cardapioRepository = cardapioRepository;
    }

    public async Task<IEnumerable<CardapioItemDto>> ObterCardapioAsync()
    {
        var itens = await _cardapioRepository.ObterTodosAsync();
        return itens.Select(i => new CardapioItemDto
        {
            Nome = i.Nome,
            Preco = i.Preco,
            Tipo = i.Tipo.ToString(),
            Categoria = i.Categoria
        });
    }
}

public class CardapioItemDto
{
    public string Nome { get; set; }
    public decimal Preco { get; set; }
    public string Tipo { get; set; }
    public string Categoria { get; set; }
}