using System;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using Xunit;
using Xunit.Abstractions;
using FluentAssertions;
using Confluent.Kafka;
using MongoDB.Driver;
using WorkerAcoes.IntegrationTests.Models;

namespace WorkerAcoes.IntegrationTests;

public class TestesIntegracaoWorkerAcoes : IClassFixture<WorkerAcoesFixture>
{
    private readonly ITestOutputHelper _output;
    private readonly WorkerAcoesFixture _fixture;

    public TestesIntegracaoWorkerAcoes(ITestOutputHelper output,
        WorkerAcoesFixture fixture)
    {
        // Capturing Output - xUnit:
        // https://xunit.net/docs/capturing-output
        _output = output;
        _output.WriteLine($"Executando o construtor {nameof(TestesIntegracaoWorkerAcoes)}");
        
        _fixture = fixture;
        _output.WriteLine($"{nameof(WorkerAcoesFixture)} - Id: {_fixture.Id.ToString()}");
    }

    [Theory]
    [InlineData("ABCD", 100.98)]
    [InlineData("EFGH", 200.9)]
    [InlineData("IJKL", 1_400.978)]
    public void TestarWorkerService(string codigo, double valor)
    {
        _output.WriteLine($"Sistema operacional: {RuntimeInformation.OSDescription}");
        
        var broker = _fixture.Configuration["ApacheKafka:Broker"];
        var topic = _fixture.Configuration["ApacheKafka:Topic"];
        _output.WriteLine($"Tópico: {topic}");

        var cotacaoAcao = new Acao()
        {
            Codigo = codigo,
            Valor = valor,
            CodCorretora = _fixture.COD_CORRETORA,
            NomeCorretora = _fixture.NOME_CORRETORA
        };
        var conteudoAcao = JsonSerializer.Serialize(cotacaoAcao);
        _output.WriteLine($"Dados: {conteudoAcao}");

        var configKafka = new ProducerConfig
        {
            BootstrapServers = broker
        };

        using (var producer = new ProducerBuilder<Null, string>(configKafka).Build())
        {
            var result = producer.ProduceAsync(
                topic,
                new Message<Null, string>
                { Value = conteudoAcao }).Result;

            _output.WriteLine(
                $"Apache Kafka - Envio para o tópico {topic} concluído | " +
                $"{conteudoAcao} | Status: {result.Status.ToString()}");
        }

        _output.WriteLine("Aguardando o processamento do Worker...");
        Thread.Sleep(
            Convert.ToInt32(_fixture.Configuration["IntervaloProcessamento"]));

        _output.WriteLine($"MongoDB Database: {_fixture.Configuration["MongoDatabase"]}");
        _output.WriteLine($"MongoDB Collection: {_fixture.Configuration["MongoCollection"]}");
        var acaoDocument = _fixture.AcaoCollection.Find(h => h.Codigo == codigo).SingleOrDefault();

        _output.WriteLine("Analisar dados processados pelo Worker...");

        acaoDocument.Should().NotBeNull();
        acaoDocument.Codigo.Should().Be(codigo);
        acaoDocument.Valor.Should().Be(valor);
        acaoDocument.CodCorretora.Should().Be(_fixture.COD_CORRETORA);
        acaoDocument.NomeCorretora.Should().Be(_fixture.NOME_CORRETORA);
        acaoDocument.HistLancamento.Should().NotBeNullOrWhiteSpace();
        acaoDocument.DataReferencia.Should().NotBeNullOrWhiteSpace();

        _output.WriteLine("Analise dos dados processados pelo Worker concluida com sucesso!");
    }
}