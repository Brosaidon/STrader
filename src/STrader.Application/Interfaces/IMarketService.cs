using STrader.Application.Models;
using STrader.Application.Services;

namespace STrader.Application.Interfaces;

public interface IMarketService
{
    MarketStateDto GetMarket(SessionService session, IReadOnlyDictionary<int, int> pending);

    void ApplyNet(SessionService session, IReadOnlyDictionary<int, int> pending);

    int GetMaxBuyQuantity(SessionService session, IReadOnlyDictionary<int, int> pending, int itemId);

    int GetMaxSellQuantity(SessionService session, IReadOnlyDictionary<int, int> pending, int itemId);

}