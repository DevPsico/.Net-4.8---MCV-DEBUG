using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace WebApplication1.Security
{
    /// <summary>
    /// Implementação do serviço de autenticação com JWT
    /// Gera tokens e valida credenciais com suporte a Roles
    /// </summary>
    public class JwtTokenProvider : IAuthenticationService
    {
        private readonly JwtSettings _jwtSettings;

        // Usuários "fake" com seus ROLES (em produção, viria do banco de dados)
        // Formato: { "usuario", ("senha", "ROLE") }
        private static readonly Dictionary<string, (string Senha, string Role)> UsuariosValidos = 
            new Dictionary<string, (string, string)>
        {
            { "admin", ("senha123", "ADMIN") },         // usuário: admin, role: ADMIN
            { "usuario", ("pass456", "USER") },         // usuário: usuario, role: USER
            { "ericson", ("dev2025", "ADMIN") }         // usuário: ericson, role: ADMIN
        };

        public JwtTokenProvider(JwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings ?? throw new ArgumentNullException(nameof(jwtSettings));
        }

        /// <summary>
        /// Valida credenciais (nome de usuário e senha)
        /// </summary>
        public bool ValidarCredenciais(string usuario, string senha)
        {
            // Verifica se usuário existe
            if (!UsuariosValidos.ContainsKey(usuario))
                return false;

            // Compara senha
            return UsuariosValidos[usuario].Senha == senha;
        }

        /// <summary>
        /// Gera um token JWT para o usuário com Role incluído
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
                new Claim("aud", _jwtSettings.Audience)           // Audience
            };

            // 3. Cria a chave de assinatura (secret)
            var key = new SymmetricSecurityKey(_jwtSettings.GetSecretKeyBytes());

            // 4. Cria credenciais de assinatura
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 5. Define quando o token expira
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
    }
}