using STrader.Application.Services;

namespace STrader.Application.Interfaces;

public interface ISessionRepository
{
    SessionService Load();
    void Save(SessionService session);
}