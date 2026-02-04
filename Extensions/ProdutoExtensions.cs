using AutoMapper;
using WebApplication1.App_Start;
using WebApplication1.Models;
using WebApplication1.Models.DTOs;

namespace WebApplication1.Extensions
{
    public static class ProdutoExtensions
    {
        private static IMapper Mapper => MapperInstance.Mapper;

        public static Produto ToEntity(this ProdutoCreateDto dto)
        {
            return Mapper.Map<Produto>(dto);
        }

        public static Produto ToEntity(this ProdutoUpdateDto dto)
        {
            return Mapper.Map<Produto>(dto);
        }

        public static ProdutoDto ToDto(this Produto entity)
        {
            return Mapper.Map<ProdutoDto>(entity);
        }
    }
}
