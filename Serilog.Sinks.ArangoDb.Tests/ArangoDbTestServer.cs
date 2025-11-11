using Testcontainers.ArangoDb;
using TUnit.Core.Interfaces;

namespace Serilog.Sinks.ArangoDb.Tests;

public class ArangoDbTestServer : IAsyncInitializer, IAsyncDisposable
{
    private const string PASSWORD = "password";
    private const string USER = "root";

    public ArangoDbContainer Instance { get; } = 
        new ArangoDbBuilder()
        .WithImage("arangodb:latest")
        .WithPassword("password")
        .WithReuse(false)
        .Build();

    public string ConnectionString => $"Server={Instance.GetTransportAddress()};User={USER};Realm=;Password={PASSWORD};";

    public async Task InitializeAsync() 
        => await Instance.StartAsync();
    
    public async ValueTask DisposeAsync() 
        => await Instance.DisposeAsync();
}
