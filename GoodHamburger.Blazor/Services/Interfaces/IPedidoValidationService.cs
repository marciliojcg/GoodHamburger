namespace GoodHamburger.Blazor.Services.Interfaces;

public interface IPedidoValidationService
{
    Task<bool> ValidarPedidoAsync(
        string sanduicheSelecionado,
        int quantidadeSanduiche,
        string acompanhamento1Selecionado,
        int quantidadeAcompanhamento1,
        string acompanhamento2Selecionado,
        int quantidadeAcompanhamento2);
}
