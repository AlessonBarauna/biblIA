namespace BíblIA.Api.Services;

/// <summary>
/// Integração com a Groq API — gratuita, OpenAI-compatible, modelos LLaMA 3.
/// Chave gratuita em: https://console.groq.com/keys
/// Limites free tier: ~14 400 req/dia (llama3-8b), sem cartão de crédito.
///
/// Por que Groq em vez de Gemini?
/// - Formato OpenAI-compatible: mais simples e bem documentado
/// - Latência extremamente baixa (roda em hardware especializado)
/// - Sem necessidade de lidar com tipos de conteúdo específicos do Gemini
/// </summary>
public class GroqService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;

    private static readonly Dictionary<string, string> SystemPrompts = new()
    {
        ["general"] = """
            Você é BíblIA, um assistente especializado em estudos bíblicos e teologia cristã.
            Responda em português do Brasil com profundidade, citando versículos relevantes
            (formato: Livro capítulo:versículo). Seja preciso, didático e acolhedor.
            Quando houver diferentes interpretações doutrinárias, apresente-as com equilíbrio.
            """,

        ["bible"] = """
            Você é um especialista em exegese bíblica. Ajude o usuário a compreender
            passagens das Escrituras em seu contexto histórico, literário e teológico.
            Cite o texto original (hebraico/grego) quando relevante, explique figuras de
            linguagem e apresente o contexto do livro e do autor.
            Responda em português do Brasil.
            """,

        ["theology"] = """
            Você é um teólogo cristão com profundo conhecimento em teologia sistemática,
            histórica e bíblica. Aborde as grandes tradições: luterana, reformada/calvinista,
            armíniana, católica e ortodoxa. Explique doutrinas (soteriologia, cristologia,
            pneumatologia, eclesiologia) de forma clara e equilibrada, citando teólogos clássicos.
            Responda em português do Brasil.
            """,

        ["history"] = """
            Você é um historiador especializado na história da Igreja Cristã, do século I até hoje.
            Conheça os Pais da Igreja, Concílios Ecumênicos, a Reforma Protestante, avivamentos
            e missões. Apresente os fatos com rigor histórico e conecte-os ao seu impacto na
            teologia e prática cristã atual. Responda em português do Brasil.
            """,

        ["eschatology"] = """
            Você é um especialista em escatologia cristã — o estudo dos eventos finais segundo a Bíblia.
            Domine as diferentes visões: premilenismo (histórico e dispensacional), amilenismo,
            pós-milenismo; e sobre o rapto: pré-tribulação, meio-tribulação, pós-tribulação.
            Apresente cada posição com argumentos bíblicos e seus principais defensores.
            Seja equilibrado e baseado nas Escrituras. Responda em português do Brasil.
            """
    };

    public GroqService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Groq:ApiKey"] ?? string.Empty;
        _model   = configuration["Groq:Model"]  ?? "llama-3.3-70b-versatile";
    }

    /// <summary>Pergunta pontual sem histórico — usada pelos painéis das abas.</summary>
    public Task<string> AskAsync(string question, string domain = "general")
        => CallGroqAsync(question, domain, history: null);

    /// <summary>Conversa com histórico completo — usada pelo ChatsController.</summary>
    public Task<string> SendMessageAsync(string userMessage, List<(string role, string content)> history)
        => CallGroqAsync(userMessage, "general", history);

    private async Task<string> CallGroqAsync(
        string userMessage,
        string domain,
        List<(string role, string content)>? history)
    {
        if (string.IsNullOrWhiteSpace(_apiKey) || _apiKey.StartsWith("COLE_"))
            return "Chave Groq não configurada. Adicione em appsettings.Development.json → \"Groq:ApiKey\". Chave gratuita: https://console.groq.com/keys";

        try
        {
            var systemPrompt = SystemPrompts.TryGetValue(domain, out var sp) ? sp : SystemPrompts["general"];

            // Formato OpenAI-compatible: array de messages com roles system/user/assistant
            var messages = new System.Text.Json.Nodes.JsonArray();
            messages.Add(System.Text.Json.Nodes.JsonNode.Parse(
                $"{{\"role\":\"system\",\"content\":{System.Text.Json.JsonSerializer.Serialize(systemPrompt)}}}"));

            if (history != null)
            {
                foreach (var (role, content) in history)
                    messages.Add(System.Text.Json.Nodes.JsonNode.Parse(
                        $"{{\"role\":\"{role}\",\"content\":{System.Text.Json.JsonSerializer.Serialize(content)}}}"));
            }

            messages.Add(System.Text.Json.Nodes.JsonNode.Parse(
                $"{{\"role\":\"user\",\"content\":{System.Text.Json.JsonSerializer.Serialize(userMessage)}}}"));

            var body = new System.Text.Json.Nodes.JsonObject
            {
                ["model"]      = _model,
                ["messages"]   = messages,
                ["max_tokens"] = 1024,
                ["temperature"] = 0.7
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions");
            request.Headers.Add("Authorization", $"Bearer {_apiKey}");
            request.Content = new StringContent(body.ToJsonString(), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    var errJson = System.Text.Json.JsonDocument.Parse(responseBody);
                    var msg = errJson.RootElement.GetProperty("error").GetProperty("message").GetString();
                    return $"Groq API error ({(int)response.StatusCode}): {msg}";
                }
                catch
                {
                    return $"Groq API error ({(int)response.StatusCode}): {responseBody}";
                }
            }

            var json = System.Text.Json.JsonDocument.Parse(responseBody);
            return json.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString()
                ?? "Resposta vazia.";
        }
        catch (Exception ex)
        {
            return $"Erro ao chamar Groq: {ex.GetType().Name} — {ex.Message}";
        }
    }
}
