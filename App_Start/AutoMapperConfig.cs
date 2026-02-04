using AutoMapper;
using System.Web.Http;
using WebApplication1.Mappings;

namespace WebApplication1.App_Start
{
    public static class AutoMapperConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProdutoMappingProfile>();
            });

            var mapper = mapperConfig.CreateMapper();
            
            // Armazenar o mapper globalmente
            MapperInstance.Mapper = mapper;
        }
    }

    // Singleton para acessar o Mapper
    public static class MapperInstance
    {
        public static IMapper Mapper { get; set; }
    }
}
