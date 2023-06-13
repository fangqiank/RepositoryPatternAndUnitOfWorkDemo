namespace OpenAiApp.Services
{
    public interface IOpenAiService
    {
        Task<string> CheckProgrammingLanguage(string language);
        Task<string> CompleteSentence(string text);
        Task<string> CompleteSentenceAdvance(string text);
    }
}