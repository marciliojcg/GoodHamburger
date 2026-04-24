using GoodHamburger.Blazor.Services.Interfaces;
using Microsoft.JSInterop;

namespace GoodHamburger.Blazor.Services;



public class PedidoValidationService : IPedidoValidationService
{
    private readonly IJSRuntime _js;

    public PedidoValidationService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<bool> ValidarPedidoAsync(
        string sanduicheSelecionado,
        int quantidadeSanduiche,
        string acompanhamento1Selecionado,
        int quantidadeAcompanhamento1,
        string acompanhamento2Selecionado,
        int quantidadeAcompanhamento2)
    {
        if (!string.IsNullOrEmpty(sanduicheSelecionado) && quantidadeSanduiche != 1)
        {
            await _js.InvokeVoidAsync("alert", "Selecione um sanduíche com uma quantidade.");
            return false;
        }

        if (!string.IsNullOrEmpty(acompanhamento1Selecionado) && quantidadeAcompanhamento1 > 1)
        {
            await _js.InvokeVoidAsync("alert", 
                $"Selecione apenas uma quantidade de acompanhamento - {acompanhamento1Selecionado}.");
            return false;
        }

        if (!string.IsNullOrEmpty(acompanhamento2Selecionado) && quantidadeAcompanhamento2 > 1)
        {
            await _js.InvokeVoidAsync("alert", 
                $"Selecione apenas uma quantidade acompanhamento - {acompanhamento2Selecionado}.");
            return false;
        }

        if (!string.IsNullOrEmpty(acompanhamento1Selecionado) && !string.IsNullOrEmpty(acompanhamento2Selecionado))
        {
            if (acompanhamento1Selecionado == acompanhamento2Selecionado)
            {
                await _js.InvokeVoidAsync("alert", "Selecione acompanhamentos diferentes.");
                return false;
            }
        }

        return true;
    }
}