using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Core.Arango;
using Serilog.Sinks.ArangoDb.Sinks;

namespace Serilog.Sinks.ArangoDb;

public static class ArangoDbSinkExtensions
{
    extension(LoggerSinkConfiguration loggerConfiguration)
    {
        /// <summary>
        /// Setup the ArangoDb Logging Sink
        /// </summary>
        /// <param name="dbContext">ArangoDb Context</param>
        /// <param name="handle">ArangoDb Handle</param>
        /// <param name="collectionName">Collection Name</param>
        /// <param name="levelSwitch">Logging Level Switch</param>
        /// <param name="restrictedToMinimumLevel">Minimum Log Level</param>
        /// <returns>LoggerConfiguration with the ArangoDb Sink added</returns>
        /// <exception cref="ArgumentNullException">Missing arguments</exception>
        public LoggerConfiguration ArangoDb(
        ArangoContext dbContext,
        ArangoHandle handle,
        string collectionName,
        LoggingLevelSwitch? levelSwitch,
        LogEventLevel restrictedToMinimumLevel = LogEventLevel.Information)
        {
            ArgumentNullException.ThrowIfNull(loggerConfiguration);
            ArgumentNullException.ThrowIfNull(dbContext);
            ArgumentNullException.ThrowIfNull(handle);
            if (string.IsNullOrWhiteSpace(collectionName)) throw new ArgumentNullException(nameof(collectionName));

            var sink = new ArangoDbSink(
                dbContext,
                handle,
                collectionName);

            return loggerConfiguration.Sink(sink, restrictedToMinimumLevel, levelSwitch);
        }

        /// <summary>
        /// Setup the ArangoDb Logging Sink, using the default 'logs' database and collection
        /// </summary>
        /// <param name="dbContext">ArangoDb Context</param>
        /// <param name="levelSwitch">Logging Level Switch</param>
        /// <param name="restrictedToMinimumLevel">Minimum Log Level</param>
        /// <returns>LoggerConfiguration with the ArangoDb Sink added</returns>
        /// <exception cref="ArgumentNullException">Missing arguments</exception>
        public LoggerConfiguration ArangoDb(
        ArangoContext dbContext,
        LoggingLevelSwitch? levelSwitch,
        LogEventLevel restrictedToMinimumLevel = LogEventLevel.Information)
        {
            ArgumentNullException.ThrowIfNull(loggerConfiguration);
            ArgumentNullException.ThrowIfNull(dbContext);

            var sink = new ArangoDbSink(
                dbContext,
                "logs",
                "logs");

            return loggerConfiguration.Sink(sink, restrictedToMinimumLevel, levelSwitch);
        }
    }

    extension(Func<ILogEventSink, LogEventLevel, LoggingLevelSwitch?, LoggerConfiguration> addSink)
    {
        /// <summary>
        /// Setup the ArangoDb Logging Sink
        /// </summary>
        /// <param name="dbContext">ArangoDb Context</param>
        /// <param name="handle">ArangoDb Handle</param>
        /// <param name="collectionName">Collection Name</param>
        /// <param name="levelSwitch">Logging Level Switch</param>
        /// <param name="restrictedToMinimumLevel">Minimum Log Level</param>
        /// <returns>LoggerConfiguration with the ArangoDb Sink added</returns>
        /// <exception cref="ArgumentNullException">Missing arguments</exception>
        public LoggerConfiguration ConfigureArangoDb(
        ArangoContext dbContext,
        ArangoHandle handle,
        string collectionName,
        LoggingLevelSwitch? levelSwitch,
        LogEventLevel restrictedToMinimumLevel = LogEventLevel.Information)
        {
            ArgumentNullException.ThrowIfNull(addSink);
            ArgumentNullException.ThrowIfNull(dbContext);
            ArgumentNullException.ThrowIfNull(handle);
            if (string.IsNullOrWhiteSpace(collectionName)) throw new ArgumentNullException(nameof(collectionName));

            var sink = new ArangoDbSink(
                dbContext,
                handle,
                collectionName);

            return addSink(sink, restrictedToMinimumLevel, levelSwitch);
        }
    }
}
