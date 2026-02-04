using System.Collections.Generic;

namespace WebApplication1.Models.DTOs
{
    public class ProdutoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
        public int Quantidade { get; set; }
        public List<LinkDto> Links { get; set; } = new List<LinkDto>();
    }

    public class LinkDto
    {
        public string Href { get; set; }
        public string Rel { get; set; }
        public string Method { get; set; }
    }
}
