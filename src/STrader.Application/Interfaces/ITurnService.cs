using STrader.Application.Services;

public interface ITurnService
{
    void ExecuteTurn(SessionService session, PendingActionStore store);
}