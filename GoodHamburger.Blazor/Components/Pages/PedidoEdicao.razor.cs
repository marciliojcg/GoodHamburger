using GoodHamburger.Blazor.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace GoodHamburger.Blazor.Components.Pages;

public partial class PedidoEdicao
{
    [Parameter]
    public Guid id { get; set; }

    private bool carregando = true;
    private PedidoResponse? pedido;
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
        await CarregarPedido();
        CarregarDadosPedido();
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

    private async Task CarregarPedido()
    {
        try
        {
            pedido = await PedidoService.GetPedidoByIdAsync(id);
        }
        catch (Exception ex)
        {
            await JS.InvokeVoidAsync("console.error", $"Erro ao carregar pedido: {ex.Message}");
        }
    }

    private void CarregarDadosPedido()
    {
        if (pedido == null) return;

        var itemSanduiche = pedido.Itens.FirstOrDefault(i => sanduiches.Any(s => s.Nome == i.Nome));
        if (itemSanduiche != null)
        {
            sanduicheSelecionado = itemSanduiche.Nome;
            quantidadeSanduiche = itemSanduiche.Quantidade;
        }

        var acompanhamentosPedido = pedido.Itens.Where(i => acompanhamentos.Any(a => a.Nome == i.Nome)).ToList();

        if (acompanhamentosPedido.Count > 0)
        {
            acompanhamento1Selecionado = acompanhamentosPedido[0].Nome;
            quantidadeAcompanhamento1 = acompanhamentosPedido[0].Quantidade;
        }

        if (acompanhamentosPedido.Count > 1)
        {
            acompanhamento2Selecionado = acompanhamentosPedido[1].Nome;
            quantidadeAcompanhamento2 = acompanhamentosPedido[1].Quantidade;
        }

        CalcularResumo();
    }

    protected override void OnParametersSet()
    {
        if (!carregando)
            CalcularResumo();
    }

    private void AoMudarSanduiche(ChangeEventArgs e)
    {
        sanduicheSelecionado = e.Value?.ToString() ?? "";
        CalcularResumo();
    }

    private void AoMudarAcompanhamento1(ChangeEventArgs e)
    {
        acompanhamento1Selecionado = e.Value?.ToString() ?? "";
        CalcularResumo();
    }

    private void AoMudarAcompanhamento2(ChangeEventArgs e)
    {
        acompanhamento2Selecionado = e.Value?.ToString() ?? "";
        CalcularResumo();
    }

    private void AoMudarQuantidade(ChangeEventArgs e)
    {
        CalcularResumo();
    }

    private void CalcularResumo()
    {
        itensPedido.Clear();

        if (!string.IsNullOrEmpty(sanduicheSelecionado) && quantidadeSanduiche > 0)
        {
            itensPedido.Add(new ItemPedidoRequest
            {
                Nome = sanduicheSelecionado,
                Quantidade = quantidadeSanduiche
            });
        }

        if (!string.IsNullOrEmpty(acompanhamento1Selecionado) && quantidadeAcompanhamento1 > 0)
        {
            itensPedido.Add(new ItemPedidoRequest
            {
                Nome = acompanhamento1Selecionado,
                Quantidade = quantidadeAcompanhamento1
            });
        }

        if (!string.IsNullOrEmpty(acompanhamento2Selecionado) && quantidadeAcompanhamento2 > 0)
        {
            itensPedido.Add(new ItemPedidoRequest
            {
                Nome = acompanhamento2Selecionado,
                Quantidade = quantidadeAcompanhamento2
            });
        }

        subtotal = 0;
        foreach (var item in itensPedido)
        {
            subtotal += CalcularSubtotalItem(item);
        }

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

    private decimal CalcularSubtotalItem(ItemPedidoRequest item)
    {
        var sanduiche = sanduiches.FirstOrDefault(s => s.Nome == item.Nome);
        if (sanduiche != null)
            return sanduiche.Preco * item.Quantidade;

        var acompanhamento = acompanhamentos.FirstOrDefault(a => a.Nome == item.Nome);
        if (acompanhamento != null)
            return acompanhamento.Preco * item.Quantidade;

        return 0;
    }

    private bool PedidoValido()
    {
        return !string.IsNullOrEmpty(sanduicheSelecionado) && quantidadeSanduiche > 0;
    }

    private async Task AtualizarPedido()
    {
        CalcularResumo();

        var request = new AtualizarPedidoRequest
        {
            Id = id,
            Itens = itensPedido.Where(i => i.Quantidade > 0).ToList()
        };

        try
        {
            var pedidoAtualizado = await PedidoService.UpdatePedidoAsync(request);
            if (pedidoAtualizado != null)
            {
                await JS.InvokeVoidAsync("alert", "Pedido atualizado com sucesso!");
                Navigation.NavigateTo("/pedidos");
            }
        }
        catch (Exception ex)
        {
            await JS.InvokeVoidAsync("alert", $"Erro ao atualizar pedido: {ex.Message}");
        }
    }

    private void Cancelar()
    {
        Navigation.NavigateTo("/pedidos");
    }
}