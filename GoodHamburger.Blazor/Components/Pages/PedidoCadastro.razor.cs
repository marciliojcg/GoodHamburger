using GoodHamburger.Blazor.Models;
using GoodHamburger.Blazor.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace GoodHamburger.Blazor.Components.Pages;

public partial class PedidoCadastro
{
    [Inject]
    private IPedidoValidationService PedidoValidationService { get; set; } = default!;

    private bool carregando = true;
    private List<CardapioItemResponse> sanduiches = new();
    private List<CardapioItemResponse> acompanhamentos = new();

    private string sanduicheSelecionado = "";
    private int quantidadeSanduiche = 1;

    private string acompanhamento1Selecionado = "";
    private int quantidadeAcompanhamento1 = 0;

    private string acompanhamento2Selecionado = "";
    private int quantidadeAcompanhamento2 = 0;

    private List<ItemPedidoRequest> itensPedido = new();
    private decimal subtotal = 0;
    private decimal desconto = 0;
    private decimal total = 0;

    protected override async Task OnInitializedAsync()
    {
        await CarregarCardapio();
        carregando = false;
    }

    private async Task CarregarCardapio()
    {
        try
        {
            sanduiches = await CardapioService.GetSanduichesAsync();
            acompanhamentos = await CardapioService.GetAcompanhamentosAsync();
        }
        catch (Exception ex)
        {
            await JS.InvokeVoidAsync("console.error", $"Erro ao carregar cardápio: {ex.Message}");
        }
    }

    private void CalcularResumo()
    {
        itensPedido.Clear();

        // Adicionar sanduíche
        if (!string.IsNullOrEmpty(sanduicheSelecionado) && quantidadeSanduiche > 0)
        {
            var sanduiche = sanduiches.FirstOrDefault(s => s.Nome == sanduicheSelecionado);
            if (sanduiche != null)
            {
                itensPedido.Add(new ItemPedidoRequest
                {
                    Nome = sanduiche.Nome,
                    Quantidade = quantidadeSanduiche
                });
            }
        }

        // Adicionar acompanhamento 1
        if (!string.IsNullOrEmpty(acompanhamento1Selecionado) && quantidadeAcompanhamento1 > 0)
        {
            itensPedido.Add(new ItemPedidoRequest
            {
                Nome = acompanhamento1Selecionado,
                Quantidade = quantidadeAcompanhamento1
            });
        }

        // Adicionar acompanhamento 2
        if (!string.IsNullOrEmpty(acompanhamento2Selecionado) && quantidadeAcompanhamento2 > 0)
        {
            itensPedido.Add(new ItemPedidoRequest
            {
                Nome = acompanhamento2Selecionado,
                Quantidade = quantidadeAcompanhamento2
            });
        }

        // Calcular totais
        subtotal = 0;
        foreach (var item in itensPedido)
        {
            var cardapioItem = sanduiches.FirstOrDefault(s => s.Nome == item.Nome) ??
                              acompanhamentos.FirstOrDefault(a => a.Nome == item.Nome);
            if (cardapioItem != null)
            {
                subtotal += cardapioItem.Preco * item.Quantidade;
            }
        }

        // Calcular desconto
        var temSanduiche = itensPedido.Any(i => sanduiches.Any(s => s.Nome == i.Nome));
        var temBatata = itensPedido.Any(i => i.Nome == "Batata frita");
        var temRefrigerante = itensPedido.Any(i => i.Nome == "Refrigerante");

        if (temSanduiche && temBatata && temRefrigerante)
            desconto = subtotal * 0.20m;
        else if (temSanduiche && temRefrigerante)
            desconto = subtotal * 0.15m;
        else if (temSanduiche && temBatata)
            desconto = subtotal * 0.10m;
        else
            desconto = 0;

        total = subtotal - desconto;
    }

    private bool PedidoValido()
    {
        return !string.IsNullOrEmpty(sanduicheSelecionado) && quantidadeSanduiche > 0;
    }

    private async Task SalvarPedido()
    {   
        if (!await PedidoValidationService.ValidarPedidoAsync(
            sanduicheSelecionado,
            quantidadeSanduiche,
            acompanhamento1Selecionado,
            quantidadeAcompanhamento1,
            acompanhamento2Selecionado,
            quantidadeAcompanhamento2))
            return;

        CalcularResumo();

        var request = new CriarPedidoRequest
        {
            Itens = itensPedido.Where(i => i.Quantidade > 0).ToList()
        };

        try
        {
            await JS.InvokeVoidAsync("console.log", $"Itens do pedido: {System.Text.Json.JsonSerializer.Serialize(request.Itens)}");

            var pedido = await PedidoService.CreatePedidoAsync(request);
            if (pedido != null && pedido.Id != Guid.Empty)
            {
                await JS.InvokeVoidAsync("alert", "Pedido criado com sucesso!");
                Navigation.NavigateTo("/pedidos");
            }
        }
        catch (Exception ex)
        {
            await JS.InvokeVoidAsync("alert", $"Erro ao criar pedido: {ex.Message}");
        }
    }

    private void Cancelar()
    {
        Navigation.NavigateTo("/pedidos");
    }
}