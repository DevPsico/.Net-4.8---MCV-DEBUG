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
        /// Recebe: { "usuario": "admin", "senha": "senha123" }
        /// Retorna: { "token": "eyJhbGc...", "usuario": "admin", "expiresIn": 3600 }
        /// </summary>
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

            // 3. Gera o token JWT
            var token = _authService.GerarToken(request.Usuario);

            // 4. Retorna o token
            return Ok(new
            {
                token = token,
                usuario = request.Usuario,
                expiresIn = 3600 // 1 hora em segundos
            });
        }
    }

    // DTOs (objetos de dados)
    public class LoginRequest
    {
        public string Usuario { get; set; }
        public string Senha { get; set; }
    }
}