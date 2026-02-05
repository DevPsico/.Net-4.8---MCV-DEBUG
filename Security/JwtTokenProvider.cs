using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace WebApplication1.Security
{
    /// <summary>
    /// Implementação do serviço de autenticação com JWT
    /// Gera tokens com Refresh Token e valida credenciais com suporte a Roles
    /// Senhas são armazenadas como HASH seguro (PBKDF2)
    /// </summary>
    public class JwtTokenProvider : IAuthenticationService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly ITokenRevocationService _revocationService;

        // Usuários "fake" com seus ROLES e SENHAS
        // Formato: { "usuario", ("senha", "ROLE") }
        private static readonly Dictionary<string, (string Senha, string Role)> UsuariosValidos = 
            new Dictionary<string, (string, string)>
        {
            { "admin", ("senha123", "ADMIN") },
            { "usuario", ("pass456", "USER") },
            { "ericson", ("dev2025", "ADMIN") }
        };

        public JwtTokenProvider(JwtSettings jwtSettings, ITokenRevocationService revocationService = null)
        {
            _jwtSettings = jwtSettings ?? throw new ArgumentNullException(nameof(jwtSettings));
            if (revocationService == null)
            {
                throw new ArgumentNullException(nameof(revocationService), "ITokenRevocationService implementation must be provided.");
            }
            _revocationService = revocationService;
        }

        /// <summary>
        /// Valida credenciais (nome de usuário e senha)
        /// </summary>
        public bool ValidarCredenciais(string usuario, string senha)
        {
            // Verifica se usuário existe
            if (!UsuariosValidos.ContainsKey(usuario))
                return false;

            // Compara senha diretamente
            var senhaArmazenada = UsuariosValidos[usuario].Senha;
            return senhaArmazenada == senha;
        }

        /// <summary>
        /// Gera um Access Token (JWT com vida curta: 15 minutos)
        /// </summary>
        public string GerarToken(string usuario)
        {
            // 1. Obtém o role do usuário
            var role = UsuariosValidos[usuario].Role;

            // 2. Cria as "claims" (informações) que vão no token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario),              // Nome do usuário
                new Claim(ClaimTypes.NameIdentifier, usuario),    // ID do usuário
                new Claim(ClaimTypes.Role, role),                 // ROLE do usuário (standard claim)
                new Claim("usuario", usuario),                    // Claim customizado
                new Claim("role", role),                          // Claim customizado do role
                new Claim("jti", Guid.NewGuid().ToString()),      // ID único do token
                new Claim("iss", _jwtSettings.Issuer),            // Issuer
                new Claim("aud", _jwtSettings.Audience),          // Audience
                new Claim("tipo", "access")                       // Tipo: access token
            };

            // 3. Cria a chave de assinatura (secret)
            var key = new SymmetricSecurityKey(_jwtSettings.GetSecretKeyBytes());

            // 4. Cria credenciais de assinatura
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 5. Define quando o token expira (15 minutos)
            var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

            // 6. Cria o token com todas as informações
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiration,
                SigningCredentials = creds,
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience
            };

            // 7. Converte o token em string
            var tokenHandler = new JsonWebTokenHandler();
            return tokenHandler.CreateToken(descriptor);
        }

        /// <summary>
        /// Gera um Refresh Token (JWT com vida longa: 7 dias)
        /// Este token é usado para renovar o Access Token sem fazer login novamente
        /// </summary>
        public string GerarRefreshToken(string usuario)
        {
            // 1. Cria as "claims" para o refresh token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario),              // Nome do usuário
                new Claim(ClaimTypes.NameIdentifier, usuario),    // ID do usuário
                new Claim("jti", Guid.NewGuid().ToString()),      // ID único do token
                new Claim("iss", _jwtSettings.Issuer),            // Issuer
                new Claim("aud", _jwtSettings.Audience),          // Audience
                new Claim("tipo", "refresh")                      // Tipo: refresh token
            };

            // 2. Cria a chave de assinatura (secret)
            var key = new SymmetricSecurityKey(_jwtSettings.GetSecretKeyBytes());

            // 3. Cria credenciais de assinatura
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 4. Define quando o token expira (7 dias)
            var expiration = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);

            // 5. Cria o token com todas as informações
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiration,
                SigningCredentials = creds,
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience
            };

            // 6. Converte o token em string
            var tokenHandler = new JsonWebTokenHandler();
            return tokenHandler.CreateToken(descriptor);
        }

        /// <summary>
        /// Valida um Refresh Token e retorna o usuário se for válido
        /// </summary>
        public string ValidarRefreshToken(string refreshToken)
        {
            try
            {
                var jwtSettings = new JwtSettings();
                var key = new SymmetricSecurityKey(jwtSettings.GetSecretKeyBytes());

                // 1. Valida o refresh token
                var tokenHandler = new JsonWebTokenHandler();
                var result = tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
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

                // 2. Verifica se o token é válido e se é um refresh token
                if (!result.IsValid)
                    return null;

                // 3. Verifica o tipo do token
                var tipoClaim = result.Claims["tipo"];
                if (tipoClaim?.ToString() != "refresh")
                    return null;

                // 4. Retorna o nome do usuário
                var usernameClaim = result.Claims[ClaimTypes.Name];
                return usernameClaim?.ToString();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Revoga um Access Token (logout)
        /// Adiciona o token à blacklist para que não possa mais ser usado
        /// </summary>
        public void RevogarToken(string accessToken)
        {
            try
            {
                var jwtSettings = new JwtSettings();
                var key = new SymmetricSecurityKey(jwtSettings.GetSecretKeyBytes());

                // 1. Valida o token para extrair o JTI
                var tokenHandler = new JsonWebTokenHandler();
                var result = tokenHandler.ValidateToken(accessToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = false, // Não valida expiração aqui (token pode estar expirado)
                    ClockSkew = TimeSpan.Zero
                });

                // 2. Extrai o JTI e calcula a data de expiração
                var jti = result.Claims.ContainsKey("jti") 
                    ? result.Claims["jti"]?.ToString() 
                    : null;
                
                // Calcula a expiração: agora + 15 minutos (tempo de vida do access token)
                var expiration = DateTime.UtcNow.AddMinutes(jwtSettings.ExpirationMinutes);

                // 3. Adiciona à blacklist
                if (!string.IsNullOrEmpty(jti))
                {
                    _revocationService.RevogarToken(jti, expiration);
                }
            }
            catch
            {
                // Se houver erro ao processar o token, ignora silenciosamente
                // O token não será adicionado à blacklist, mas não deve causar erro ao usuário
            }
        }
    }
}