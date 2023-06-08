namespace ApiCorrectlation.Configuration.Interface
{
    public interface ICorrelationIdGenerator
    {
        string Get();
        void Set(string correlationId);
    }
}
