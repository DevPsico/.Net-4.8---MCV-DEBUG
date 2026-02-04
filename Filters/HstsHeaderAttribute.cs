using System.Web.Http.Filters;

namespace WebApplication1.Filters
{
    /// <summary>
    /// Adiciona o header HSTS (HTTP Strict Transport Security)
    /// Instrui navegadores a SEMPRE usar HTTPS por um período determinado
    /// </summary>
    public class HstsHeaderAttribute : ActionFilterAttribute
    {
        private readonly int _maxAgeInSeconds;
        private readonly bool _includeSubDomains;
        private readonly bool _preload;

        /// <summary>
        /// Configura HSTS
        /// </summary>
        /// <param name="maxAgeDays">Duração em dias que o navegador deve lembrar de usar HTTPS (padrão: 365 dias)</param>
        /// <param name="includeSubDomains">Se deve aplicar a subdomínios também (padrão: true)</param>
        /// <param name="preload">Se deve ser incluído na lista de preload dos navegadores (padrão: false)</param>
        public HstsHeaderAttribute(int maxAgeDays = 365, bool includeSubDomains = true, bool preload = false)
        {
            _maxAgeInSeconds = maxAgeDays * 24 * 60 * 60;
            _includeSubDomains = includeSubDomains;
            _preload = preload;
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Response != null)
            {
                // Só adiciona HSTS se a requisição for HTTPS
                // (HSTS não faz sentido em HTTP)
                if (actionExecutedContext.Request.RequestUri.Scheme == "https")
                {
                    var hstsValue = $"max-age={_maxAgeInSeconds}";

                    if (_includeSubDomains)
                        hstsValue += "; includeSubDomains";

                    if (_preload)
                        hstsValue += "; preload";

                    actionExecutedContext.Response.Headers.Add(
                        "Strict-Transport-Security", 
                        hstsValue);
                }
            }

            base.OnActionExecuted(actionExecutedContext);
        }
    }
}
