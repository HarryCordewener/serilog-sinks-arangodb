namespace Serilog.Sinks.ArangoDb.Tests
{
    public sealed class SinkTests
    {
        [ClassDataSource<ArangoDbTestSink>(Shared = SharedType.PerTestSession)]
        public required ArangoDbTestSink ArangoDb { get; init; }


        [Test]
        [NotInParallel]
        public async ValueTask CanLog()
        {
            ArangoDb.Logger.Warning("Test log message {Number}", 42);

            var result = await ArangoDb.ArangoContext.Query.ExecuteAsync<Dictionary<string, object>>(ArangoDb.ArangoHandle, 
                $"FOR doc IN test_logs_collection RETURN doc");

            await Assert.That(result.Count).IsGreaterThanOrEqualTo(1);
        }

        [Test]
        [NotInParallel]
        public async ValueTask LogsCarryPropertyInformation()
        {
            ArangoDb.Logger
                .ForContext<SinkTests>()
                .Information("Logging with context {ContextValue}", "TestContext");

            var result = await ArangoDb.ArangoContext.Query.ExecuteAsync<dynamic>(ArangoDb.ArangoHandle, 
                $"FOR doc IN test_logs_collection RETURN doc.Properties.ContextValue");

            var findResult = result.Where(log => log?.ToString() == "TestContext");

            await Assert.That(findResult.Count()).IsGreaterThanOrEqualTo(1);
        }

        [Test]
        [NotInParallel]
        public async ValueTask LogsCarryDeepPropertyInformation()
        {
            ArangoDb.Logger
                .ForContext<SinkTests>()
                .Information("Logging with context {@ContextValue}", new { Gquuuuuux = "Bar"});

            var result = await ArangoDb.ArangoContext.Query.ExecuteAsync<string>(ArangoDb.ArangoHandle, 
                $"FOR doc IN test_logs_collection RETURN doc.Properties.ContextValue.Gquuuuuux");

            var findResult = result.Where(log => log == "Bar");

            await Assert.That(findResult.Count()).IsGreaterThanOrEqualTo(1);
        }
    }
}
