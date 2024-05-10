using RabbitMQ.Client;
using System;
using System.IO;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(
    queue: "fila_mensagem",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

Console.WriteLine("Informe o caminho do arquivo XML a ser enviado ou digite 'exit' para sair:");

while (true)
{
    string filePath = Console.ReadLine();

    if (string.IsNullOrEmpty(filePath) || filePath.ToLower() == "exit")
        break;

    if (!File.Exists(filePath))
    {
        Console.WriteLine("Arquivo não encontrado. Tente novamente.");
        continue;
    }

    try
    {
        var xmlContent = File.ReadAllText(filePath);
        var body = Encoding.UTF8.GetBytes(xmlContent);

        var fileName = Path.GetFileName(filePath);
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true; // Faz a mensagem persistir em caso de reinicialização do RabbitMQ
        properties.Headers = new Dictionary<string, object>();
        properties.Headers.Add("FileName", fileName); // Adiciona o nome do arquivo nas propriedades da mensagem

        channel.BasicPublish(
            exchange: "",
            routingKey: "fila_mensagem",
            basicProperties: properties,
            body: body);

        Console.WriteLine($"Arquivo XML foi enviado: {filePath}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ocorreu um erro ao enviar o arquivo: {ex.Message}");
    }
}
