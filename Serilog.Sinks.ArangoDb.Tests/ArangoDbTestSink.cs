using Core.Arango;
using Core.Arango.Serialization.Json;
using Serilog.Sinks.ArangoDb.Sinks;
using TUnit.Core.Interfaces;

namespace Serilog.Sinks.ArangoDb.Tests;

public class ArangoDbTestSink : IAsyncInitializer, IAsyncDisposable
{
    [ClassDataSource<ArangoDbTestServer>(Shared = SharedType.PerTestSession)]
    public required ArangoDbTestServer ArangoDb { get; init; }

    private ArangoContext? _arangoContext;
    private ArangoHandle? _handle;
    private ILogger? _logger;

    public ArangoContext ArangoContext { get => _arangoContext!; }
    public ArangoHandle ArangoHandle { get => _handle!; }
    public ILogger Logger { get => _logger!; }

    public Task InitializeAsync()
    {
        var arangoConfig = new ArangoConfiguration()
        {
            ConnectionString = ArangoDb.ConnectionString,
            Serializer = new ArangoJsonSerializer(System.Text.Json.JsonNamingPolicy.CamelCase)
        };

        _arangoContext = new ArangoContext(arangoConfig);
        _handle = new ArangoHandle("test_logs");

        var sink = new ArangoDbSink(
            _arangoContext,
            _handle,
            "test_logs_collection");

        _logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Sink(sink)
            .CreateLogger();

        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
