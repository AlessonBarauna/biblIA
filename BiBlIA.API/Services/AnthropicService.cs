namespace BíblIA.Api.Services;

public class AnthropicService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly string _apiKey;
    private readonly string _model;

    public AnthropicService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _apiKey = configuration["Anthropic:ApiKey"] ?? throw new InvalidOperationException("Anthropic API Key not configured");
        _model = configuration["Anthropic:Model"] ?? "claude-3-5-haiku-20241022";
    }

    public async Task<string> SendMessageAsync(string userMessage, List<(string role, string content)> conversationHistory)
    {
        var messages = new List<object>();
        
        // Adicionar histórico de conversa
        foreach (var (role, messageContent) in conversationHistory)
        {
            messages.Add(new { role, content = messageContent });
        }
        
        // Adicionar mensagem atual
        messages.Add(new { role = "user", content = userMessage });

        var request = new
        {
            model = _model,
            max_tokens = 1024,
            messages = messages,
            system = "Você é um assistente especializado em teologia bíblica e escatologia. Responda com profundidade e baseado em escrituras."
        };

        var httpContent = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(request),
            System.Text.Encoding.UTF8,
            "application/json");

        _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
        _httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

        try
        {
            var response = await _httpClient.PostAsync("https://api.anthropic.com/v1/messages", httpContent);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var jsonResponse = System.Text.Json.JsonDocument.Parse(responseBody);
            
            var assistantMessage = jsonResponse
                .RootElement
                .GetProperty("content")[0]
                .GetProperty("text")
                .GetString();

            return assistantMessage ?? "Desculpe, não consegui processar sua mensagem.";
        }
        catch (HttpRequestException ex)
        {
            return $"Erro ao conectar com a API: {ex.Message}";
        }
    }
}