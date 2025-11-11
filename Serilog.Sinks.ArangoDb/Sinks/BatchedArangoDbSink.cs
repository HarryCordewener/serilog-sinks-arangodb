using Core.Arango;
using Core.Arango.Protocol;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Json;
using System.Text;
using System.Text.Json;

namespace Serilog.Sinks.ArangoDb;

/// <summary>
/// ArangoDb sink for Serilog
/// </summary>
public class BatchedArangoDbSink : IBatchedLogEventSink
{
    private readonly string[] _requiredIndexes = ["Level", "Timestamp", "MessageTemplate"];
    private readonly JsonFormatter _formatProvider = new(renderMessage: false);
    private readonly ArangoContext _arango;
    private readonly ArangoHandle _database;
    private readonly string _collection;

    /// <summary>
    /// Constructs ArangoDbSink
    /// </summary>
    /// <param name="dbContext">An Arango Db Context, with a live endpoint.</param>
    /// <param name="handle">An Arango Db Handle, representing the database - can accept a string for the database name.</param>
    /// <param name="collectionName">The name of the collection to use.</param>
    public BatchedArangoDbSink(
        ArangoContext dbContext,
        ArangoHandle handle,
        string collectionName)
    {
        _arango = dbContext;
        _database = handle;
        _collection = collectionName;

        EnsureIndexesAndDatabase(dbContext, handle)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    private async ValueTask EnsureIndexesAndDatabase(ArangoContext dbContext, ArangoHandle handle)
    {

        if (!await dbContext.Database.ExistAsync(handle))
        {
            await dbContext.Database.CreateAsync(handle);
        }

        if (!await dbContext.Collection.ExistAsync(handle, _collection))
        {
            await dbContext.Collection.CreateAsync(handle, new ArangoCollection
            {
                Name = _collection,
                KeyOptions = new ArangoKeyOptions
                {
                    Type = ArangoKeyType.Padded
                }
            });
        }

        var existingIndexes = await dbContext.Index.ListAsync(handle, _collection);

        foreach (var index in _requiredIndexes)
        {
            if (!existingIndexes.Any(x => x.Name == index))
            {
                await dbContext.Index.CreateAsync(handle, _collection, new ArangoIndex
                {
                    Type = ArangoIndexType.Persistent,
                    Name = index,
                    Fields = [index]
                });
            }
        }
    }

    public async Task EmitBatchAsync(IReadOnlyCollection<LogEvent> batch)
    {
        try
        {
            var serialized = new List<Dictionary<string, object>?>();
            foreach (var logEvent in batch)
            {
                StringBuilder sb = new();
                using (StringWriter writer = new(sb))
                {
                    _formatProvider.Format(logEvent, writer);
                }

                serialized.Add(JsonSerializer.Deserialize<Dictionary<string, object>>(sb.ToString().AsSpan()));

            }

            await _arango.Document.CreateManyAsync(_database, _collection, serialized.Where(x => x is { }));
        }
        catch
        {
            // Swallow all exceptions
        }
    }
}
