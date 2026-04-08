using Confluent.Kafka;
using LOMS_Leave_DataAccess;
using LOMS_Leave_Shared;
using Microsoft.Extensions.Hosting;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LOMS_Leave_Buisness.Messaging
{
    public class LeaveApplicationConsumer : BackgroundService
    {
        private readonly ConsumerConfig _config;
        private readonly string _topic = "applications-topic";

        public LeaveApplicationConsumer()
        {
            _config = new ConsumerConfig
            {
                BootstrapServers = "kafka-container:9092",
                GroupId = "leave-service-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var consumer = new ConsumerBuilder<Ignore, string>(_config).Build())
            {
                consumer.Subscribe(_topic);

                // On utilise Task.Run pour que le polling Kafka ne bloque pas le thread principal
                await Task.Run(async () =>
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        try
                        {
                            var consumeResult = consumer.Consume(stoppingToken);
                            var eventData = JsonSerializer.Deserialize<ApplicationCreatedEvent>(consumeResult.Message.Value);

                            if (eventData != null && eventData.ApplicationTypeID == 1)
                            {
                                // Appel de la méthode asynchrone pour traiter le message
                                await ProcessLeaveApplication(eventData);
                            }
                        }
                        catch (OperationCanceledException) { break; }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Erreur Consumer: {ex.Message}");
                        }
                    }
                }, stoppingToken);
            }
        }

        private async Task ProcessLeaveApplication(ApplicationCreatedEvent e)
        {
            // On exécute tout le travail DB sur un thread séparé pour l'asynchronisme
            bool success = await Task.Run(() =>
            {
                // 1. Préparation du DTO de base pour le cache
                ApplicationDTO baseDTO = new ApplicationDTO
                {
                    ApplicationID = e.ApplicationID,
                    EmployeeID = e.EmployeeID,
                    ApplicationDate = e.ApplicationDate,
                    Status = "New",
                    ApplicationTypeID = e.ApplicationTypeID,
                    CreatedByUserID = e.CreatedByUserID,
                    Notes = e.Notes
                };

                // 2. Sauvegarde dans la table miroir BaseApplications
                if (clsApplicationData.SaveBaseApplication(baseDTO))
                {
                    // 3. Sauvegarde dans la table LeaveApplications via la BLL
                    clsLeaveApplication leaveApp = new clsLeaveApplication();
                    leaveApp.BaseApplicationData = baseDTO;
                    leaveApp.LeaveDTO.ApplicationID = e.ApplicationID;

                    return leaveApp.Save(); // Appel de ta BLL (Synchrone ADO.NET)
                }
                return false;
            });

            if (success)
            {
                Console.WriteLine($"[Success] Synchronisation complète pour l'ID: {e.ApplicationID}");
            }
        }
    }
}