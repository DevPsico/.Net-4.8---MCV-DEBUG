using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Models.DTOs;
using WebApplication1.Repositories;

namespace WebApplication1.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly IProdutoRepository _produtoRepository;

        public ProdutoService(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        public async Task<IEnumerable<Produto>> ObterTodosAsync()
        {
            return await _produtoRepository.GetAllAsync();
        }

        public async Task<PagedResult<Produto>> ObterPaginadoAsync(int pageNumber, int pageSize, string search = null, string orderBy = null)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            if (pageSize < 1 || pageSize > 100)
                pageSize = 10;

            var produtos = await _produtoRepository.GetPagedAsync(pageNumber, pageSize, search, orderBy);
            var totalRecords = await _produtoRepository.GetTotalCountAsync(search);

            return new PagedResult<Produto>
            {
                Items = produtos,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
        }

        public async Task<Produto> ObterPorIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id inválido", nameof(id));

            var produto = await _produtoRepository.GetByIdAsync(id);
            if (produto == null)
                throw new KeyNotFoundException($"Produto com Id {id} não encontrado");

            return produto;
        }

        public async Task CriarAsync(Produto produto)
        {
            if (produto == null)
                throw new ArgumentNullException(nameof(produto));

            ValidarProduto(produto);
            await _produtoRepository.AddAsync(produto);
        }

        public async Task AtualizarAsync(Produto produto)
        {
            if (produto == null)
                throw new ArgumentNullException(nameof(produto));

            if (!await _produtoRepository.ExistsAsync(produto.Id))
                throw new KeyNotFoundException($"Produto com Id {produto.Id} não encontrado");

            ValidarProduto(produto);
            await _produtoRepository.UpdateAsync(produto);
        }

        public async Task ExcluirAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id inválido", nameof(id));

            if (!await _produtoRepository.ExistsAsync(id))
                throw new KeyNotFoundException($"Produto com Id {id} não encontrado");

            await _produtoRepository.DeleteAsync(id);
        }

        public async Task<bool> ExisteAsync(int id)
        {
            return await _produtoRepository.ExistsAsync(id);
        }

        private void ValidarProduto(Produto produto)
        {
            if (string.IsNullOrWhiteSpace(produto.Nome))
                throw new ArgumentException("Nome do produto é obrigatório");

            if (produto.Preco <= 0)
                throw new ArgumentException("Preço deve ser maior que zero");

            if (produto.Quantidade < 0)
                throw new ArgumentException("Quantidade não pode ser negativa");
        }
    }
}
