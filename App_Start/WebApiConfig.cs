using System.Web.Http;
using WebApplication1.Filters;

namespace WebApplication1.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // ========================================
            // SEGURANÇA NÍVEL ENTERPRISE
            // ========================================
            
            // CAMADA 1: Força uso de HTTPS (Defense in Depth)
            // Em produção, todas as requisições HTTP serão redirecionadas para HTTPS
            // Em desenvolvimento local, apenas loga um aviso (não bloqueia)
            config.Filters.Add(new RequireHttpsAttribute());

            // CAMADA 2: HSTS (HTTP Strict Transport Security)
            // Instrui navegadores a SEMPRE usar HTTPS por 365 dias
            // Protege contra downgrade attacks e man-in-the-middle
            config.Filters.Add(new HstsHeaderAttribute(
                maxAgeDays: 365,        // 1 ano
                includeSubDomains: true, // Aplica a subdomínios
                preload: false          // Não incluir em preload lists (requer setup adicional)
            ));

            // CAMADA 3: Security Headers (OWASP Compliance)
            // Adiciona headers de segurança obrigatórios para clientes enterprise:
            // - X-Content-Type-Options: Previne MIME-sniffing
            // - X-Frame-Options: Previne Clickjacking
            // - X-XSS-Protection: Proteção XSS
            // - Content-Security-Policy: Política de conteúdo
            // - Permissions-Policy: Controle de features do navegador
            // - Referrer-Policy: Controle de informações de referência
            config.Filters.Add(new SecurityHeadersAttribute());

            // ========================================
            // CONFIGURAÇÃO DE ROTAS
            // ========================================
            
            // Rotas por atributos (RESTful)
            config.MapHttpAttributeRoutes();

            // Rota padrão da Web API (fallback)
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // ========================================
            // CONFIGURAÇÃO DE FORMATADORES
            // ========================================
            
            // JSON como formato padrão (RESTful best practice)
            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
            json.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            
            // Remove XML (API moderna, JSON-only)
            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }
    }
}