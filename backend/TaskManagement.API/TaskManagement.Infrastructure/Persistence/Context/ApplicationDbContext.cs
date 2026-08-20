using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Identity;

namespace TaskManagement.Infrastructure.Persistence.Contexts;

public class ApplicationDbContext
    : IdentityDbContext<ApplicationUser, ApplicationRole, string>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();

    public DbSet<TaskCategory> TaskCategories => Set<TaskCategory>();

    public DbSet<UserSession> UserSessions => Set<UserSession>();

    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();

    IQueryable<TaskItem> IApplicationDbContext.TaskItems => TaskItems;
    IQueryable<TaskCategory> IApplicationDbContext.TaskCategories => TaskCategories;
    IQueryable<ApplicationUser> IApplicationDbContext.Users => Users;

    void IApplicationDbContext.Add<T>(T entity) => Set<T>().Add(entity);

    Task<int> IApplicationDbContext.SaveChangesAsync(CancellationToken cancellationToken)
        => base.SaveChangesAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Future entity configurations will be added here.
        // builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}