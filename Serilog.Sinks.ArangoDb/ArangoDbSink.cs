using Core.Arango;
using Core.Arango.Protocol;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Json;
using System.Text;
using System.Text.Json;

namespace Serilog.Sinks.ArangoDb;

public class ArangoDbSink : ILogEventSink
{
    private readonly JsonFormatter _formatProvider = new (renderMessage: false);
    private readonly ArangoContext _arango;
    private readonly ArangoHandle _database;
    private readonly string _collection;

    public ArangoDbSink(
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
            await dbContext.Database.CreateAsync(handle);

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

        var indexes = await dbContext.Index.ListAsync(handle, _collection);

        if (!indexes.Any(x => x.Name == "Level"))
        {
            await dbContext.Index.CreateAsync(handle, _collection, new ArangoIndex
            {
                Type = ArangoIndexType.Persistent,
                Name = "Level",
                Fields = ["Level"]
            });
        }

        if (!indexes.Any(x => x.Name == "Timestamp"))
        {
            await dbContext.Index.CreateAsync(handle, _collection, new ArangoIndex
            {
                Type = ArangoIndexType.Persistent,
                Name = "Timestamp",
                Fields = ["Timestamp"]
            });
        }

        if (!indexes.Any(x => x.Name == "MessageTemplate"))
        {
            await dbContext.Index.CreateAsync(handle, _collection, new ArangoIndex
            {
                Type = ArangoIndexType.Persistent,
                Name = "MessageTemplate",
                Fields = ["MessageTemplate"]
            });
        }
    }

    public void Emit(LogEvent logEvent)
    {
        try
        {
            StringBuilder sb = new();
            using (StringWriter writer = new(sb))
            {
                _formatProvider.Format(logEvent, writer);
            }

            var serialized = JsonSerializer.Deserialize<Dictionary<string, object>>(sb.ToString());

            _arango.Document.CreateAsync(_database, _collection, serialized)
                .AsTask()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }
        catch
        {
            // Swallow all exceptions
        }
    }
    public class ArangoLogEvent
    {
        public required DateTimeOffset Timestamp { get; set; }
        public required string Level { get; set; }
        public required string Message { get; set; }
        public required string MessageTemplate { get; set; }
        public string? Exception { get; set; }
        public required Dictionary<string, object?> Properties { get; set; }
    }
}
