// Sanitized portfolio sample.
// This is illustrative code showing the architectural pattern.
// It is NOT production employer source code.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Portfolio.IntegrationSample;

public sealed record TransferRecord(string Id, string? Status, string? Payload);

public interface ISourceRepository
{
    Task<IReadOnlyList<TransferRecord>> GetCandidatesAsync(
        CancellationToken cancellationToken);
}

public interface ITargetRepository
{
    Task<IReadOnlySet<string>> GetExistingIdsAsync(
        CancellationToken cancellationToken);

    Task InsertAsync(
        TransferRecord record,
        CancellationToken cancellationToken);
}

public sealed class SyncEngine
{
    private readonly ISourceRepository _source;
    private readonly ITargetRepository _target;

    public SyncEngine(ISourceRepository source, ITargetRepository target)
    {
        _source = source;
        _target = target;
    }

    public async Task<int> SyncOnceAsync(CancellationToken cancellationToken)
    {
        var existing = await _target.GetExistingIdsAsync(cancellationToken);
        var candidates = await _source.GetCandidatesAsync(cancellationToken);

        var transferred = 0;

        foreach (var record in candidates)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (existing.Contains(record.Id))
                continue;

            await _target.InsertAsync(record, cancellationToken);
            transferred++;
        }

        return transferred;
    }

    public async Task RunAsync(
        TimeSpan interval,
        Action<string> log,
        CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var count = await SyncOnceAsync(cancellationToken);
                log($"Synchronization cycle completed. New records: {count}");
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                log($"Synchronization error: {ex.Message}");
            }

            await Task.Delay(interval, cancellationToken);
        }
    }
}
