using Microsoft.IdentityModel.Tokens;
using System;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using WebApplication1.Security;

namespace WebApplication1.Filters
{
    /// <summary>
    /// Filtro que valida JWT em endpoints protegidos
    /// Uso: [JwtAuthorizeAttribute] no controller ou método
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class JwtAuthorizeAttribute : AuthorizationFilterAttribute
    {
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

                // 4. Valida o token
                var tokenHandler = new JsonWebTokenHandler();
                tokenHandler.ValidateToken(token, new TokenValidationParameters
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

                // 5. Se chegou aqui, o token é válido
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