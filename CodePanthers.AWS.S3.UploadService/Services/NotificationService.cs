using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace CodePanthers.AWS.S3.UploadService.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IAmazonSimpleNotificationService _snsService;
        private readonly string TopicArn;

        public NotificationService(IAmazonSimpleNotificationService snsService, IConfiguration Configuration)
        {
            _snsService = snsService;
            TopicArn = Configuration["SNSTopicArn"];
        }
        public async Task RegisterSubscirption(string topic, string email)
        {
            try
            {
                await _snsService.SubscribeAsync(topic, "email", email);
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public async Task RegisterSubscirption(string email)
        {
            try
            {
                await _snsService.SubscribeAsync(TopicArn, "email", email);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SendUploadNotification( string topic, string message)
        {
            try
            {
                var request = new PublishRequest
                {
                    Message = message,
                    TopicArn = topic,
                };
                await _snsService.PublishAsync(request);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SendUploadNotification(string message)
        {
            try
            {
                var request = new PublishRequest
                {
                    Message = message,
                    TopicArn = TopicArn,
                };
                await _snsService.PublishAsync(request);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
