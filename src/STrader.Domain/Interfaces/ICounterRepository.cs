
namespace STrader.Domain.Interfaces

{
    // The interface for Counter actions.

    public interface ICounterRepository
    {
        Task IncrementAsync(CancellationToken ct);
        Task GetAsync<Counter>(CancellationToken ct);
    }
}