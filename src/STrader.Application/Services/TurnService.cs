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
        var pending = store.();

        // 1. Apply market changes
        _market.ApplyNet(session, pending);

        // 2. Future: crew effects
        // _crew.Apply(session);

        // 3. Future: random events
        // _events.Resolve(session);

        // 4. Future: market recalibration
        // _market.UpdatePrices(session);

        // 5. Persist state
        _repository.Save(session);

        // 6. Clear pending actions
        store.Clear();
    }
}