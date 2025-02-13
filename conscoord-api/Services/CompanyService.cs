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

    public async Task AddCompany(string companyName)
    {
        Company company = new Company()
        {
            Name = companyName,
        };

        _context.Companies.Add(company);
        await _context.SaveChangesAsync();
    }
}
