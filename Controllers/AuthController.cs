using System;
using System.Web.Http;
using WebApplication1.Security;

namespace WebApplication1.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private readonly IAuthenticationService _authService;

        public AuthController(IAuthenticationService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        /// <summary>
        /// POST /api/auth/login
        /// Autentica o usuário e retorna Access Token + Refresh Token
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        /// {
        ///   "usuario": "admin",
        ///   "senha": "senha123"
        /// }
        /// 
        /// Exemplo de resposta:
        /// {
        ///   "accessToken": "eyJhbGc...",
        ///   "refreshToken": "eyJhbGc...",
        ///   "expiresIn": 900,
        ///   "tokenType": "Bearer"
        /// }
        /// </remarks>
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public IHttpActionResult Login([FromBody] LoginRequest request)
        {
            // 1. Valida se recebeu dados
            if (request == null || string.IsNullOrWhiteSpace(request.Usuario) || string.IsNullOrWhiteSpace(request.Senha))
                return BadRequest("Usuário e senha são obrigatórios");

            // 2. Valida as credenciais
            if (!_authService.ValidarCredenciais(request.Usuario, request.Senha))
                return Unauthorized();

            // 3. Gera os tokens
            var accessToken = _authService.GerarToken(request.Usuario);
            var refreshToken = _authService.GerarRefreshToken(request.Usuario);

            // 4. Retorna os tokens
            return Ok(new
            {
                accessToken = accessToken,
                refreshToken = refreshToken,
                expiresIn = 900,  // 15 minutos em segundos
                tokenType = "Bearer"
            });
        }

        /// <summary>
        /// POST /api/auth/refresh
        /// Renova o Access Token usando um Refresh Token válido
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        /// {
        ///   "refreshToken": "eyJhbGc..."
        /// }
        /// 
        /// Exemplo de resposta:
        /// {
        ///   "accessToken": "eyJhbGc...",
        ///   "refreshToken": "eyJhbGc...",
        ///   "expiresIn": 900,
        ///   "tokenType": "Bearer"
        /// }
        /// </remarks>
        [HttpPost]
        [Route("refresh")]
        [AllowAnonymous]
        public IHttpActionResult RefreshToken([FromBody] RefreshTokenRequest request)
        {
            // 1. Valida se recebeu o refresh token
            if (request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
                return BadRequest("Refresh token é obrigatório");

            // 2. Valida o refresh token
            var usuario = _authService.ValidarRefreshToken(request.RefreshToken);
            if (string.IsNullOrEmpty(usuario))
                return Unauthorized();

            // 3. Gera novos tokens
            var novoAccessToken = _authService.GerarToken(usuario);
            var novoRefreshToken = _authService.GerarRefreshToken(usuario);

            // 4. Retorna os novos tokens
            return Ok(new
            {
                accessToken = novoAccessToken,
                refreshToken = novoRefreshToken,
                expiresIn = 900,  // 15 minutos em segundos
                tokenType = "Bearer"
            });
        }

        /// <summary>
        /// POST /api/auth/logout
        /// Revoga o Access Token e o Refresh Token
        /// Usuário não conseguirá mais usar esses tokens
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        /// {
        ///   "accessToken": "eyJhbGc...",
        ///   "refreshToken": "eyJhbGc..."
        /// }
        /// 
        /// Exemplo de resposta:
        /// {
        ///   "message": "Logout realizado com sucesso",
        ///   "success": true
        /// }
        /// </remarks>
        [HttpPost]
        [Route("logout")]
        [AllowAnonymous]
        public IHttpActionResult Logout([FromBody] LogoutRequest request)
        {
            // 1. Valida se recebeu os tokens
            if (request == null || string.IsNullOrWhiteSpace(request.AccessToken))
                return BadRequest("Access Token é obrigatório");

            // 2. Revoga o Access Token
            _authService.RevogarToken(request.AccessToken);

            // 3. Revoga o Refresh Token (se fornecido)
            if (!string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                _authService.RevogarToken(request.RefreshToken);
            }

            // 4. Retorna sucesso
            return Ok(new
            {
                message = "Logout realizado com sucesso",
                success = true
            });
        }
    }

    // DTOs (objetos de dados)
    public class LoginRequest
    {
        public string Usuario { get; set; }
        public string Senha { get; set; }
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }
    }

    public class LogoutRequest
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}