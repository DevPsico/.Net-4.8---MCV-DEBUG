namespace WebApplication1.Security
{
    /// <summary>
    /// Interface para serviço de autenticação
    /// Define os métodos que o JWT vai implementar com suporte a Refresh Token e Logout
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Gera um token JWT para o usuário
        /// </summary>
        /// <param name="usuario">Nome do usuário</param>
        /// <returns>Token JWT em string</returns>
        string GerarToken(string usuario);

        /// <summary>
        /// Gera um Refresh Token para renovação
        /// </summary>
        /// <param name="usuario">Nome do usuário</param>
        /// <returns>Refresh Token em string</returns>
        string GerarRefreshToken(string usuario);

        /// <summary>
        /// Valida as credenciais do usuário
        /// </summary>
        /// <param name="usuario">Nome do usuário</param>
        /// <param name="senha">Senha do usuário</param>
        /// <returns>True se credenciais válidas, False caso contrário</returns>
        bool ValidarCredenciais(string usuario, string senha);

        /// <summary>
        /// Valida um Refresh Token
        /// </summary>
        /// <param name="refreshToken">Refresh Token a validar</param>
        /// <returns>Nome do usuário se válido, null caso contrário</returns>
        string ValidarRefreshToken(string refreshToken);

        /// <summary>
        /// Revoga um Access Token (logout)
        /// </summary>
        /// <param name="accessToken">Access Token a revogar</param>
        void RevogarToken(string accessToken);
    }
}