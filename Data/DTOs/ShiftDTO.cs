namespace conscoord_api.Data.DTOs
{
    public class ShiftDTO
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public int RequestedEmployees { get; set; }
        public string Status { get; set; }
    }
}
