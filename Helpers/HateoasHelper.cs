using System.Collections.Generic;
using System.Web.Http.Routing;
using WebApplication1.Models;
using WebApplication1.Models.DTOs;

namespace WebApplication1.Helpers
{
    public static class HateoasHelper
    {
        public static ProdutoDto CreateProdutoDtoWithLinks(Produto produto, UrlHelper urlHelper)
        {
            var dto = new ProdutoDto
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                Preco = produto.Preco,
                Quantidade = produto.Quantidade,
                Links = new List<LinkDto>
                {
                    new LinkDto
                    {
                        Href = urlHelper.Link("GetProdutoById", new { id = produto.Id }),
                        Rel = "self",
                        Method = "GET"
                    },
                    new LinkDto
                    {
                        Href = urlHelper.Link("UpdateProduto", new { id = produto.Id }),
                        Rel = "update",
                        Method = "PUT"
                    },
                    new LinkDto
                    {
                        Href = urlHelper.Link("DeleteProduto", new { id = produto.Id }),
                        Rel = "delete",
                        Method = "DELETE"
                    },
                    new LinkDto
                    {
                        Href = urlHelper.Link("GetAllProdutos", new { }),
                        Rel = "collection",
                        Method = "GET"
                    }
                }
            };

            return dto;
        }

        public static List<LinkDto> CreateCollectionLinks(UrlHelper urlHelper)
        {
            return new List<LinkDto>
            {
                new LinkDto
                {
                    Href = urlHelper.Link("GetAllProdutos", new { }),
                    Rel = "self",
                    Method = "GET"
                },
                new LinkDto
                {
                    Href = urlHelper.Link("CreateProduto", new { }),
                    Rel = "create",
                    Method = "POST"
                }
            };
        }

        public static List<LinkDto> CreatePaginationLinks(
            UrlHelper urlHelper, 
            int pageNumber, 
            int pageSize, 
            int totalPages,
            string routeName = "GetAllProdutos")
        {
            var links = new List<LinkDto>
            {
                new LinkDto
                {
                    Href = urlHelper.Link(routeName, new { pageNumber, pageSize }),
                    Rel = "self",
                    Method = "GET"
                }
            };

            if (pageNumber > 1)
            {
                links.Add(new LinkDto
                {
                    Href = urlHelper.Link(routeName, new { pageNumber = 1, pageSize }),
                    Rel = "first",
                    Method = "GET"
                });

                links.Add(new LinkDto
                {
                    Href = urlHelper.Link(routeName, new { pageNumber = pageNumber - 1, pageSize }),
                    Rel = "previous",
                    Method = "GET"
                });
            }

            if (pageNumber < totalPages)
            {
                links.Add(new LinkDto
                {
                    Href = urlHelper.Link(routeName, new { pageNumber = pageNumber + 1, pageSize }),
                    Rel = "next",
                    Method = "GET"
                });

                links.Add(new LinkDto
                {
                    Href = urlHelper.Link(routeName, new { pageNumber = totalPages, pageSize }),
                    Rel = "last",
                    Method = "GET"
                });
            }

            return links;
        }
    }
}
