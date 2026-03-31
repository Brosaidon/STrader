using STrader.Application.Services;
using STrader.Application.Interfaces;

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