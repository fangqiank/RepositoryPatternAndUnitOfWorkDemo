namespace AutomapperDemo.Models.Dtos.Incoming
{
    public class CreateDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int DriverNumber { get; set; }
        public int WorldChampionship { get; set; }
    }
}
