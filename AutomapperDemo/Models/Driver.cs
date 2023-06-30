namespace AutomapperDemo.Models
{
    public class Driver
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int DriverNumber { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateUpdated { get; set; }
        public int Status { get; set; }
        public int WorldChampionship { get; set; }
    }
}
