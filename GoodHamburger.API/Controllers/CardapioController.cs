using GoodHamburger.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CardapioController : ControllerBase
{
    private readonly CardapioUseCases _cardapioUseCases;

    public CardapioController(CardapioUseCases cardapioUseCases)
    {
        _cardapioUseCases = cardapioUseCases;
    }

    [HttpGet]
    public async Task<IActionResult> ObterCardapio()
    {
        try
        {
            var cardapio = await _cardapioUseCases.ObterCardapioAsync();
            return Ok(cardapio);
        }
        catch (Exception ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    
    }
}