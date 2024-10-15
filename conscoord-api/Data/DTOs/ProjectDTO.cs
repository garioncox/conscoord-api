namespace conscoord_api.Data.DTOs
{
    public class ProjectDTO
    {
        public required string Name { get; set; }
        public required string Location { get; set; }
        public required string StartDate { get; set; }
        public required string EndDate { get; set; }
    }
}
