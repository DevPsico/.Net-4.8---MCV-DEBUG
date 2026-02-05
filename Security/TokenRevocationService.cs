using System;
using System.Collections.Concurrent;
using System.Linq;

namespace WebApplication1.Security
{
    /// <summary>
    /// Implementação em memória do serviço de revogação
    /// Para produção, implementar com Redis ou banco de dados
    /// </summary>
    public class TokenRevocationService : ITokenRevocationService
    {
        // Armazena JTI e data de expiração dos tokens revogados
        // Formato: { "jti", dataExpiracao }
        private static readonly ConcurrentDictionary<string, DateTime> TokensRevogados =
            new ConcurrentDictionary<string, DateTime>();

        /// <summary>
        /// Adiciona um token à blacklist
        /// </summary>
        public void RevogarToken(string jti, DateTime dataExpiracao)
        {
            if (string.IsNullOrWhiteSpace(jti))
                throw new ArgumentException("JTI não pode ser vazio", nameof(jti));

            TokensRevogados.TryAdd(jti, dataExpiracao);
        }

        /// <summary>
        /// Verifica se um token foi revogado
        /// </summary>
        public bool TokenFoiRevogado(string jti)
        {
            if (string.IsNullOrWhiteSpace(jti))
                return false;

            return TokensRevogados.ContainsKey(jti);
        }

        /// <summary>
        /// Remove tokens expirados da blacklist para liberar memória
        /// Deve ser chamado periodicamente (ex: a cada 1 hora)
        /// </summary>
        public void LimparTokensExpirados()
        {
            var agora = DateTime.UtcNow;
            var jtiExpirados = TokensRevogados
                .Where(x => x.Value < agora)
                .Select(x => x.Key)
                .ToList();

            foreach (var jti in jtiExpirados)
            {
                TokensRevogados.TryRemove(jti, out _);
            }
        }
    }
}