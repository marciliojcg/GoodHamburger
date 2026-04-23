using GoodHamburger.Blazor.Models;
using Microsoft.JSInterop;

namespace GoodHamburger.Blazor.Components.Pages;

public partial class PedidoLista
{

    private List<PedidoResponse>? pedidos = null;

    protected override async Task OnInitializedAsync()
    {
        await CarregarPedidos();
    }

    private async Task CarregarPedidos()
    {
        try
        {
            pedidos = await PedidoService.GetPedidosAsync();
            StateHasChanged();
        }
        catch (Exception ex)
        {
            await JS.InvokeVoidAsync("console.error", $"Erro ao carregar pedidos: {ex.Message}");
        }
    }

    private void EditarPedido(Guid id)
    {
        Navigation.NavigateTo($"/pedido/editar/{id}");
    }

    private async Task ExcluirPedido(Guid id)
    {
        var confirm = await JS.InvokeAsync<bool>("confirm", $"Deseja realmente excluir o pedido {id.ToString().Substring(0, 8)}?");

        if (confirm)
        {
            try
            {
                var success = await PedidoService.DeletePedidoAsync(id);
                if (success)
                {
                    await CarregarPedidos();
                }
                else
                {
                    await JS.InvokeVoidAsync("alert", "Erro ao excluir o pedido");
                }
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("alert", $"Erro ao excluir pedido: {ex.Message}");
            }
        }
    }

    private string GetStatusColor(string status)
    {
        return status switch
        {
            "Pendente" => "warning",
            "Confirmado" => "info",
            "Entregue" => "success",
            "Cancelado" => "danger",
            _ => "secondary"
        };
    }
}