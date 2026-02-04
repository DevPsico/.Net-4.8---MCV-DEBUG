using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace WebApplication1.Filters
{
    /// <summary>
    /// Filtro que força o uso de HTTPS em todas as requisições
    /// Em produção, redireciona HTTP para HTTPS
    /// Em desenvolvimento, apenas loga um aviso
    /// </summary>
    public class RequireHttpsAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var request = actionContext.Request;

            // Verifica se a requisição já está usando HTTPS
            if (request.RequestUri.Scheme != Uri.UriSchemeHttps)
            {
                // Em desenvolvimento, apenas logamos
                if (IsLocalRequest(request))
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"[AVISO HTTPS] Requisição local sem HTTPS: {request.RequestUri}");
                    
                    // Não bloqueamos em ambiente local para facilitar desenvolvimento
                    return;
                }

                // Em produção, redirecionamos para HTTPS
                var httpsUri = new UriBuilder(request.RequestUri)
                {
                    Scheme = Uri.UriSchemeHttps,
                    Port = 443 // Porta padrão HTTPS
                }.Uri;

                var response = request.CreateResponse(HttpStatusCode.Redirect);
                response.Headers.Location = httpsUri;

                actionContext.Response = response;
            }

            base.OnAuthorization(actionContext);
        }

        /// <summary>
        /// Verifica se a requisição é local (localhost ou 127.0.0.1)
        /// </summary>
        private bool IsLocalRequest(HttpRequestMessage request)
        {
            var host = request.RequestUri.Host.ToLower();
            return host == "localhost" || 
                   host == "127.0.0.1" || 
                   host == "::1" ||
                   host.EndsWith(".local");
        }
    }
}
