namespace MinimalApiDemo.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Todo { get; set; } = string.Empty;
        public bool Completed { get; set; }
    }
}
