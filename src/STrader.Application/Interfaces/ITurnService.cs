using STrader.Application.Services;

public interface ITurnService
{
    void ExecuteTurn(SessionService session, NetTradeStore store);
}