using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Domain.Entities;

public class CardapioItem
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public decimal Preco { get; set; }
    public TipoItem Tipo { get; set; }
    public string Categoria { get; set; }
    public bool Ativo { get; set; }
}