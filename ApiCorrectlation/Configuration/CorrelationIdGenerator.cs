using ApiCorrectlation.Configuration.Interface;

namespace ApiCorrectlation.Configuration
{
    public class CorrelationIdGenerator : ICorrelationIdGenerator
    {
        private string _correlationId = Guid.NewGuid().ToString();
        public CorrelationIdGenerator()
        {
            
        }
        public string Get() => _correlationId;
        

        public void Set(string correlationId)
        {
            _correlationId = correlationId;
        }
    }
}
