# Serilog.Sinks.Arangodb&nbsp;[![Build status](https://github.com/HarryCordewener/serilog-sinks-arangodb/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/HarryCordewener/serilog-sinks-arangodb/actions)&nbsp;[![NuGet](https://img.shields.io/nuget/v/Serilog.Sinks.ArangoDb.svg)](https://nuget.org/packages/serilog.sinks.arangodb)

A Serilog sink that writes events to a ArangoDB database

### Getting started

Install _Serilog.Sinks.ArangoDb_ into your .NET project:

```powershell
> dotnet add package Serilog.Sinks.ArangoDb
```

Point the logger to ArangoDb:

```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.ArangoDb(
        dbContext: arangoContext,
        handle: "db_log",
        collectionName: "logs")
    .CreateLogger();
```

### Notes
- This library uses [Core.Arango](https://github.com/coronabytes/dotnet-arangodb) for its connection to ArangoDB.
- This library uses System.Text.Json for its serialization.