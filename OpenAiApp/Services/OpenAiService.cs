using Microsoft.Extensions.Options;
using OpenAI_API.Models;
using OpenAiApp.Configurations;

namespace OpenAiApp.Services
{
    public class OpenAiService : IOpenAiService
    {
        private readonly OpenAiConfig _config;

        public OpenAiService(IOptionsMonitor<OpenAiConfig> optionsMonitor)
        {
            _config = optionsMonitor.CurrentValue;
        }

        public async Task<string> CompleteSentence(string text)
        {
            var api = new OpenAI_API.OpenAIAPI(_config.ApiKey);

            var result = await api.Completions.GetCompletion(text);

            return result;
        }

        public async Task<string> CompleteSentenceAdvance(string text)
        {
            var api = new OpenAI_API.OpenAIAPI(_config.ApiKey);

            var result = await api.Completions.CreateCompletionAsync(
                new OpenAI_API.Completions.CompletionRequest(text, model: Model.CurieText,
                temperature: 0.1));

            return result.Completions[0].Text;
        }

        public async Task<string> CheckProgrammingLanguage(string language)
        {
            var api = new OpenAI_API.OpenAIAPI(_config.ApiKey);

            var chat = api.Chat.CreateConversation();

            chat.AppendSystemMessage("help me");
            chat.AppendUserInput(language);

            var res = await chat.GetResponseFromChatbotAsync();

            return res;
        }
    }
}
