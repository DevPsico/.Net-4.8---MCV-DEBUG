using Microsoft.IdentityModel.Tokens;
using System;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using WebApplication1.Security;

namespace WebApplication1.Filters
{
    /// <summary>
    /// Filtro que valida JWT E verifica se o usuário tem o Role requerido
    /// Uso: [JwtAuthorizeRoleAttribute("ADMIN")] ou [JwtAuthorizeRoleAttribute("USER", "ADMIN")]
    /// Exemplo 1: [JwtAuthorizeRoleAttribute("ADMIN")] - apenas ADMIN pode acessar
    /// Exemplo 2: [JwtAuthorizeRoleAttribute("USER", "ADMIN")] - USER ou ADMIN podem acessar
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class JwtAuthorizeRoleAttribute : AuthorizationFilterAttribute
    {
        private readonly string[] _rolesPermitidos;

        /// <summary>
        /// Construtor que recebe os roles permitidos
        /// </summary>
        /// <param name="roles">Roles permitidos (ex: "ADMIN", "USER")</param>
        public JwtAuthorizeRoleAttribute(params string[] roles)
        {
            _rolesPermitidos = roles ?? new string[] { };
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            // 1. Obtém o header Authorization
            var authHeader = actionContext.Request.Headers.Authorization;

            // 2. Valida se tem token
            if (authHeader == null || authHeader.Scheme != "Bearer")
            {
                actionContext.Response = actionContext.Request.CreateResponse(
                    System.Net.HttpStatusCode.Unauthorized,
                    new { message = "Token não fornecido. Use: Authorization: Bearer {token}" });
                return;
            }

            try
            {
                // 3. Extrai o token
                var token = authHeader.Parameter;
                var jwtSettings = new JwtSettings();
                var key = new SymmetricSecurityKey(jwtSettings.GetSecretKeyBytes());

                // 4. Valida o token e obtém o resultado com as claims
                var tokenHandler = new JsonWebTokenHandler();
                var result = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                });

                // 5. Verifica se o token é válido
                if (!result.IsValid)
                {
                    actionContext.Response = actionContext.Request.CreateResponse(
                        System.Net.HttpStatusCode.Unauthorized,
                        new { message = "Token inválido" });
                    return;
                }

                // 6. Obtém o claim de role do token
                var roleClaim = result.Claims.FirstOrDefault(c => c.Key == "role").Value?.ToString();

                // 7. Verifica se o usuário tem algum dos roles requeridos
                if (!_rolesPermitidos.Contains(roleClaim))
                {
                    actionContext.Response = actionContext.Request.CreateResponse(
                        System.Net.HttpStatusCode.Forbidden,
                        new { message = $"Acesso negado. Roles requeridos: {string.Join(", ", _rolesPermitidos)}. Seu role: {roleClaim}" });
                    return;
                }

                // 8. Se chegou aqui, token e role estão validados!
                base.OnAuthorization(actionContext);
            }
            catch (SecurityTokenExpiredException)
            {
                actionContext.Response = actionContext.Request.CreateResponse(
                    System.Net.HttpStatusCode.Unauthorized,
                    new { message = "Token expirado" });
            }
            catch (Exception ex)
            {
                actionContext.Response = actionContext.Request.CreateResponse(
                    System.Net.HttpStatusCode.Unauthorized,
                    new { message = $"Token inválido: {ex.Message}" });
            }
        }
    }
}
