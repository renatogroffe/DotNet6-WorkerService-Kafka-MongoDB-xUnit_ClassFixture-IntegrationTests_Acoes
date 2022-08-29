using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using WorkerAcoes.IntegrationTests.Documents;
using Confluent.Kafka;

namespace WorkerAcoes.IntegrationTests;

public class WorkerAcoesFixture : IDisposable
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public IConfiguration Configuration { get; init; }
    public string COD_CORRETORA { get; init; } = "00000";
    public string NOME_CORRETORA { get; init; } = "Corretora Testes";
    public IMongoCollection<AcaoDocument>? AcaoCollection { get; set; }

    public WorkerAcoesFixture()
    {
        Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.json")
            .AddEnvironmentVariables().Build();
        
        AcaoCollection = new MongoClient(Configuration["MongoDBConnection"])
            .GetDatabase(Configuration["MongoDatabase"])
            .GetCollection<AcaoDocument>(Configuration["MongoCollection"]);
        AcaoCollection.DeleteMany(filter => true);

    }

    public void Dispose()
    {
        AcaoCollection = null;
    }
}