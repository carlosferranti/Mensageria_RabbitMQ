using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

var factory = new ConnectionFactory { HostName = "localhost" };

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(
    queue: "fila_mensagem",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

Console.WriteLine("Informe a mensagem a ser enviada e pressione a tecla <ENTER>");

while (true)
{
    string message = Console.ReadLine();

    if (message == null || message.ToLower() == "exit")
        break;

    var aluno = new Aluno(1, "Bob");
    message = JsonSerializer.Serialize(aluno);

    var body = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(
        exchange: "",
        routingKey: "fila_mensagem",
        basicProperties: null,
        body: body);

    Console.WriteLine($"Foi enviado: {message}");
}


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
