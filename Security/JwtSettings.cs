namespace WebApplication1.Security
{
    /// <summary>
    /// Configurações do JWT
    /// Contém Secret, Issuer, Audience, Tempo de Expiração (Access Token e Refresh Token)
    /// </summary>
    public class JwtSettings
    {
        // Chave secreta para assinar o token (MUDE ISSO EM PRODUÇÃO!)
        public string Secret { get; set; } = "sua-chave-secreta-super-segura-com-minimo-32-caracteres-aqui-12345";

        // Quem está emitindo o token
        public string Issuer { get; set; } = "WebApplication1";

        // Para quem o token é válido
        public string Audience { get; set; } = "WebApplication1Users";

        // Quantos minutos o ACCESS TOKEN dura (curta vida: 15 minutos)
        public int ExpirationMinutes { get; set; } = 15;

        // Quantos dias o REFRESH TOKEN dura (longa vida: 7 dias)
        public int RefreshTokenExpirationDays { get; set; } = 7;

        // Método auxiliar para gerar chave em bytes (usado no JWT)
        public byte[] GetSecretKeyBytes()
        {
            return System.Text.Encoding.UTF8.GetBytes(this.Secret);
        }
    }
}