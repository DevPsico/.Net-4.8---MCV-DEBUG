using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Models.DTOs;

namespace WebApplication1.Services
{
    public interface IProdutoService
    {
        Task<IEnumerable<Produto>> ObterTodosAsync();
        Task<PagedResult<Produto>> ObterPaginadoAsync(int pageNumber, int pageSize, string search = null, string orderBy = null);
        Task<Produto> ObterPorIdAsync(int id);
        Task CriarAsync(Produto produto);
        Task AtualizarAsync(Produto produto);
        Task ExcluirAsync(int id);
        Task<bool> ExisteAsync(int id);
    }
}
