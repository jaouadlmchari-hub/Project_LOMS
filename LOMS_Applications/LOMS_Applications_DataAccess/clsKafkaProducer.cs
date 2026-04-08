using Confluent.Kafka;
using LOMS_Applications_Shared;
using System.Text.Json;
using System.Threading.Tasks;

namespace LOMS_Applications_DataAccess
{
    public static class clsKafkaProducer
    {
        private static readonly ProducerConfig _config = new ProducerConfig
        {
            BootstrapServers = "kafka-container:9092",
            Acks = Acks.All 
        };

        public static async Task<bool> PublishApplicationCreatedAsync(ApplicationCreatedEvent eventModel)
        {
            using (var producer = new ProducerBuilder<Null, string>(_config).Build())
            {
                try
                {
                    string messageValue = JsonSerializer.Serialize(eventModel);

                    var result = await producer.ProduceAsync("applications-topic",
                        new Message<Null, string> { Value = messageValue });

                    return result.Status == PersistenceStatus.Persisted;
                }
                catch (System.Exception)
                {
                    return false; // L'envoi a échoué
                }
            }
        }
    }
}