namespace conscoord_api.Data.DTOs;
public class ProjectDetailsDTO
{
    public required Employee contactInfo;
    public required List<Shift> availableShifts;
    public required Company companyInfo;
}