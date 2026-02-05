using System.Web.Http;
using System.Web.Http.Dependencies;
using System;
using WebApplication1.Repositories;
using WebApplication1.Services;
using WebApplication1.Security;

namespace WebApplication1.App_Start
{
    public class SimpleInjectorContainer : IDependencyResolver
    {
        public IDependencyScope BeginScope()
        {
            return this;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IProdutoRepository))
                return new ProdutoRepository();
            
            if (serviceType == typeof(IProdutoService))
                return new ProdutoService(new ProdutoRepository());

            if (serviceType == typeof(Controllers.ProdutosApiController))
                return new Controllers.ProdutosApiController(new ProdutoService(new ProdutoRepository()));

            // Registrar IAuthenticationService
            if (serviceType == typeof(IAuthenticationService))
                return new JwtTokenProvider(new JwtSettings(), new TokenRevocationService());

            // Registrar AuthController
            if (serviceType == typeof(Controllers.AuthController))
                return new Controllers.AuthController(new JwtTokenProvider(new JwtSettings(), new TokenRevocationService()));

            return null;
        }

        public System.Collections.Generic.IEnumerable<object> GetServices(Type serviceType)
        {
            return new System.Collections.Generic.List<object>();
        }

        public void Dispose()
        {
        }
    }

    public static class DependencyInjectionConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.DependencyResolver = new SimpleInjectorContainer();
        }
    }
}
