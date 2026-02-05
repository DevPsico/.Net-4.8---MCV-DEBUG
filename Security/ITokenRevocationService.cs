using System;

namespace WebApplication1.Security
{
    /// <summary>
    /// Serviço para revogar (fazer blacklist) de tokens JWT
    /// Armazena em memória JTI (JWT ID) de tokens revogados
    /// Em produção, usar Redis ou banco de dados
    /// </summary>
    public interface ITokenRevocationService
    {
        /// <summary>
        /// Adiciona um token à blacklist (revoga ele)
        /// </summary>
        void RevogarToken(string jti, DateTime dataExpiracao);

        /// <summary>
        /// Verifica se um token foi revogado
        /// </summary>
        bool TokenFoiRevogado(string jti);

        /// <summary>
        /// Limpa tokens expirados da blacklist (limpeza automática)
        /// </summary>
        void LimparTokensExpirados();
    }
}