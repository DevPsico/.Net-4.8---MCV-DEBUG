using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private static List<Produto> _produtos = new List<Produto>
        {
            new Produto { Id = 1, Nome = "Notebook", Descricao = "Notebook Dell Inspiron", Preco = 3500.00m, Quantidade = 10 },
            new Produto { Id = 2, Nome = "Mouse", Descricao = "Mouse Logitech MX Master", Preco = 350.00m, Quantidade = 50 },
            new Produto { Id = 3, Nome = "Teclado", Descricao = "Teclado Mecânico Redragon", Preco = 250.00m, Quantidade = 30 }
        };
        private static int _nextId = 4;

        public Task<IEnumerable<Produto>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<Produto>>(_produtos);
        }

        public Task<IEnumerable<Produto>> GetPagedAsync(int pageNumber, int pageSize, string search = null, string orderBy = null)
        {
            var query = _produtos.AsQueryable();

            // Filtro de busca
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(p => 
                    p.Nome.ToLower().Contains(search) || 
                    (p.Descricao != null && p.Descricao.ToLower().Contains(search)));
            }

            // Ordenação (compatível com C# 7.3)
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                var orderByLower = orderBy.ToLower();
                if (orderByLower == "nome")
                    query = query.OrderBy(p => p.Nome);
                else if (orderByLower == "nome_desc")
                    query = query.OrderByDescending(p => p.Nome);
                else if (orderByLower == "preco")
                    query = query.OrderBy(p => p.Preco);
                else if (orderByLower == "preco_desc")
                    query = query.OrderByDescending(p => p.Preco);
                else
                    query = query.OrderBy(p => p.Id);
            }
            else
            {
                query = query.OrderBy(p => p.Id);
            }

            // Paginação
            var result = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Task.FromResult<IEnumerable<Produto>>(result);
        }

        public Task<int> GetTotalCountAsync(string search = null)
        {
            var query = _produtos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(p => 
                    p.Nome.ToLower().Contains(search) || 
                    (p.Descricao != null && p.Descricao.ToLower().Contains(search)));
            }

            return Task.FromResult(query.Count());
        }

        public Task<Produto> GetByIdAsync(int id)
        {
            var produto = _produtos.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(produto);
        }

        public Task AddAsync(Produto produto)
        {
            produto.Id = _nextId++;
            _produtos.Add(produto);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Produto produto)
        {
            var existingProduto = _produtos.FirstOrDefault(p => p.Id == produto.Id);
            if (existingProduto != null)
            {
                existingProduto.Nome = produto.Nome;
                existingProduto.Descricao = produto.Descricao;
                existingProduto.Preco = produto.Preco;
                existingProduto.Quantidade = produto.Quantidade;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            var produto = _produtos.FirstOrDefault(p => p.Id == id);
            if (produto != null)
            {
                _produtos.Remove(produto);
            }
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(int id)
        {
            var exists = _produtos.Any(p => p.Id == id);
            return Task.FromResult(exists);
        }
    }
}
