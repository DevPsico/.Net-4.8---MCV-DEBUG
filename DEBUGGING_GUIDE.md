# ?? GUIA COMPLETO DE DEBUG - MICROSERVIÇOS

## ?? ÍNDICE
1. [Debug Básico (Visual Studio)](#debug-básico)
2. [Debug de API Web (Postman/Swagger)](#debug-de-api)
3. [Debug de Microserviços](#debug-de-microserviços)
4. [Técnicas Avançadas](#técnicas-avançadas)
5. [Troubleshooting Comum](#troubleshooting)

---

## ?? DEBUG BÁSICO (VISUAL STUDIO)

### 1. BREAKPOINTS (Pontos de Parada)

**O que é:** Um "ponto de parada" onde o código para de executar para você investigar.

**Como usar:**
1. Clique na margem esquerda da linha (bolinha vermelha aparece ??)
2. Execute em modo Debug (F5)
3. Quando chegar no breakpoint, a execução PARA

**Atalhos:**
- `F9` - Adicionar/Remover breakpoint na linha atual
- `Ctrl+Shift+F9` - Remover todos os breakpoints
- `F5` - Continuar execução
- `Shift+F5` - Parar debug

**Exemplo Prático:**
```csharp
// ProdutosApiController.cs
[HttpGet]
[Route("{id:int}", Name = "GetProdutoById")]
public async Task<IHttpActionResult> GetById(int id)
{
    // ?? COLOQUE UM BREAKPOINT AQUI (clique na margem)
    var produto = await _produtoService.ObterPorIdAsync(id);
    
    // Quando executar, vai parar aqui e você pode:
    // - Ver o valor de 'id'
    // - Ver se 'produto' veio null
    // - Inspecionar '_produtoService'
    
    var produtoDto = HateoasHelper.CreateProdutoDtoWithLinks(produto, Url);
    return Ok(produtoDto);
}
```

---

### 2. NAVEGAÇÃO NO DEBUG

**F10 - Step Over (Pular Por Cima)**
- Executa a linha atual e vai para a próxima
- NÃO entra dentro de métodos

```csharp
var produto = await _produtoService.ObterPorIdAsync(id); // ? Pressione F10
// Executa a linha inteira, não entra no método ObterPorIdAsync
```

**F11 - Step Into (Entrar Dentro)**
- Executa a linha atual E entra dentro do método

```csharp
var produto = await _produtoService.ObterPorIdAsync(id); // ? Pressione F11
// Vai ENTRAR no método ObterPorIdAsync para debugar linha por linha
```

**Shift+F11 - Step Out (Sair Fora)**
- Sai do método atual e volta para quem chamou

```csharp
// Você está dentro de ObterPorIdAsync
public async Task<Produto> ObterPorIdAsync(int id)
{
    var produto = await _produtoRepository.GetByIdAsync(id); // ? Pressione Shift+F11
    // Volta para o Controller
}
```

---

### 3. JANELAS DE DEBUG

**Locals (Variáveis Locais)**
- Mostra TODAS as variáveis no escopo atual
- Abrir: `Debug > Windows > Locals` ou `Ctrl+Alt+V, L`

**Watch (Observação)**
- Você ESCOLHE o que quer monitorar
- Abrir: `Debug > Windows > Watch > Watch 1` ou `Ctrl+Alt+W, 1`

**Call Stack (Pilha de Chamadas)**
- Mostra COMO chegou até aqui (caminho de execução)
- Abrir: `Debug > Windows > Call Stack` ou `Ctrl+Alt+C`

**Immediate Window (Janela Imediata)**
- Você pode EXECUTAR CÓDIGO durante o debug!
- Abrir: `Debug > Windows > Immediate` ou `Ctrl+Alt+I`

```csharp
// Exemplo de uso no Immediate Window:
? id                    // Mostra valor de 'id'
? produto.Nome          // Mostra nome do produto
? produto.Preco * 2     // Calcula preço dobrado
```

---

### 4. CONDITIONAL BREAKPOINTS (Avançado)

**Problema:** Você tem um loop de 1000 itens, mas só quer parar quando ID = 42

**Solução:** Breakpoint Condicional

**Como fazer:**
1. Clique com botão direito no breakpoint ??
2. Escolha "Conditions..."
3. Configure: `id == 42`

```csharp
foreach (var produto in produtos) // ? Breakpoint condicional aqui
{
    // Só vai parar quando produto.Id == 42
    ProcessarProduto(produto);
}
```

---

## ?? DEBUG DE API WEB

### CENÁRIO: Debugar requisição HTTP

**Passo a Passo:**

1. **Coloque breakpoint no controller**
```csharp
[HttpGet]
[Route("{id:int}")]
public async Task<IHttpActionResult> GetById(int id)
{
    // ?? BREAKPOINT AQUI
    var produto = await _produtoService.ObterPorIdAsync(id);
    return Ok(produto);
}
```

2. **Execute o projeto em Debug (F5)**

3. **Faça a requisição via:**
   - **Postman:** `GET http://localhost:PORT/api/produtos/1`
   - **Navegador:** `http://localhost:PORT/api/produtos/1`
   - **Swagger:** (vamos implementar depois)

4. **O código VAI PARAR no breakpoint!**

5. **Inspecione:**
   - `id` - Valor veio correto?
   - `Request` - Headers, body, etc
   - `_produtoService` - Está injetado?

---

### INSPECIONANDO REQUISIÇÃO HTTP

```csharp
[HttpPost]
[Route("")]
public async Task<IHttpActionResult> Create([FromBody] ProdutoCreateDto produtoDto)
{
    // ?? BREAKPOINT AQUI
    
    // No Watch, adicione:
    // - produtoDto          ? Ver se o JSON foi mapeado corretamente
    // - produtoDto.Nome     ? Valor específico
    // - Request.Content     ? Body da requisição
    // - Request.Headers     ? Headers HTTP
    
    var produto = produtoDto.ToEntity();
    await _produtoService.CriarAsync(produto);
    return Created(...);
}
```

---

## ?? DEBUG DE MICROSERVIÇOS

### CENÁRIO REAL: 2 APIs conversando

```
API Produtos (Porta 5000)
    ? chama
API Estoque (Porta 5001)
```

### ESTRATÉGIA 1: Debug de Múltiplas Instâncias

**Passo 1: Configurar múltiplos projetos**
1. Visual Studio ? Solution Explorer
2. Botão direito na Solution ? Properties
3. Startup Project ? Multiple startup projects
4. Marque ambos projetos como "Start"

**Passo 2: Debug simultâneo**
```csharp
// API Produtos (Porta 5000)
[HttpPost]
public async Task<IHttpActionResult> Create(ProdutoDto dto)
{
    // ?? BREAKPOINT 1
    var produto = await _produtoService.CriarAsync(dto);
    
    // Chama API Estoque
    await _estoqueHttpClient.ReservarEstoque(produto.Id); // ?? BREAKPOINT 2
    
    return Ok(produto);
}

// API Estoque (Porta 5001)
[HttpPost]
public async Task<IHttpActionResult> ReservarEstoque(int produtoId)
{
    // ?? BREAKPOINT 3 (vai parar aqui quando API Produtos chamar!)
    var estoque = await _estoqueService.Reservar(produtoId);
    return Ok(estoque);
}
```

**F5 ? Breakpoint 1 ? F5 ? Breakpoint 2 ? F5 ? Breakpoint 3**

---

### ESTRATÉGIA 2: Logging para Debug Distribuído

Quando você NÃO PODE debugar ao vivo (produção, múltiplos serviços):

```csharp
public async Task<IHttpActionResult> Create(ProdutoDto dto)
{
    var correlationId = Guid.NewGuid(); // ID único para rastrear
    
    _logger.LogInformation($"[{correlationId}] Iniciando criação de produto: {dto.Nome}");
    
    var produto = await _produtoService.CriarAsync(dto);
    _logger.LogInformation($"[{correlationId}] Produto criado: ID={produto.Id}");
    
    _logger.LogInformation($"[{correlationId}] Chamando API Estoque...");
    await _estoqueHttpClient.ReservarEstoque(produto.Id);
    _logger.LogInformation($"[{correlationId}] Estoque reservado com sucesso");
    
    return Ok(produto);
}
```

**No log você vai ver:**
```
[abc-123] Iniciando criação de produto: Notebook
[abc-123] Produto criado: ID=42
[abc-123] Chamando API Estoque...
[abc-123] Estoque reservado com sucesso
```

---

## ?? TÉCNICAS AVANÇADAS

### 1. TRACEPOINTS (Log sem parar execução)

**Problema:** Você quer logar, mas NÃO parar o código

**Solução:**
1. Botão direito no breakpoint
2. "Actions..."
3. Marque "Log a message to Output Window"
4. Digite: `Produto ID: {id}, Nome: {produto.Nome}`

**Resultado:** Aparece no Output sem parar!

---

### 2. DEBUG DE EXCEPTIONS

```csharp
public async Task<Produto> ObterPorIdAsync(int id)
{
    try
    {
        var produto = await _repository.GetByIdAsync(id);
        if (produto == null)
            throw new KeyNotFoundException($"Produto {id} não encontrado");
        
        return produto;
    }
    catch (Exception ex)
    {
        // ?? BREAKPOINT AQUI para investigar exceções
        _logger.LogError(ex, "Erro ao buscar produto {id}", id);
        throw;
    }
}
```

**Configurar Visual Studio para parar em exceções:**
- `Ctrl+Alt+E` ? Exception Settings
- Marque "Common Language Runtime Exceptions"

---

### 3. DEBUG DE CÓDIGO ASYNC/AWAIT

```csharp
public async Task<IHttpActionResult> GetAll()
{
    // ?? BREAKPOINT 1
    var produtos = await _produtoService.ObterTodosAsync(); // ?? vai "pular" aqui
    
    // ?? BREAKPOINT 2 (código continua aqui DEPOIS do await)
    var dtos = produtos.Select(p => p.ToDto()).ToList();
    
    return Ok(dtos);
}
```

**Importante:** O código pode "pular" entre threads com async!

---

## ?? TROUBLESHOOTING COMUM

### PROBLEMA 1: "Breakpoint não será atingido. Sem símbolos carregados."

**Causa:** Compilação em Release ou símbolos desatualizados

**Solução:**
1. Verificar `Web.config`: `<compilation debug="true" />`
2. Rebuild (Ctrl+Shift+B)
3. Clean Solution ? Rebuild Solution

---

### PROBLEMA 2: "API não responde quando debugando"

**Causa:** Timeout de requisição enquanto você está no breakpoint

**Solução:**
- No Postman: Aumentar timeout
- Ou: Pressione F5 rápido para não demorar

---

### PROBLEMA 3: "Não consigo ver valor de variável"

**Causa:** Otimização do compilador

**Solução:**
```xml
<!-- Web.config -->
<compilation debug="true" optimizeCompilations="false" />
```

---

## ?? CHECKLIST DE DEBUG

### Antes de debugar:
- [ ] Código compila sem erros?
- [ ] `debug="true"` no Web.config?
- [ ] Símbolos carregados? (rebuild se necessário)
- [ ] Breakpoints nos lugares certos?

### Durante o debug:
- [ ] Ver valores no Locals/Watch
- [ ] Verificar Call Stack (como chegou aqui?)
- [ ] Inspecionar exceções
- [ ] Usar Immediate Window para testar

### Debugging Microserviços:
- [ ] Correlation ID em todos os logs
- [ ] Múltiplos projetos rodando?
- [ ] Verificar ports/URLs corretos
- [ ] Timeout configurado?

---

## ?? EXERCÍCIOS PRÁTICOS

### Exercício 1: Debug Básico
1. Coloque breakpoint no `GetById` do controller
2. Execute (F5)
3. Chame `GET /api/produtos/1` no Postman
4. Use F10 para navegar linha por linha
5. Inspecione a variável `produto`

### Exercício 2: Conditional Breakpoint
1. Coloque breakpoint no loop de produtos
2. Configure condição: `produto.Preco > 1000`
3. Execute e veja parar apenas em produtos caros

### Exercício 3: Immediate Window
1. Pare em um breakpoint
2. Abra Immediate Window (Ctrl+Alt+I)
3. Digite: `? produto.Nome`
4. Digite: `? produto.Preco * 1.1` (calcula 10% aumento)

---

## ?? DICAS DE OURO

1. **Use Conditional Breakpoints** - Economiza tempo em loops
2. **Sempre veja o Call Stack** - Mostra o caminho de execução
3. **Log com Correlation ID** - Essencial para microserviços
4. **Tracepoints > Debug.WriteLine** - Mais profissional
5. **Aprenda atalhos** - F5, F10, F11 são seus melhores amigos

---

## ?? PRÓXIMO NÍVEL

Quando dominar isso, aprenda:
- [ ] Remote Debugging (debugar em servidor remoto)
- [ ] Performance Profiling (encontrar gargalos)
- [ ] Memory Profiling (vazamentos de memória)
- [ ] Distributed Tracing (Application Insights, Jaeger)

---

**Criado por:** Ericson Costa Soares  
**Data:** 2025  
**Projeto:** WebApplication1 - Enterprise API
