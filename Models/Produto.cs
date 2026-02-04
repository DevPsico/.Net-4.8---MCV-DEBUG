using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
        public int Quantidade { get; set; }
    }
}
