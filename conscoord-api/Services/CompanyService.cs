using conscoord_api.Data;
using conscoord_api.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace conscoord_api.Services;

public class CompanyService : ICompanyService
{
    readonly PostgresContext _context;
    public CompanyService(PostgresContext context)
    {
        _context = context;
    }

    public async Task<List<Company>> GetCompanyListAsync()
    {
        return await _context.Companies.ToListAsync();
    }

    public async Task<int> AddCompany(string companyName)
    {
        Company company = new Company()
        {
            Name = companyName,
        };

        _context.Companies.Add(company);
        await _context.SaveChangesAsync();
        var NewCompany = await _context.Companies.FirstOrDefaultAsync(c => c.Name == companyName);
        return NewCompany!.Id;
    }

    public async Task<string?> GetCompanyNameByProjectIdAsync(int projectId)
    {
        try
        {
            var company = await _context.Companies
                .Include(c => c.CompanyProjects)
                .ThenInclude(cp => cp.Project)
                .Where(c => c.CompanyProjects.Any(cp => cp.Project.Id == projectId))
                .FirstAsync();

            return company.Name;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }
}
