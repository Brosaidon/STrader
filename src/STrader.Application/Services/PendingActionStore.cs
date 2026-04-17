namespace STrader.Application.Services;

using STrader.Application.Models;

public class PendingActionStore
{
    private readonly Dictionary<int, int> _net = new();
    public List<UserAction> Actions { get; } = new();
}