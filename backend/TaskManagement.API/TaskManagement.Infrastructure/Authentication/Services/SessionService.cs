using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Domain.Identity;
using TaskManagement.Infrastructure.Persistence.Contexts;

namespace TaskManagement.Infrastructure.Authentication.Services;

public class SessionService : ISessionService
{
    private readonly ApplicationDbContext _context;

    public SessionService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<UserSession> CreateAsync(UserSession session)
    {
        session.Id = Guid.NewGuid();
        session.CreatedAt = DateTime.UtcNow;
        session.LastActivityAt = DateTime.UtcNow;
        session.IsActive = true;

        _context.UserSessions.Add(session);

        await _context.SaveChangesAsync();

        return session;
    }
    public async Task UpdateLastActivityAsync(Guid sessionId)
    {
        var session = await _context.UserSessions
            .FirstOrDefaultAsync(x => x.Id == sessionId);

        if (session == null)
            return;

        session.LastActivityAt = DateTime.UtcNow;

        _context.UserSessions.Update(session);

        await _context.SaveChangesAsync();
    }



    public async Task<List<UserSession>> GetActiveSessionsAsync(string userId)
    {
        return await _context.UserSessions
            .Where(x =>
                x.ApplicationUserId == userId &&
                x.IsActive &&
                x.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(x => x.LastActivityAt)
            .ToListAsync();
    }

    public async Task TerminateSessionAsync(Guid sessionId)
    {
        var session = await _context.UserSessions
            .FirstOrDefaultAsync(x => x.Id == sessionId);

        if (session == null)
            return;

        session.IsActive = false;
        session.LoggedOutAt = DateTime.UtcNow;

        _context.UserSessions.Update(session);

        await _context.SaveChangesAsync();
    }

    public async Task TerminateAllSessionsAsync(string userId)
    {
        var sessions = await _context.UserSessions
            .Where(x =>
                x.ApplicationUserId == userId &&
                x.IsActive)
            .ToListAsync();

        foreach (var session in sessions)
        {
            session.IsActive = false;
            session.LoggedOutAt = DateTime.UtcNow;
        }

        _context.UserSessions.UpdateRange(sessions);

        await _context.SaveChangesAsync();
    }

    public async Task RemoveExpiredSessionsAsync()
    {
        var expiredSessions = await _context.UserSessions
            .Where(x =>
                x.ExpiresAt <= DateTime.UtcNow ||
                !x.IsActive)
            .ToListAsync();

        if (!expiredSessions.Any())
            return;

        _context.UserSessions.RemoveRange(expiredSessions);

        await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync(UserSession session)
    {
        _context.UserSessions.Update(session);

        await _context.SaveChangesAsync();
    }

    public async Task<UserSession?> GetByIdAsync(Guid sessionId)
    {
        return await _context.UserSessions
            .Include(x => x.RefreshToken)
            .FirstOrDefaultAsync(x => x.Id == sessionId);
    }
}


//using TaskManagement.Application.Common.Interfaces;
//using TaskManagement.Domain.Identity;

//namespace TaskManagement.Infrastructure.Authentication.Services;

//public class SessionService : ISessionService
//{
//    public Task<UserSession> CreateAsync(UserSession session)
//    {
//        throw new NotImplementedException();
//    }

//    public Task UpdateLastActivityAsync(Guid sessionId)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<List<UserSession>> GetActiveSessionsAsync(string userId)
//    {
//        throw new NotImplementedException();
//    }

//    public Task TerminateSessionAsync(Guid sessionId)
//    {
//        throw new NotImplementedException();
//    }

//    public Task TerminateAllSessionsAsync(string userId)
//    {
//        throw new NotImplementedException();
//    }

//    public Task RemoveExpiredSessionsAsync()
//    {
//        throw new NotImplementedException();
//    }
//}
