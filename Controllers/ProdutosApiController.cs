using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Extensions;
using WebApplication1.Filters;
using WebApplication1.Helpers;
using WebApplication1.Models.DTOs;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    /// <summary>
    /// API de Produtos - Richardson Maturity Model Level 3 (HATEOAS)
    /// </summary>
    /// <remarks>
    /// SEGURANÇA: Esta API requer HTTPS obrigatório
    /// - Filtro Global: RequireHttpsAttribute
    /// - Filtro Controller: RequireHttpsAttribute
    /// - HSTS: Strict-Transport-Security header
    /// - Autenticação: JWT Bearer Token
    /// - Autorização: Role-based (USER, ADMIN)
    /// </remarks>
    [RequireHttps]
    [RoutePrefix("api/produtos")]
    [LogAction]
    [JwtAuthorizeRoleAttribute("USER", "ADMIN")] // Todos precisam estar logado: USER ou ADMIN
    public class ProdutosApiController : ApiController
    {
        private readonly IProdutoService _produtoService;

        public ProdutosApiController(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        /// <summary>
        /// Obtém lista paginada de produtos com HATEOAS
        /// Acesso: USER ✅ | ADMIN ✅
        /// </summary>
        /// <param name="pageNumber">Número da página (padrão: 1)</param>
        /// <param name="pageSize">Tamanho da página (padrão: 10, máx: 100)</param>
        /// <param name="search">Termo de busca para filtrar por nome ou descrição</param>
        /// <param name="orderBy">Campo de ordenação: nome, nome_desc, preco, preco_desc</param>
        [HttpGet]
        [Route("", Name = "GetAllProdutos")]
        public async Task<IHttpActionResult> GetAll(
            [FromUri] int pageNumber = 1, 
            [FromUri] int pageSize = 10,
            [FromUri] string search = null,
            [FromUri] string orderBy = null)
        {
            try
            {
                var pagedResult = await _produtoService.ObterPaginadoAsync(pageNumber, pageSize, search, orderBy);

                var produtosDto = pagedResult.Items.Select(p => 
                    HateoasHelper.CreateProdutoDtoWithLinks(p, Url)).ToList();

                var response = new PagedResult<ProdutoDto>
                {
                    Items = produtosDto,
                    PageNumber = pagedResult.PageNumber,
                    PageSize = pagedResult.PageSize,
                    TotalRecords = pagedResult.TotalRecords,
                    Links = HateoasHelper.CreatePaginationLinks(
                        Url, 
                        pagedResult.PageNumber, 
                        pagedResult.PageSize, 
                        pagedResult.TotalPages)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtém um produto específico por ID com HATEOAS
        /// Acesso: USER ✅ | ADMIN ✅
        /// </summary>
        [HttpGet]
        [Route("{id:int}", Name = "GetProdutoById")]
        public async Task<IHttpActionResult> GetById(int id)
        {
            try
            {
                var produto = await _produtoService.ObterPorIdAsync(id);
                var produtoDto = HateoasHelper.CreateProdutoDtoWithLinks(produto, Url);
                
                return Ok(produtoDto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Cria um novo produto
        /// Acesso: USER ❌ | ADMIN ✅
        /// </summary>
        [HttpPost]
        [Route("", Name = "CreateProduto")]
        [ValidateModel]
        [JwtAuthorizeRoleAttribute("ADMIN")] // Sobrescreve: apenas ADMIN
        public async Task<IHttpActionResult> Create([FromBody] ProdutoCreateDto produtoDto)
        {
            try
            {
                var produto = produtoDto.ToEntity();
                await _produtoService.CriarAsync(produto);
                
                var produtoDtoResponse = HateoasHelper.CreateProdutoDtoWithLinks(produto, Url);
                var location = Url.Link("GetProdutoById", new { id = produto.Id });
                
                return Created(location, produtoDtoResponse);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Atualiza um produto existente
        /// Acesso: USER ❌ | ADMIN ✅
        /// </summary>
        [HttpPut]
        [Route("{id:int}", Name = "UpdateProduto")]
        [ValidateModel]
        [JwtAuthorizeRoleAttribute("ADMIN")] // Sobrescreve: apenas ADMIN
        public async Task<IHttpActionResult> Update(int id, [FromBody] ProdutoUpdateDto produtoDto)
        {
            try
            {
                var produto = produtoDto.ToEntity();
                produto.Id = id;
                await _produtoService.AtualizarAsync(produto);
                
                var produtoDtoResponse = HateoasHelper.CreateProdutoDtoWithLinks(produto, Url);
                
                return Ok(produtoDtoResponse);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Remove um produto
        /// Acesso: USER ❌ | ADMIN ✅
        /// </summary>
        [HttpDelete]
        [Route("{id:int}", Name = "DeleteProduto")]
        [JwtAuthorizeRoleAttribute("ADMIN")] // Sobrescreve: apenas ADMIN
        public async Task<IHttpActionResult> Delete(int id)
        {
            try
            {
                await _produtoService.ExcluirAsync(id);
                
                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Verifica se um produto existe (HEAD)
        /// Acesso: USER ✅ | ADMIN ✅
        /// </summary>
        [HttpHead]
        [Route("{id:int}", Name = "HeadProduto")]
        public async Task<IHttpActionResult> Head(int id)
        {
            try
            {
                var existe = await _produtoService.ExisteAsync(id);
                return existe ? Ok() : (IHttpActionResult)NotFound();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Retorna os métodos HTTP permitidos (OPTIONS)
        /// Acesso: USER ✅ | ADMIN ✅
        /// </summary>
        [HttpOptions]
        [Route("")]
        [Route("{id:int}")]
        public IHttpActionResult Options()
        {
            var response = new
            {
                Methods = new[] { "GET", "POST", "PUT", "DELETE", "HEAD", "OPTIONS" },
                Description = "API de Produtos - Nível 3 Richardson Maturity Model",
                Version = "1.0.0",
                Security = new
                {
                    Protocol = "HTTPS Required",
                    HSTS = "Enabled (365 days)",
                    TLS = "1.2+"
                }
            };

            return Ok(response);
        }
    }
}
