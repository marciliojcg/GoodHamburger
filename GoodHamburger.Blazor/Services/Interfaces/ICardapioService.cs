using GoodHamburger.Blazor.Models;

namespace GoodHamburger.Blazor.Services.Interfaces;

public interface ICardapioService
{
    Task<List<CardapioItemResponse>> GetCardapioAsync();
    Task<List<CardapioItemResponse>> GetSanduichesAsync();
    Task<List<CardapioItemResponse>> GetAcompanhamentosAsync();
}
