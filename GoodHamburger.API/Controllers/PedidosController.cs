using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.UseCases;
using GoodHamburger.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly PedidoUseCases _pedidoUseCases;

    public PedidosController(PedidoUseCases pedidoUseCases)
    {
        _pedidoUseCases = pedidoUseCases;
    }

    [HttpPost]
    public async Task<IActionResult> CriarPedido([FromBody] CriarPedidoDto dto)
    {
        try
        {
            var pedido = await _pedidoUseCases.CriarPedidoAsync(dto);
            return CreatedAtAction(nameof(ObterPedido), new { id = pedido.Id }, pedido);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
       
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPedido(Guid id)
    {
        try
        {
            var pedido = await _pedidoUseCases.ObterPedidoAsync(id);
            return Ok(pedido);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> ListarPedidos()
    {
        try
        {
            var pedidos = await _pedidoUseCases.ListarPedidosAsync();
            return Ok(pedidos);

        }
        catch (NotFoundException ex)
        {

            return NotFound(new { erro = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }


    }

    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarPedido(Guid id, [FromBody] AtualizarPedidoDto dto)
    {
        try
        {
            dto.Id = id;
            var pedido = await _pedidoUseCases.AtualizarPedidoAsync(dto);
            return Ok(pedido);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoverPedido(Guid id)
    {
        try
        {
            await _pedidoUseCases.RemoverPedidoAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }
}