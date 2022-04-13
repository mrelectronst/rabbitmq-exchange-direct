
using PublisherRabbitMQ;
using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://rktcczxq:eBER7Ivsuu2oNFAD-RPxpEFEPZMu99W5@whale.rmq.cloudamqp.com/rktcczxq"); //write AMQP URL

string messageRead;

do
{
    Console.Write("Please write your message for 50 times: ");
    messageRead = Console.ReadLine();
    if (messageRead != string.Empty && messageRead != null)
    {
        PublishMessage(messageRead);
    }
    else
    {
        Environment.Exit(0);
    }

} while (messageRead != null);

void PublishMessage(string message)
{
    try
    {
        using (var connection = factory.CreateConnection())
        {
            var channel = connection.CreateModel();

            channel.ExchangeDeclare("exchange-direct-logs", durable: true, type: ExchangeType.Direct);

            Enum.GetNames(typeof(LogNames.EnumLognames)).ToList().ForEach(x =>
            {
                var queueName = $"direct-queue-{x}";
                channel.QueueDeclare(queueName, durable: true, false, false);

                var routeKey = $"route-{x}";

                channel.QueueBind(queueName, "exchange-direct-logs", routeKey, null);
            });

            Enumerable.Range(0, 40).ToList().ForEach(x =>
            {
                LogNames.EnumLognames enumLognames = (LogNames.EnumLognames)new Random().Next(1, 5);

                var messageBody = Encoding.UTF8.GetBytes(message + $"_{enumLognames}");

                var routeKey = $"route-{enumLognames}";

                channel.BasicPublish("exchange-direct-logs", routeKey, null, messageBody);

                Console.WriteLine($"'{message}-{enumLognames}' is sended");
            });

            Console.WriteLine("All Logs are sended");
        }
    }
    catch (Exception ex) { Console.WriteLine(ex.ToString()); }


}


