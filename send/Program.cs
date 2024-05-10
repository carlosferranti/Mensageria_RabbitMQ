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

        channel.BasicPublish(
            exchange: "",
            routingKey: "fila_mensagem",
            basicProperties: null,
            body: body);

        Console.WriteLine($"Arquivo XML foi enviado: {filePath}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ocorreu um erro ao enviar o arquivo: {ex.Message}");
    }
}
