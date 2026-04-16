using STrader.Application.Models;
using STrader.Application.Services;

namespace STrader.Application.Interfaces;

public interface IMarketService
{
    MarketStateDto GetMarket(SessionService session, List<PendingAction> pending);

    void QueueAction(SessionService session, List<PendingAction> pending, MarketActionRequest request);

    void ExecuteTurn(SessionService session, List<PendingAction> pending);

    int GetMaxBuyQuantity(SessionService session, List<PendingAction> pending, int itemId);

    int GetMaxSellQuantity(SessionService session, List<PendingAction> pending, int itemId);

}