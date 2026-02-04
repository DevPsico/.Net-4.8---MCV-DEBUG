using System.Web.Http.Filters;

namespace WebApplication1.Filters
{
    /// <summary>
    /// Adiciona headers de segurança recomendados por OWASP e padrões enterprise
    /// </summary>
    /// <remarks>
    /// Headers implementados:
    /// - X-Content-Type-Options: Previne MIME-sniffing
    /// - X-Frame-Options: Previne Clickjacking
    /// - X-XSS-Protection: Proteção XSS do navegador
    /// - Referrer-Policy: Controla informações de referência
    /// - Content-Security-Policy: Define políticas de conteúdo
    /// - Permissions-Policy: Controla features do navegador
    /// </remarks>
    public class SecurityHeadersAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Response != null)
            {
                var headers = actionExecutedContext.Response.Headers;

                // ==========================================
                // 1. X-Content-Type-Options
                // ==========================================
                // Previne MIME-sniffing attacks
                // Força navegador a respeitar o Content-Type declarado
                if (!headers.Contains("X-Content-Type-Options"))
                {
                    headers.Add("X-Content-Type-Options", "nosniff");
                }

                // ==========================================
                // 2. X-Frame-Options
                // ==========================================
                // Previne ataques de Clickjacking
                // DENY: Não permite iframe de ninguém
                // SAMEORIGIN: Permite apenas do mesmo domínio
                if (!headers.Contains("X-Frame-Options"))
                {
                    headers.Add("X-Frame-Options", "DENY");
                }

                // ==========================================
                // 3. X-XSS-Protection
                // ==========================================
                // Ativa proteção XSS do navegador (legacy, mas ainda útil)
                // 1: Ativa proteção
                // mode=block: Bloqueia página ao invés de sanitizar
                if (!headers.Contains("X-XSS-Protection"))
                {
                    headers.Add("X-XSS-Protection", "1; mode=block");
                }

                // ==========================================
                // 4. Referrer-Policy
                // ==========================================
                // Controla quanto de informação de referência é enviada
                // strict-origin-when-cross-origin: Balanceia segurança e funcionalidade
                if (!headers.Contains("Referrer-Policy"))
                {
                    headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
                }

                // ==========================================
                // 5. Content-Security-Policy (CSP)
                // ==========================================
                // Define de onde recursos podem ser carregados
                // Crucial para prevenir XSS e injeção de dados
                if (!headers.Contains("Content-Security-Policy"))
                {
                    headers.Add("Content-Security-Policy", 
                        "default-src 'self'; " +           // Padrão: apenas mesma origem
                        "script-src 'self'; " +             // Scripts: apenas mesma origem
                        "style-src 'self' 'unsafe-inline'; " + // Estilos: permite inline (comum em APIs)
                        "img-src 'self' data:; " +          // Imagens: mesma origem + data URIs
                        "font-src 'self'; " +               // Fontes: apenas mesma origem
                        "connect-src 'self'; " +            // AJAX/WebSocket: apenas mesma origem
                        "frame-ancestors 'none'; " +        // Não permite ser embedado em iframe
                        "base-uri 'self'; " +               // Restringe tag <base>
                        "form-action 'self'");              // Formulários: apenas mesma origem
                }

                // ==========================================
                // 6. Permissions-Policy (Feature-Policy)
                // ==========================================
                // Controla quais features do navegador podem ser usadas
                // Desabilita features não necessárias em uma API
                if (!headers.Contains("Permissions-Policy"))
                {
                    headers.Add("Permissions-Policy",
                        "accelerometer=(), " +      // Desabilita acelerômetro
                        "camera=(), " +              // Desabilita câmera
                        "geolocation=(), " +         // Desabilita localização
                        "gyroscope=(), " +           // Desabilita giroscópio
                        "magnetometer=(), " +        // Desabilita magnetômetro
                        "microphone=(), " +          // Desabilita microfone
                        "payment=(), " +             // Desabilita Payment API
                        "usb=()");                   // Desabilita USB
                }

                // ==========================================
                // 7. X-Powered-By (Remover)
                // ==========================================
                // Remove header que expõe tecnologia (ASP.NET)
                // Dificulta reconnaissance de atacantes
                if (headers.Contains("X-Powered-By"))
                {
                    headers.Remove("X-Powered-By");
                }

                // ==========================================
                // 8. Server (Remover/Ofuscar)
                // ==========================================
                // Remove ou ofusca versão do servidor
                // Nota: Este header é adicionado pelo IIS, precisa configuração adicional
                if (headers.Contains("Server"))
                {
                    headers.Remove("Server");
                }
            }

            base.OnActionExecuted(actionExecutedContext);
        }
    }
}