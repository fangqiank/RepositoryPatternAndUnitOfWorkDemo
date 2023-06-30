namespace AutomapperDemo.Models.Dtos.Outgoing
{
    public class DriverDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int DriverNumber { get; set; }
        public int WorldChampionship { get; set; }
    }
}
