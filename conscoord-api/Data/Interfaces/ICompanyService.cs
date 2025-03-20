namespace conscoord_api.Data.Interfaces;

public interface ICompanyService
{
    public Task<List<Company>> GetCompanyListAsync();
    public Task<int> AddCompany(string companyName);
    public Task<string?> GetCompanyNameByProjectIdAsync(int projectId);
}
