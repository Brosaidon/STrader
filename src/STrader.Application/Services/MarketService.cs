using STrader.Application.Interfaces;
using STrader.Application.Models;
using STrader.Domain.Entities;

namespace STrader.Application.Services;

public class MarketService : IMarketService
{
    private readonly ISessionRepository _repository;

    public MarketService(ISessionRepository repository)
    {
        _repository = repository;
    }

    //READ ONLY
    public MarketStateDto GetMarket(SessionService session, List<PendingAction> pending)
    {
        return new MarketStateDto
        {
            Credits = ProjectionHelper.GetProjectedCredits(session, pending),
            CargoUsed = ProjectionHelper.GetProjectedCargoUsed(session, pending),
            CargoLeft = ProjectionHelper.GetProjectedCargoLeft(session, pending),

            Items = session.Market
            .Select(item => MapItem(session, item, pending))
            .ToList()
        };
    }

    // NO STATE MUTATION
    public void QueueAction(SessionService session, List<PendingAction> pending, MarketActionRequest request)
    {
        var action = new PendingAction
        {
            ItemId = request.ItemId,
            ActionType = request.ActionType,
            Quantity = request.Quantity
        };

        if (!ActionValidator.CanExecute(session, pending, action))
            return;

        pending.Add(action);
    }

    // The ONLY WRITE ENTRY POINT
    public void ExecuteTurn(SessionService session, List<PendingAction> pending)
    {
        foreach (var action in pending)
        {
            if (!ActionValidator.CanExecute(session, pending, action))
                continue;

            Apply(session, action);
        }
        pending.Clear();

        _repository.SaveSession(session); //<--- This, this is where we write to the database.
    }

    private static MarketItemDto MapItem(SessionService session, MarketItem item, List<PendingAction> pending)
    {
        var catalog = ItemCatalog.Items.First(i => i.Id == item.ItemId);

        var cargo = session.Cargo.FirstOrDefault(c => c.ItemId == item.ItemId);
        var inCargo = cargo?.Quantity ?? 0;

        var pendingbuy = pending
        .Where(a => a.ItemId == item.ItemId && a.ActionType == ActionType.Buy)
        .Sum(a => a.Quantity);

        return new MarketItemDto
        {
            ItemId = item.ItemId,
            Name = catalog.Name,
            Icon = catalog.Icon,
            Price = item.Price,

            Available = Math.Max(0, item.Available - pendingbuy + pendingSell), // Show reduced availability for pending buys
            InCargo = Math.Max(0, inCargo + pendingBuy - pendingSell)
        };
    }
    private static void Apply(SessionService session, PendingAction action)
    {
        var item = session.GetMarketItem(action.ItemId);
        if (item == null) return;

        switch (action.ActionType)
        {
            case ActionType.Buy:
                var cargo = session.GetCargoItem(action.ItemId);
                if (cargo == null)
                {
                    session.Cargo.Add(new CargoItem
                    {
                        ItemId = action.ItemId,
                        Quantity = action.Quantity
                    });
                }
                else
                {
                    cargo.Quantity += action.Quantity;
                }

                item.Available -= action.Quantity;
                session.Credits -= item.Price * action.Quantity;
                break;

            case ActionType.Sell:
                var cargoItem = session.GetCargoItem(action.ItemId);
                if (cargoItem == null) return;

                cargoItem.Quantity -= action.Quantity;
                item.Available += action.Quantity;
                session.Credits += item.Price * action.Quantity;
                break;
        }
    }
}

