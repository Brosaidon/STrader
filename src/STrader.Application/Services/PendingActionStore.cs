namespace STrader.Application.Services;

using STrader.Application.Models;

public class PendingActionStore
{
    public List<PendingAction> Actions { get; } = new();
}