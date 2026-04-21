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
    public MarketStateDto GetMarket(
        SessionService session,
        IReadOnlyDictionary<int, int> pending)
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

    private MarketItemDto MapItem(
        SessionService session,
        MarketItem item,
        IReadOnlyDictionary<int, int> pending)
    {
        var def = session.GetDefinition(item.ItemId);
        var net = pending.GetValueOrDefault(item.ItemId);

        var cargo = session.Cargo
        .FirstOrDefault(c => c.ItemId == item.ItemId)?.Quantity ?? 0;

        var projectedCargo = cargo + net;
        var projectedAvailable = item.Available - net;

        var maxBuy = GetMaxBuyQuantity(session, pending, item.ItemId);
        var maxSell = GetMaxSellQuantity(session, pending, item.ItemId);

        return new MarketItemDto
        {
            ItemId = item.ItemId,
            Name = def.Name,
            Icon = def.Icon,
            Price = item.Price,

            // 👉 UI should use these (projected state)
            Available = Math.Max(0, projectedAvailable),
            InCargo = Math.Max(0, projectedCargo),
            PendingNet = net,

            MaxBuy = maxBuy,
            MaxSell = maxSell,
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

    private static int GetProjectedCredits(
        SessionService session,
        IReadOnlyDictionary<int, int> pending)
    {
        var credits = session.Credits;

        foreach (var (itemId, net) in pending)
        {
            var price = session.Market[itemId].Price;
            credits -= net * price;
        }
        return credits;
    }

    private static int GetProjectedCargoUsed(
        SessionService session,
        IReadOnlyDictionary<int, int> pending)
    {
        var total = 0;
        foreach (var cargo in session.Cargo)
        {
            var net = pending.GetValueOrDefault(cargo.ItemId);
            total += cargo.Quantity + net;
        }

        //this is for items that are not currently in cargo but are being bought this turn.
        foreach (var (itemId, net) in pending)
        {
            var exists = session.Cargo.Any(c => c.ItemId == itemId);
            if (!exists && net > 0)
            {
                total += net;
            }
        }
        return Math.Max(0, total);
    }

    private static int GetProjectedCargoLeft(
        SessionService session,
        IReadOnlyDictionary<int, int> pending)
    {
        return Math.Max(0, session.CargoSpace - GetProjectedCargoUsed(session, pending));
    }

    public int GetMaxBuyQuantity(
        SessionService session,
        IReadOnlyDictionary<int, int> pending,
        int itemId)
    {
        var item = session.GetMarketItem(itemId);
        if (item == null || item.Price <= 0) return 0;

        var credits = GetProjectedCredits(session, pending);
        var cargoLeft = GetProjectedCargoLeft(session, pending);

        var net = pending.GetValueOrDefault(itemId);

        var available = item.Available - net;

        var byCredits = credits / item.Price;
        var byCargo = cargoLeft;

        return Math.Max(0,
            Math.Min(available,
            Math.Min(byCredits, byCargo)));
    }

    public int GetMaxSellQuantity(
        SessionService session,
        IReadOnlyDictionary<int, int> pending,
        int itemId)
    {
        var cargo = session.GetCargoItem(itemId)?.Quantity ?? 0;
        var net = pending.GetValueOrDefault(itemId);

        var effectiveCargo = cargo + net;

        return Math.Max(0, effectiveCargo);
    }

    public void ApplyNet(
    SessionService session,
    IReadOnlyDictionary<int, int> pending)
    {
        foreach (var (itemId, net) in pending)
        {
            if (net == 0) continue;

            ApplyNetInternal(session, itemId, net);
        }
    }

    private static void ApplyNetInternal(
        SessionService session,
        int itemId,
        int net)
    {
        var item = session.GetMarketItem(itemId);
        if (item == null) return;

        if (net > 0)
        {
            var cargo = session.GetCargoItem(itemId);

            if (cargo == null)
            {
                session.Cargo.Add(new CargoItem
                {
                    ItemId = itemId,
                    Quantity = net
                });
            }
            else
            {
                cargo.Quantity += net;
            }

            item.Available -= net;
            session.Credits -= item.Price * net;
        }
        else
        {
            var sellQty = -net;

            var cargo = session.GetCargoItem(itemId);
            if (cargo == null) return;

            cargo.Quantity -= sellQty;
            item.Available += sellQty;
            session.Credits += item.Price * sellQty;
        }
    }
}

