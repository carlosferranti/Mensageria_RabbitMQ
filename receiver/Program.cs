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

    Console.WriteLine($"Mensagem recebida: {message}");
};

channel.BasicConsume(
    queue: "fila_mensagem",
    autoAck: true,
    consumer: consumer);

Console.WriteLine("Pressione a tecla <ENTER> para encerrar.");
Console.ReadLine();
