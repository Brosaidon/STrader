using STrader.Application.Interfaces;

namespace STrader.Application.Services;

public class TurnService : ITurnService
{
    private readonly IMarketService _market;
    private readonly ISessionRepository _repository;

    public TurnService(
        IMarketService market,
        ISessionRepository repository)
    {
        _market = market;
        _repository = repository;
    }

    public void ExecuteTurn(SessionService session, PendingActionStore store)
    {
        var tradeDeltas = store.Net;

        // 1. Commit all staged trades (materialize optimistic actions)
        _market.ApplyNet(session, tradeDeltas);

        // 2. Resolve turn-based systems (future extensions)
        // _crew.Apply(session);
        // _events.Resolve(session);

        // 3. Recalculate market for new location (future)
        // _market.UpdatePrices(session);

        // 4. Persist the updated session
        _repository.Save(session);

        // 5. Clear staged actions for next turn
        store.Clear();
    }
}