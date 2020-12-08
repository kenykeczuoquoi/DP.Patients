using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DP.Notifications.Services
{
    public class ServiceBusConsumer
    {
        private readonly ILogger _logger;
        private readonly QueueClient _queueClient;

        public ServiceBusConsumer(IConfiguration configuration, ILogger logger)
        {
            _logger = logger;
            _queueClient = new QueueClient(configuration.GetConnectionString("ServiceBusConnectionString"), "messages");
        }

        public void Register()
        {
            var options = new MessageHandlerOptions((e) => Task.CompletedTask)
            {
                AutoComplete = false
            };
            _queueClient.RegisterMessageHandler(ProcessMessage, options);
        }

        private async Task ProcessMessage(Message message, CancellationToken token)
        {

            try
            {


                var payload = JsonConvert.DeserializeObject<MessagePayLoad>(Encoding.UTF8.GetString(message.Body));

                if (payload.EventName == "NewUserRegistered")
                {
                    EmailSender sender = new EmailSender();

                    sender.SendNewUserEmail(payload.UserEmail);

                }


                await _queueClient.CompleteAsync(message.SystemProperties.LockToken);

            } 
            catch (Exception e)
            {
                Console.WriteLine(e);
                _logger.Error(e,"Something wrong during message process");
                throw;
            }
        }

        public class MessagePayLoad
        {
            public string EventName { get; set; }
            public string UserEmail { get; set; }
        }
    }
}
