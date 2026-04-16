using STrader.Application.Interfaces;
using STrader.Application.Models;
using STrader.Domain.Entities;
using STrader.Application.Validation;

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
            Credits = GetProjectedCredits(session, pending),
            CargoUsed = GetProjectedCargoUsed(session, pending),
            CargoLeft = GetProjectedCargoLeft(session, pending),


            Items = session.Market
            .Select(item => MapItem(session, item, pending))
            .ToList()

        };
    }

    // NO STATE MUTATION
    public void QueueAction(SessionService session, List<PendingAction> actions, MarketActionRequest request)
    {
        var item = session.Market.First(i => i.ItemId == request.ItemId);

        int validQuantity = request.ActionType switch
        {
            ActionType.Buy => ActionValidator.ClampBuyQuantity(session, actions, item, request.Quantity),
            ActionType.Sell => ActionValidator.ClampSellQuantity(session, actions, request.ItemId, request.Quantity),
            _ => 0
        };

        if (validQuantity <= 0)
            return;

        actions.Add(new PendingAction
        {
            ItemId = request.ItemId,
            ActionType = request.ActionType,
            Quantity = validQuantity
        });
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

        _repository.Save(session); //<--- This, this is where we write to the database.
    }

    private static MarketItemDto MapItem(
        SessionService session,
        MarketItem item,
        List<PendingAction> pending)
    {
        var catalog = ItemCatalog.Items.First(i => i.Id == item.ItemId);

        var cargo = session.Cargo.FirstOrDefault(c => c.ItemId == item.ItemId);
        var inCargo = cargo?.Quantity ?? 0;

        // ✅ BOTH directions must exist
        var pendingBuy = pending
            .Where(a => a.ItemId == item.ItemId && a.ActionType == ActionType.Buy)
            .Sum(a => a.Quantity);

        var pendingSell = pending
            .Where(a => a.ItemId == item.ItemId && a.ActionType == ActionType.Sell)
            .Sum(a => a.Quantity);

        // ✅ Projection math (same everywhere in system)
        var projectedAvailable = item.Available - pendingBuy + pendingSell;
        var projectedInCargo = inCargo + pendingBuy - pendingSell;

        return new MarketItemDto
        {
            ItemId = item.ItemId,
            Name = catalog.Name,
            Icon = catalog.Icon,
            Price = item.Price,

            // 👉 UI should use these (projected state)
            Available = Math.Max(0, projectedAvailable),
            InCargo = Math.Max(0, projectedInCargo)
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

    private static int GetProjectedCredits(SessionService session, List<PendingAction> pending)
    {
        var marketById = session.Market.ToDictionary(m => m.ItemId);

        return session.Credits - pending.Sum(a =>
        {
            if (!marketById.TryGetValue(a.ItemId, out var item))
                return 0;
            return a.ActionType switch
            {
                ActionType.Buy => item.Price * a.Quantity,
                ActionType.Sell => -item.Price * a.Quantity,
                _ => 0
            };
        });
    }

    private static int GetProjectedCargoUsed(SessionService session, List<PendingAction> pending)
    {
        var baseCargo = session.Cargo.Sum(c => c.Quantity);

        var pendingDelta = pending.Sum(a =>
        {
            return a.ActionType switch
            {
                ActionType.Buy => a.Quantity,
                ActionType.Sell => -a.Quantity,
                _ => 0
            };
        });

        return Math.Max(0, baseCargo + pendingDelta);
    }

    private static int GetProjectedCargoLeft(SessionService session, List<PendingAction> pending)
    {
        return Math.Max(0, session.CargoSpace - GetProjectedCargoUsed(session, pending));
    }

    public int GetMaxBuyQuantity(SessionService session, List<PendingAction> pending, int itemId)
    {
        var item = session.GetMarketItem(itemId);
        if (item == null || item.Price <= 0) return 0;

        var credits = GetProjectedCredits(session, pending);
        var cargoLeft = GetProjectedCargoLeft(session, pending);

        var affordableByCredits = credits / item.Price;
        var affordableByCargo = cargoLeft;

        return Math.Max(0, Math.Min(item.Available, Math.Min(affordableByCredits, affordableByCargo)));
    }

    public int GetMaxSellQuantity(SessionService session, List<PendingAction> pending, int itemId)
    {
        var cargo = session.GetCargoItem(itemId);
        var inCargo = cargo?.Quantity ?? 0;

        var pendingBuy = pending
            .Where(a => a.ItemId == itemId && a.ActionType == ActionType.Buy)
            .Sum(a => a.Quantity);

        var pendingSell = pending
            .Where(a => a.ItemId == itemId && a.ActionType == ActionType.Sell)
            .Sum(a => a.Quantity);

        return Math.Max(0, inCargo + pendingBuy - pendingSell);
    }
}

