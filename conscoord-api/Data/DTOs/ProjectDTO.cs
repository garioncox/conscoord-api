namespace conscoord_api.Data.DTOs
{
    public class ProjectDTO
    {
        public required string Name { get; set; }
        public required string Location { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public int? Contactinfo { get; set; }
    }
}
