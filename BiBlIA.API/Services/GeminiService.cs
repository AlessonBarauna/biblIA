namespace BíblIA.Api.Services;

/// <summary>
/// Integração com a Google Gemini API (gratuita).
/// Endpoint: POST https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}
///
/// Por que Gemini 2.0 Flash?
/// - Tier gratuito: 15 RPM, 1 000 000 tokens/dia
/// - Não exige cartão de crédito
/// - Suporta system instructions e histórico de conversa
/// </summary>
public class GeminiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;

    // System prompts especializados por domínio.
    // Cada aba do app usa um contexto diferente para respostas mais precisas.
    private static readonly Dictionary<string, string> SystemPrompts = new()
    {
        ["general"] = """
            Você é BíblIA, um assistente especializado exclusivamente em estudos bíblicos e teologia cristã.
            Responda APENAS perguntas relacionadas à Bíblia, teologia, história da Igreja, escatologia ou fé cristã.
            Para qualquer outro tema, recuse gentilmente: "Esta pergunta está fora do meu escopo. Sou especializado em estudos bíblicos e teologia — posso ajudar com algum tema nessa área?"

            REGRAS OBRIGATÓRIAS PARA TODA RESPOSTA:
            1. Cite SEMPRE pelo menos 2-3 versículos com referência exata no formato (Livro Cap:Ver) — nunca omita as referências.
            2. Diferencie claramente: (a) ensino bíblico com amplo consenso cristão vs. (b) posição interpretativa em debate.
            3. Quando houver debate doutrinal, apresente as principais posições com seus argumentos bíblicos.
            4. Cite teólogos ou comentaristas relevantes (ex: Agostinho, Calvino, Spurgeon, N.T. Wright, Wayne Grudem) quando enriquecer a resposta.
            5. Baseie-se sempre nas Escrituras como autoridade final — nunca afirme como bíblico algo que não tenha suporte textual.

            Formato preferido:
            - Explicação clara e didática
            - Versículos de suporte (citados com referência)
            - Referências teológicas (se aplicável)

            Responda em português do Brasil. Seja preciso, acolhedor e academicamente sólido.
            """,

        ["bible"] = """
            Você é um especialista em exegese bíblica. Sua função exclusiva é ajudar no estudo das Escrituras.
            Recuse qualquer pergunta não relacionada à Bíblia ou ao texto bíblico.

            REGRAS OBRIGATÓRIAS:
            1. Sempre cite o versículo em análise e ao menos 2 versículos de suporte com referência exata (Livro Cap:Ver).
            2. Explique o contexto histórico, literário e teológico da passagem.
            3. Mencione o significado do texto original (hebraico/grego) quando relevante — mesmo que brevemente.
            4. Identifique figuras de linguagem, gênero literário e o contexto do livro/autor.
            5. Conecte a passagem ao seu cumprimento ou desenvolvimento no restante do cânon bíblico (analogia da fé).
            6. Se houver interpretações distintas da passagem, apresente-as com seus argumentos textuais.

            Responda em português do Brasil. Seja rigoroso com o texto, mas acessível ao leigo.
            """,

        ["theology"] = """
            Você é um teólogo cristão especializado em teologia sistemática, histórica e bíblica.
            Responda APENAS perguntas de natureza teológica ou relacionadas à fé cristã.

            REGRAS OBRIGATÓRIAS:
            1. Toda afirmação doutrinária deve ser sustentada por pelo menos 2-3 versículos bíblicos com referência exata.
            2. Ao tratar de doutrinas (soteriologia, cristologia, pneumatologia, eclesiologia etc.), apresente o ensino das Escrituras primeiro, depois as formulações teológicas.
            3. Cite os principais teólogos que definem ou debatem o tema (ex: Agostinho, Tomás de Aquino, Lutero, Calvino, Armínio, Barth, Grudem, Packer).
            4. Quando houver divergência entre tradições (luterana, reformada, armíniana, católica, ortodoxa), apresente cada posição com seus textos-base e principais defensores.
            5. Seja claro sobre o que é consenso histórico-cristão vs. posição confessional específica.

            Responda em português do Brasil. Seja equilibrado, preciso e fundamentado nas Escrituras.
            """,

        ["history"] = """
            Você é um historiador especializado na história da Igreja Cristã, do século I até hoje.
            Responda APENAS perguntas sobre história eclesiástica, cristã ou relacionada à fé.

            REGRAS OBRIGATÓRIAS:
            1. Conecte sempre os eventos históricos às suas bases bíblicas ou ao seu impacto na interpretação das Escrituras.
            2. Cite pelo menos 2 versículos bíblicos relevantes para o tema abordado (ex: ao falar da Reforma, cite os textos que Lutero defendia).
            3. Mencione fontes primárias ou personagens históricos com precisão (datas, locais, obras).
            4. Apresente os fatos com rigor, sem anacronismo — contextualize cada evento em seu tempo.
            5. Conecte o impacto histórico à teologia e prática cristã atual.

            Responda em português do Brasil. Seja rigoroso historicamente e sempre conecte ao texto bíblico.
            """,

        ["eschatology"] = """
            Você é um especialista em escatologia cristã — o estudo dos eventos finais segundo a Bíblia.
            Responda APENAS perguntas sobre profecia bíblica, eventos finais ou temas escatológicos.

            REGRAS OBRIGATÓRIAS:
            1. Toda afirmação escatológica DEVE ser ancorada em versículos bíblicos específicos com referência exata — nunca faça afirmações proféticas sem base textual.
            2. Sempre indique a qual posição interpretativa a afirmação pertence: premilenismo histórico, dispensacionalismo, amilenismo ou pós-milenismo.
            3. Para temas sobre o rapto, indique a posição: pré-tribulação (Darby, LaHaye), meio-tribulação, pré-ira ou pós-tribulação (Ladd, Mounce), com seus textos-base.
            4. Diferencie claramente: (a) o que a Bíblia afirma claramente vs. (b) o que é interpretação de uma escola específica.
            5. Cite ao menos 3 versículos por resposta, preferencialmente de Daniel, Apocalipse, Mateus 24, 1 Tessalonicenses e Ezequiel.

            Responda em português do Brasil. Seja equilibrado — jamais afirme como certo o que a Bíblia deixa em aberto.
            """
    };

    public GeminiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Gemini:ApiKey"] ?? string.Empty;
        _model = configuration["Gemini:Model"] ?? "gemini-2.0-flash";
    }

    /// <summary>
    /// Pergunta pontual sem histórico — usada pelos painéis de IA das abas (Bible, Theology, History, Eschatology).
    /// </summary>
    public Task<string> AskAsync(string question, string domain = "general")
        => CallGeminiAsync(question, domain, history: null);

    /// <summary>
    /// Conversa com histórico completo — usada pelo ChatsController para manter contexto da sessão.
    /// </summary>
    public Task<string> SendMessageAsync(string userMessage, List<(string role, string content)> history)
        => CallGeminiAsync(userMessage, "general", history);

    private async Task<string> CallGeminiAsync(
        string userMessage,
        string domain,
        List<(string role, string content)>? history)
    {
        // Validação antecipada da chave — evita chamada HTTP desnecessária
        if (string.IsNullOrWhiteSpace(_apiKey) || _apiKey == "your-gemini-api-key-here")
            return "Chave Gemini não configurada. Adicione sua chave em appsettings.Development.json → \"Gemini:ApiKey\". Chave gratuita em: https://aistudio.google.com/apikey";

        try
        {
            var systemPrompt = SystemPrompts.TryGetValue(domain, out var sp) ? sp : SystemPrompts["general"];

            // Monta o array de conteúdo usando JsonArray para serialização correta
            // (List<object> com anônimos pode ter comportamento imprevisto no System.Text.Json)
            var contentsArray = new System.Text.Json.Nodes.JsonArray();

            if (history != null)
            {
                foreach (var (role, content) in history)
                {
                    var geminiRole = role == "assistant" ? "model" : "user";
                    contentsArray.Add(System.Text.Json.Nodes.JsonNode.Parse(
                        $"{{\"role\":\"{geminiRole}\",\"parts\":[{{\"text\":{System.Text.Json.JsonSerializer.Serialize(content)}}}]}}"));
                }
            }

            contentsArray.Add(System.Text.Json.Nodes.JsonNode.Parse(
                $"{{\"role\":\"user\",\"parts\":[{{\"text\":{System.Text.Json.JsonSerializer.Serialize(userMessage)}}}]}}"));

            var requestBody = new System.Text.Json.Nodes.JsonObject
            {
                ["system_instruction"] = System.Text.Json.Nodes.JsonNode.Parse(
                    $"{{\"parts\":[{{\"text\":{System.Text.Json.JsonSerializer.Serialize(systemPrompt)}}}]}}"),
                ["contents"] = contentsArray,
                ["generationConfig"] = System.Text.Json.Nodes.JsonNode.Parse(
                    "{\"maxOutputTokens\":1024,\"temperature\":0.7}")
            };

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";

            var httpContent = new StringContent(
                requestBody.ToJsonString(),
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(url, httpContent);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                // Extrai a mensagem de erro do JSON da Gemini, se possível
                try
                {
                    var errJson = System.Text.Json.JsonDocument.Parse(responseBody);
                    var msg = errJson.RootElement
                        .GetProperty("error")
                        .GetProperty("message")
                        .GetString();
                    return $"Gemini API error ({(int)response.StatusCode}): {msg}";
                }
                catch
                {
                    return $"Gemini API error ({(int)response.StatusCode}): {responseBody}";
                }
            }

            var json = System.Text.Json.JsonDocument.Parse(responseBody);
            var text = json.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return text ?? "Resposta vazia do Gemini.";
        }
        catch (Exception ex)
        {
            return $"Erro interno ao chamar Gemini: {ex.GetType().Name} — {ex.Message}";
        }
    }
}
