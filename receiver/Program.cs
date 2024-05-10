using RabbitMQ.Client;
using RabbitMQ.Client.Events;
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

Console.WriteLine("Aguardando as mensagens...");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    // Extrai o nome do arquivo da mensagem
    var fileNameBytes = ea.BasicProperties.Headers["FileName"] as byte[];
    var fileName = Encoding.UTF8.GetString(fileNameBytes);

    // Escreve o conteúdo da mensagem em um arquivo no diretório especificado
    string directoryPath = @"C:\Users\carlo\source\Mensageria_RabbitMQ\files";
    string filePath = Path.Combine(directoryPath, fileName);
    File.WriteAllText(filePath, message);

    Console.WriteLine($"Arquivo XML recebido e salvo em: {filePath}");
    Console.WriteLine($"Conteúdo do arquivo XML: {message}");
};

channel.BasicConsume(
    queue: "fila_mensagem",
    autoAck: true,
    consumer: consumer);

Console.WriteLine("Pressione a tecla <ENTER> para encerrar.");
Console.ReadLine();
