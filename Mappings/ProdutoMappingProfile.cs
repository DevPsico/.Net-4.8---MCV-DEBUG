using AutoMapper;
using WebApplication1.Models;
using WebApplication1.Models.DTOs;

namespace WebApplication1.Mappings
{
    public class ProdutoMappingProfile : Profile
    {
        public ProdutoMappingProfile()
        {
            // CreateDto -> Entity
            CreateMap<ProdutoCreateDto, Produto>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // Id é gerado pelo sistema

            // UpdateDto -> Entity
            CreateMap<ProdutoUpdateDto, Produto>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // Id vem da URL, não do DTO

            // Entity -> Dto (para resposta)
            CreateMap<Produto, ProdutoDto>()
                .ForMember(dest => dest.Links, opt => opt.Ignore()); // Links são adicionados depois pelo Helper
        }
    }
}
