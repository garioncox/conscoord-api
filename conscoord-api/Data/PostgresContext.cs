using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace conscoord_api.Data;

public partial class PostgresContext : DbContext
{
    public PostgresContext(IOptions<CustomConfiguration> configuration)
    {
        _configuration = configuration.Value;
    }

    public PostgresContext(DbContextOptions<PostgresContext> options, IOptions<CustomConfiguration> configuration)
        : base(options)
    {
        _configuration = configuration.Value;
    }

    private CustomConfiguration _configuration;

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<CompanyProject> CompanyProjects { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeShift> EmployeeShifts { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectShift> ProjectShifts { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Shift> Shifts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.DB);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum("practicum2425", "status", new[] { "ACTIVE", "ARCHIVED", "COMPLETED" });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("company_pkey");

            entity.ToTable("company", "practicum2425");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<CompanyProject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("company_project_pkey");

            entity.ToTable("company_project", "practicum2425");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");

            entity.HasOne(d => d.Company).WithMany(p => p.CompanyProjects)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("company_project_company_id_fkey");

            entity.HasOne(d => d.Project).WithMany(p => p.CompanyProjects)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("company_project_project_id_fkey");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("employee_pkey");

            entity.ToTable("employee", "practicum2425");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(30)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Phonenumber)
                .HasMaxLength(13)
                .HasColumnName("phonenumber");
            entity.Property(e => e.Roleid).HasColumnName("roleid");

            entity.HasOne(d => d.Role).WithMany(p => p.Employees)
                .HasForeignKey(d => d.Roleid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("rolename");
        });

        modelBuilder.Entity<EmployeeShift>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("employee_shift_pkey");

            entity.ToTable("employee_shift", "practicum2425");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClockInTime)
                .HasMaxLength(20)
                .HasColumnName("clock_in_time");
            entity.Property(e => e.ClockOutTime)
                .HasMaxLength(20)
                .HasColumnName("clock_out_time");
            entity.Property(e => e.EmpId).HasColumnName("emp_id");
            entity.Property(e => e.ShiftId).HasColumnName("shift_id");

            entity.HasOne(d => d.Emp).WithMany(p => p.EmployeeShifts)
                .HasForeignKey(d => d.EmpId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("employee_shift_emp_id_fkey");

            entity.HasOne(d => d.Shift).WithMany(p => p.EmployeeShifts)
                .HasForeignKey(d => d.ShiftId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("employee_shift_shift_id_fkey");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("project_pkey");

            entity.ToTable("project", "practicum2425");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .HasColumnName("description");
            entity.Property(e => e.EndDate)
                .HasMaxLength(20)
                .HasColumnName("end_date");
            entity.Property(e => e.Location)
                .HasMaxLength(50)
                .HasColumnName("location");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.StartDate)
                .HasMaxLength(20)
                .HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(16)
                .HasColumnName("status");
        });

        modelBuilder.Entity<ProjectShift>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("project_shift_pkey");

            entity.ToTable("project_shift", "practicum2425");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.ShiftId).HasColumnName("shift_id");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectShifts)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("project_shift_project_id_fkey");

            entity.HasOne(d => d.Shift).WithMany(p => p.ProjectShifts)
                .HasForeignKey(d => d.ShiftId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("project_shift_shift_id_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("role_pkey");

            entity.ToTable("role", "practicum2425");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Rolename)
                .HasMaxLength(15)
                .HasColumnName("rolename");
        });

        modelBuilder.Entity<Shift>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("shift_pkey");

            entity.ToTable("shift", "practicum2425");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasColumnName("description");
            entity.Property(e => e.EndTime)
                .HasMaxLength(20)
                .HasColumnName("end_time");
            entity.Property(e => e.Location)
                .HasMaxLength(50)
                .HasColumnName("location");
            entity.Property(e => e.RequestedEmployees).HasColumnName("requested_employees");
            entity.Property(e => e.StartTime)
                .HasMaxLength(20)
                .HasColumnName("start_time");
            entity.Property(e => e.Status)
                .HasMaxLength(16)
                .HasColumnName("status");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}