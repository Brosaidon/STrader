
using System.Threading;
using System.Threading.Tasks;
using STrader.Domain.Interfaces;

namespace STrader.Application;

public sealed class IncrementCounter
{
    private readonly ICounterRepository _repository;

    public IncrementCounter(ICounterRepository repository)
    {
        _repository = repository;
    }

    public Task ExecuteAsync(CancellationToken ct)
    {
        return _repository.IncrementAsync(ct);
    }
}
