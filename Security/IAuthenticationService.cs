namespace WebApplication1.Security
{
    /// <summary>
    /// Interface para serviço de autenticação
    /// Define os métodos que o JWT vai implementar
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Gera um token JWT
        /// </summary>
        /// <param name="usuario">Nome do usuário</param>
        /// <returns>Token JWT em string</returns>
        string GerarToken(string usuario);

        /// <summary>
        /// Valida as credenciais do usuário
        /// </summary>
        /// <param name="usuario">Nome do usuário</param>
        /// <param name="senha">Senha do usuário</param>
        /// <returns>True se credenciais válidas, False caso contrário</returns>
        bool ValidarCredenciais(string usuario, string senha);
    }
}