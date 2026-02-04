using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Repositories
{
    public interface IProdutoRepository
    {
        Task<IEnumerable<Produto>> GetAllAsync();
        Task<IEnumerable<Produto>> GetPagedAsync(int pageNumber, int pageSize, string search = null, string orderBy = null);
        Task<int> GetTotalCountAsync(string search = null);
        Task<Produto> GetByIdAsync(int id);
        Task AddAsync(Produto produto);
        Task UpdateAsync(Produto produto);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}