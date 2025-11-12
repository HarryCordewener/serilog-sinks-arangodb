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
        IArangoContext dbContext,
        ArangoHandle handle,
        string collectionName,
        LoggingLevelSwitch? levelSwitch,
        LogEventLevel restrictedToMinimumLevel = LogEventLevel.Information)
        {
            if (loggerConfiguration is null) throw new ArgumentNullException(nameof(loggerConfiguration));
            if (dbContext is null) throw new ArgumentNullException(nameof(dbContext));
            if (handle is null) throw new ArgumentNullException(nameof(handle));

            if (string.IsNullOrWhiteSpace(collectionName)) throw new ArgumentNullException(nameof(collectionName));

            var sink = new ArangoDbSink(
                dbContext,
                handle,
                collectionName);

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
        IArangoContext dbContext,
        ArangoHandle handle,
        string collectionName,
        LoggingLevelSwitch? levelSwitch,
        LogEventLevel restrictedToMinimumLevel = LogEventLevel.Information)
        {
            if (addSink is null) throw new ArgumentNullException(nameof(addSink));
            if (dbContext is null) throw new ArgumentNullException(nameof(dbContext));
            if (handle is null) throw new ArgumentNullException(nameof(handle));

            if (string.IsNullOrWhiteSpace(collectionName)) throw new ArgumentNullException(nameof(collectionName));

            var sink = new ArangoDbSink(
                dbContext,
                handle,
                collectionName);

            return addSink(sink, restrictedToMinimumLevel, levelSwitch);
        }
    }
}
