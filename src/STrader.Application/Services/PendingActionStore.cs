namespace STrader.Application.Services;

using STrader.Application.Models;

// Stores net trade deltas per item.
// Used to project market state optimistically before committing on travel.
public class PendingActionStore
{
    private readonly Dictionary<int, int> _net = new();

    public IReadOnlyDictionary<int, int> Net => _net;

    public void Add(int itemId, ActionType type, int quantity)
    {
        var delta = type == ActionType.Buy ? quantity : -quantity;

        if (_net.ContainsKey(itemId))
            _net[itemId] += delta;
        else
            _net[itemId] = delta;
    }

    public void Clear()
    {
        _net.Clear();
    }
}