
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://rktcczxq:eBER7Ivsuu2oNFAD-RPxpEFEPZMu99W5@whale.rmq.cloudamqp.com/rktcczxq"); //write AMQP URL

using (var connection = factory.CreateConnection())
{
    var channel = connection.CreateModel();

    channel.BasicQos(0, 1, false);

    var subscriber = new EventingBasicConsumer(channel);

    string queueName = "direct-queue-Critical";

    channel.BasicConsume(queueName, false, subscriber);

    Console.WriteLine("Listening...");

    subscriber.Received += (object? sender, BasicDeliverEventArgs e) =>
    {
        var message = Encoding.UTF8.GetString(e.Body.ToArray());

        Thread.Sleep(1500);

        Console.WriteLine($"Received Message : {message}");

        //File.AppendAllText("logs"+ queueName + ".txt", message + "\n");

        channel.BasicAck(e.DeliveryTag, false);
    };

    Console.ReadLine();
}