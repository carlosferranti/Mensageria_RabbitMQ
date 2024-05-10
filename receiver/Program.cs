using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

var factory = new ConnectionFactory { HostName = "localhost" };

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(
    queue: "mensagem...",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

Console.WriteLine("Aguardando a mensagens");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    var aluno = JsonSerializer.Deserialize<Aluno>(message);

    Console.WriteLine($" RECEDIBO: {message}");
};

channel.BasicConsume(
    queue: "fila_mensagem",
    autoAck: true,
    consumer: consumer);

Console.WriteLine("Aperte a tecla <ENTER> para encerrar.");
Console.ReadLine();

class Aluno
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public DateTime DataCriacao { get; }
    public DateTime DataAtualizacao { get; set; }

    public Aluno(int id, string nome)
    {
        Id = id;
        Nome = nome;
        DataCriacao = DateTime.Now;
        DataAtualizacao = DateTime.Now;
    }
}