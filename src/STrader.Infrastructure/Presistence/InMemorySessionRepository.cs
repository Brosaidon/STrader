using STrader.Application.Interfaces;
using STrader.Application.Services;

namespace STrader.Infrastructure.Persistence;

public class InMemorySessionRepository : ISessionRepository
{
    private SessionService _session = new();

    public SessionService Load() => _session;

    public void Save(SessionService session)
    {
        _session = session;
    }
}