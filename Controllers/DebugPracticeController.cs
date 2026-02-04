using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    /// <summary>
    /// Controller ESPECIAL para praticar DEBUG
    /// Use este controller para aprender técnicas de debugging
    /// </summary>
    [RoutePrefix("api/debug")]
    public class DebugPracticeController : ApiController
    {
        // ========================================
        // EXERCÍCIO 1: Debug Básico com Breakpoints
        // ========================================
        /// <summary>
        /// EXERCÍCIO 1: Coloque um breakpoint na linha do cálculo
        /// e inspecione as variáveis
        /// </summary>
        [HttpGet]
        [Route("exercicio1/{numero:int}")]
        public IHttpActionResult Exercicio1(int numero)
        {
            // ?? COLOQUE UM BREAKPOINT AQUI (F9)
            var resultado = numero * 2;
            
            // Quando parar aqui:
            // 1. Veja o valor de 'numero' no Locals
            // 2. Adicione 'resultado' no Watch
            // 3. Use Immediate Window: ? numero + resultado
            
            return Ok(new { 
                numero = numero, 
                dobro = resultado,
                mensagem = "Parabéns! Você debugou com sucesso!"
            });
        }

        // ========================================
        // EXERCÍCIO 2: Step Over (F10) vs Step Into (F11)
        // ========================================
        /// <summary>
        /// EXERCÍCIO 2: Use F10 e F11 para navegar
        /// </summary>
        [HttpGet]
        [Route("exercicio2/{nome}")]
        public IHttpActionResult Exercicio2(string nome)
        {
            // ?? BREAKPOINT AQUI
            var saudacao = GerarSaudacao(nome);  // ? Pressione F11 aqui (entra no método)
            
            var resultado = ProcessarSaudacao(saudacao); // ? Pressione F10 aqui (não entra)
            
            return Ok(new { saudacao = resultado });
        }

        private string GerarSaudacao(string nome)
        {
            // Quando pressionar F11, vai parar AQUI
            return $"Olá, {nome}!";
        }

        private string ProcessarSaudacao(string saudacao)
        {
            return saudacao.ToUpper();
        }

        // ========================================
        // EXERCÍCIO 3: Conditional Breakpoint
        // ========================================
        /// <summary>
        /// EXERCÍCIO 3: Breakpoint condicional em loop
        /// Botão direito no breakpoint ? Conditions ? preco > 1000
        /// </summary>
        [HttpGet]
        [Route("exercicio3")]
        public IHttpActionResult Exercicio3()
        {
            var produtos = GerarProdutos();
            var caros = new List<Produto>();

            foreach (var produto in produtos)
            {
                // ?? BREAKPOINT CONDICIONAL AQUI
                // Botão direito ? Conditions ? produto.Preco > 1000
                if (produto.Preco > 1000)
                {
                    caros.Add(produto);
                }
            }

            return Ok(new { 
                total = produtos.Count, 
                caros = caros.Count,
                lista = caros 
            });
        }

        // ========================================
        // EXERCÍCIO 4: Debug de Exceptions
        // ========================================
        /// <summary>
        /// EXERCÍCIO 4: Debug quando ocorre exceção
        /// Ctrl+Alt+E ? Marque "Common Language Runtime Exceptions"
        /// </summary>
        [HttpGet]
        [Route("exercicio4/{dividir:int}")]
        public IHttpActionResult Exercicio4(int dividir)
        {
            try
            {
                // ?? BREAKPOINT AQUI
                var resultado = 100 / dividir;  // Tente com dividir=0
                
                return Ok(new { resultado = resultado });
            }
            catch (DivideByZeroException ex)
            {
                // ?? BREAKPOINT AQUI para inspecionar a exceção
                Debug.WriteLine($"Erro capturado: {ex.Message}");
                
                // No Watch, adicione:
                // - ex.Message
                // - ex.StackTrace
                
                return BadRequest("Não pode dividir por zero!");
            }
        }

        // ========================================
        // EXERCÍCIO 5: Debug Async/Await
        // ========================================
        /// <summary>
        /// EXERCÍCIO 5: Debug de código assíncrono
        /// Observe como o código "pula" entre linhas
        /// </summary>
        [HttpGet]
        [Route("exercicio5")]
        public async Task<IHttpActionResult> Exercicio5()
        {
            // ?? BREAKPOINT 1
            Debug.WriteLine("Iniciando operação async");
            
            // ?? BREAKPOINT 2 (vai "pular" para aqui)
            var resultado = await OperacaoAsyncSimulada();
            
            // ?? BREAKPOINT 3 (continua aqui DEPOIS do await)
            Debug.WriteLine($"Resultado: {resultado}");
            
            return Ok(new { resultado = resultado });
        }

        private async Task<string> OperacaoAsyncSimulada()
        {
            // ?? BREAKPOINT 4 (se usar F11 no await acima)
            await Task.Delay(100); // Simula operação demorada
            return "Operação concluída!";
        }

        // ========================================
        // EXERCÍCIO 6: Inspecionar Request HTTP
        // ========================================
        /// <summary>
        /// EXERCÍCIO 6: Inspecionar headers e request
        /// </summary>
        [HttpPost]
        [Route("exercicio6")]
        public IHttpActionResult Exercicio6([FromBody] Produto produto)
        {
            // ?? BREAKPOINT AQUI
            
            // No Watch, adicione:
            // - produto
            // - produto.Nome
            // - Request.Headers
            // - Request.Content
            
            var headers = Request.Headers.ToDictionary(
                h => h.Key, 
                h => string.Join(", ", h.Value));

            return Ok(new
            {
                produtoRecebido = produto,
                headersRecebidos = headers,
                metodo = Request.Method.Method,
                url = Request.RequestUri.ToString()
            });
        }

        // ========================================
        // EXERCÍCIO 7: Call Stack (Pilha de Chamadas)
        // ========================================
        /// <summary>
        /// EXERCÍCIO 7: Entender Call Stack
        /// Debug ? Windows ? Call Stack (Ctrl+Alt+C)
        /// </summary>
        [HttpGet]
        [Route("exercicio7")]
        public IHttpActionResult Exercicio7()
        {
            // Quando parar no método mais profundo,
            // veja o Call Stack para entender o caminho
            var resultado = MetodoNivel1();
            return Ok(new { resultado = resultado });
        }

        private string MetodoNivel1()
        {
            return MetodoNivel2();
        }

        private string MetodoNivel2()
        {
            return MetodoNivel3();
        }

        private string MetodoNivel3()
        {
            // ?? BREAKPOINT AQUI
            // Abra Call Stack (Ctrl+Alt+C) e veja:
            // MetodoNivel3 ? VOCÊ ESTÁ AQUI
            // MetodoNivel2 ? Foi chamado por aqui
            // MetodoNivel1 ? Foi chamado por aqui
            // Exercicio7   ? Foi chamado por aqui
            
            return "Olhe o Call Stack!";
        }

        // ========================================
        // EXERCÍCIO 8: Immediate Window
        // ========================================
        /// <summary>
        /// EXERCÍCIO 8: Usar Immediate Window para testar código
        /// Debug ? Windows ? Immediate (Ctrl+Alt+I)
        /// </summary>
        [HttpGet]
        [Route("exercicio8/{preco:decimal}")]
        public IHttpActionResult Exercicio8(decimal preco)
        {
            // ?? BREAKPOINT AQUI
            var produto = new Produto 
            { 
                Id = 1, 
                Nome = "Notebook", 
                Preco = preco 
            };

            // No Immediate Window, teste:
            // ? produto.Nome
            // ? produto.Preco * 1.1
            // ? produto.Preco > 1000
            // ? CalcularDesconto(produto.Preco, 0.1m)
            
            var precoComDesconto = CalcularDesconto(preco, 0.1m);
            
            return Ok(new 
            { 
                produto = produto,
                precoOriginal = preco,
                precoComDesconto = precoComDesconto
            });
        }

        private decimal CalcularDesconto(decimal preco, decimal percentual)
        {
            return preco - (preco * percentual);
        }

        // ========================================
        // EXERCÍCIO 9: Tracepoint (Log sem parar)
        // ========================================
        /// <summary>
        /// EXERCÍCIO 9: Usar Tracepoint
        /// Botão direito no breakpoint ? Actions ? "Log a message to Output Window"
        /// Digite: "Processando produto: {produto.Nome}, Preço: {produto.Preco}"
        /// </summary>
        [HttpGet]
        [Route("exercicio9")]
        public IHttpActionResult Exercicio9()
        {
            var produtos = GerarProdutos();

            foreach (var produto in produtos)
            {
                // ?? TRACEPOINT AQUI (não breakpoint normal!)
                // 1. F9 para criar breakpoint
                // 2. Botão direito ? Actions
                // 3. Marque "Log a message"
                // 4. Digite: Processando {produto.Nome}
                // 5. DESMARQUE "Continue execution"
                ProcessarProduto(produto);
            }

            return Ok(new { total = produtos.Count });
        }

        private void ProcessarProduto(Produto produto)
        {
            // Simula processamento
            var resultado = produto.Preco * 1.1m;
        }

        // ========================================
        // EXERCÍCIO 10: Debug de Microserviço
        // ========================================
        /// <summary>
        /// EXERCÍCIO 10: Simular chamada entre microserviços
        /// Este endpoint "chama" outro serviço
        /// </summary>
        [HttpGet]
        [Route("exercicio10/{produtoId:int}")]
        public async Task<IHttpActionResult> Exercicio10(int produtoId)
        {
            var correlationId = Guid.NewGuid();
            
            // ?? BREAKPOINT 1
            Debug.WriteLine($"[{correlationId}] Iniciando busca produto {produtoId}");
            
            // Simula busca de produto
            var produto = await BuscarProdutoAsync(produtoId);
            Debug.WriteLine($"[{correlationId}] Produto encontrado: {produto?.Nome}");
            
            // ?? BREAKPOINT 2
            // Simula chamada para outro microserviço (Estoque)
            var estoque = await ConsultarEstoqueAsync(produtoId, correlationId);
            Debug.WriteLine($"[{correlationId}] Estoque: {estoque}");
            
            return Ok(new 
            { 
                correlationId = correlationId,
                produto = produto,
                estoque = estoque
            });
        }

        private async Task<Produto> BuscarProdutoAsync(int id)
        {
            // ?? BREAKPOINT 3 (se usar F11)
            await Task.Delay(50); // Simula latência
            return new Produto 
            { 
                Id = id, 
                Nome = "Produto Teste", 
                Preco = 100m 
            };
        }

        private async Task<int> ConsultarEstoqueAsync(int produtoId, Guid correlationId)
        {
            // ?? BREAKPOINT 4
            Debug.WriteLine($"[{correlationId}] Consultando estoque do produto {produtoId}");
            await Task.Delay(100); // Simula chamada HTTP para outro serviço
            return 42; // Quantidade em estoque
        }

        // ========================================
        // HELPER METHODS
        // ========================================
        private List<Produto> GerarProdutos()
        {
            return new List<Produto>
            {
                new Produto { Id = 1, Nome = "Mouse", Preco = 50m },
                new Produto { Id = 2, Nome = "Teclado", Preco = 200m },
                new Produto { Id = 3, Nome = "Monitor", Preco = 800m },
                new Produto { Id = 4, Nome = "Notebook", Preco = 3500m },
                new Produto { Id = 5, Nome = "Cadeira Gamer", Preco = 1200m }
            };
        }
    }
}
