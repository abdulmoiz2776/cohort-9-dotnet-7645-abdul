using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Identity;

namespace TaskManagement.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    IQueryable<TaskItem> TaskItems { get; }

    IQueryable<TaskCategory> TaskCategories { get; }

    IQueryable<ApplicationUser> Users { get; }

    void Add<T>(T entity) where T : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
