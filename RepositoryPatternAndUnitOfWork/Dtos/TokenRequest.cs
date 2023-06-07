using System.ComponentModel.DataAnnotations;

namespace RepositoryPatternAndUnitOfWork.Dtos
{
    public class TokenRequest
    {
        [Required]
        public string? Token { get; set; }
        [Required]
        public string? RefreshToken { get; set; }
    }
}
