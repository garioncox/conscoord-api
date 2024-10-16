namespace conscoord_api.Data.Interfaces;

public interface ICompanyService
{
    public Task<List<Company>> GetCompanyListAsync();
}