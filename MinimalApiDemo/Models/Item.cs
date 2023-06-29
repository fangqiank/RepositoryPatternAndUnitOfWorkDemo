namespace MinimalApiDemo.Models
{
    public record Item
    {
        public int Id { get; set; }
        public string Todo { get; set; } = string.Empty;
        public bool Completed { get; init; } 
    }
}
