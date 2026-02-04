using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.DTOs
{
    public class ProdutoCreateDto
    {
        [Required(ErrorMessage = "O nome do produto é obrigatório")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres")]
        public string Nome { get; set; }

        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O preço é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero")]
        [DataType(DataType.Currency)]
        public decimal Preco { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "A quantidade não pode ser negativa")]
        public int Quantidade { get; set; }
    }
}
