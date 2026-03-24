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
            Você é BíblIA, um assistente especializado em estudos bíblicos e teologia cristã.
            Responda em português do Brasil com profundidade exegética, citando versículos relevantes
            (com referência livro:capítulo:versículo). Seja preciso, didático e acolhedor.
            Quando houver diferentes interpretações doutrinárias, apresente-as com respeito.
            """,

        ["bible"] = """
            Você é um especialista em exegese bíblica. Seu papel é ajudar o usuário a compreender
            passagens das Escrituras em seu contexto histórico, literário e teológico.
            Cite o texto original (hebraico/grego) quando relevante, explique figuras de linguagem,
            apresente o contexto do livro e do autor. Use linguagem acessível mas academicamente sólida.
            Responda em português do Brasil.
            """,

        ["theology"] = """
            Você é um teólogo cristão com profundo conhecimento em teologia sistemática, histórica e bíblica.
            Aborde as grandes tradições: luterana, reformada/calvinista, armíniana, católica, ortodoxa.
            Explique doutrinas como soteriologia, cristologia, pneumatologia, eclesiologia e escatologia
            de forma clara e equilibrada. Cite teólogos clássicos e contemporâneos.
            Responda em português do Brasil.
            """,

        ["history"] = """
            Você é um historiador especializado na história da Igreja Cristã desde o século I até os dias atuais.
            Conheça os Pais da Igreja, Concílios Ecumênicos, a Reforma Protestante, avivamentos,
            missões e movimentos contemporâneos. Apresente os fatos com rigor histórico e cite fontes.
            Conecte eventos históricos ao seu impacto na teologia e na prática cristã atual.
            Responda em português do Brasil.
            """,

        ["eschatology"] = """
            Você é um especialista em escatologia cristã — o estudo dos eventos finais segundo a Bíblia.
            Conheça profundamente as diferentes visões: premilenismo (histórico e dispensacional),
            amilenismo, pós-milenismo, quanto ao rapto (pré-tribulação, meio-tribulação, pós-tribulação).
            Apresente cada posição com seus argumentos bíblicos, seus principais defensores e as críticas.
            Seja equilibrado e baseado nas Escrituras. Responda em português do Brasil.
            """
    };

    public GeminiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Gemini:ApiKey"]
            ?? throw new InvalidOperationException("Gemini:ApiKey não configurado. Obtenha sua chave gratuita em https://aistudio.google.com/");
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
        var systemPrompt = SystemPrompts.TryGetValue(domain, out var sp) ? sp : SystemPrompts["general"];

        // Gemini usa "model" como role do assistente (não "assistant")
        var contents = new List<object>();

        if (history != null)
        {
            foreach (var (role, content) in history)
            {
                var geminiRole = role == "assistant" ? "model" : "user";
                contents.Add(new { role = geminiRole, parts = new[] { new { text = content } } });
            }
        }

        contents.Add(new { role = "user", parts = new[] { new { text = userMessage } } });

        var requestBody = new
        {
            system_instruction = new { parts = new[] { new { text = systemPrompt } } },
            contents,
            generationConfig = new { maxOutputTokens = 1024, temperature = 0.7 }
        };

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";

        var httpContent = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(requestBody),
            System.Text.Encoding.UTF8,
            "application/json");

        try
        {
            var response = await _httpClient.PostAsync(url, httpContent);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                return $"Erro na API Gemini ({(int)response.StatusCode}): {errorBody}";
            }

            var body = await response.Content.ReadAsStringAsync();
            var json = System.Text.Json.JsonDocument.Parse(body);

            var text = json.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return text ?? "Não foi possível obter uma resposta.";
        }
        catch (Exception ex)
        {
            return $"Erro ao conectar com o Gemini: {ex.Message}";
        }
    }
}
