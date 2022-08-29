# DotNet6-WorkerService-Kafka-MongoDB-xUnit_ClassFixture-IntegrationTests_Acoes
Exemplo de consumo de mensagens de um tópico do Apache Kafka com dados de ações em um Worker Service criado com .NET 6, utilizando ainda MongoDB para gravação dos dados, FluentValidation para validações e um Dockerfile para geração de imagens Docker em Linux. Inclui também um projeto criado com .NET 6 + xUnit + Fluent Assertions + configurações de ambiente para testes de integração desta aplicação via Class Fixture, com uso da interface ITestOutputHelper (xUnit) direcionando mensagens para saída padrão (stdout).

Documentação:

[**Shared Context between Tests | xUnit Documentation**](https://xunit.net/docs/shared-context)