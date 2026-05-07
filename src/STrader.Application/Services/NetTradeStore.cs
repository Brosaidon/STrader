namespace STrader.Application.Services;

// Stores net trade deltas per item.
// Used to project market state optimistically before committing on travel.
public class NetTradeStore
{
    private readonly Dictionary<int, int> _net = new();

    public IReadOnlyDictionary<int, int> Net => _net;

    public Dictionary<int, int> Snapshot()
    => _net.ToDictionary(x => x.Key, x => x.Value);

    public void Add(int itemId, int delta)
    {
        _net[itemId] = _net.GetValueOrDefault(itemId) + delta;

        if (_net[itemId] == 0)
        {
            _net.Remove(itemId);
        }
    }

    public void Clear()
    {
        _net.Clear();
    }
}