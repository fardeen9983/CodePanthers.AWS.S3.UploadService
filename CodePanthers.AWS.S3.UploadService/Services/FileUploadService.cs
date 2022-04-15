using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using Amazon.CloudFront;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CodePanthers.AWS.S3.UploadService.Services
{
    /// <summary>
    /// Allows user to upload Files to S3 Bucket and Generate Presigned URLs
    /// </summary>
    public class FileUploadService : IFileUploadService
    {
        private readonly IAmazonS3 _client;
        private readonly INotificationService _notificationService;
        
        private readonly string BucketName;
        private readonly string CloudFronDomain;
        private readonly string CloudFrontKeyId;
        private readonly long MB = (long)Math.Pow(2, 20);
        private readonly int ExpirationDurationInHours;

        public FileUploadService(IAmazonS3 s3Client, INotificationService notificationService, IConfiguration Configuration)
        {
            _client = s3Client;
            _notificationService = notificationService;
            BucketName = Configuration["S3BucketName"];
            CloudFronDomain = Configuration["CloudFrontDomain"];
            CloudFrontKeyId = Configuration["CloudFrontKeyId"];
            ExpirationDurationInHours = int.Parse(Configuration["S3ObjectExpirationHours"]);
        }

        /// <summary>
        /// Generates a presigned URL of the file if it is successfully uploaded to S3
        /// </summary>
        /// <param name="file">File to be Uploaded</param>
        /// <returns></returns>
        public async Task<string> GetS3ObjectPresignedUrl(IFormFile file)
        {
            try
            {
                var key = Path.GetRandomFileName() + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName).ToLowerInvariant();

                if (await UploadFileToS3(file, key))
                {
                    var getUrlRequest = new GetPreSignedUrlRequest
                    {
                        BucketName = BucketName,
                        Key = key,
                        Expires = DateTime.UtcNow.AddHours(ExpirationDurationInHours),
                    };
                    await _notificationService.SendUploadNotification("hello");
                    return GetPrivateUrl(key);
                }

                return null;
            }
            catch (AmazonS3Exception)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Uploads the file to S3 Bucket if it exists
        /// </summary>
        /// <param name="file">Uploaded File</param>
        /// <param name="key">Object Key for S3</param>
        /// <returns></returns>
        private async Task<bool> UploadFileToS3(IFormFile file, string key)
        {
            try
            {
                if (await AmazonS3Util.DoesS3BucketExistV2Async(_client, BucketName))
                {
                    var transferUtilityConfig = new TransferUtilityConfig
                    {
                        ConcurrentServiceRequests = 5,
                        MinSizeBeforePartUpload = 20 * MB,
                    };

                    using (var transferUtility = new TransferUtility(_client, transferUtilityConfig))
                    {
                        var uploadRequest = new TransferUtilityUploadRequest
                        {
                            Key = key,
                            BucketName = BucketName,
                            InputStream = file.OpenReadStream(),
                            PartSize = 20 * MB,
                            StorageClass = S3StorageClass.Standard,
                            ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256,
                        };

                        await transferUtility.UploadAsync(uploadRequest);
                    }
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        private string GetPrivateUrl(string file)
        {
            try
            {
                return
                    AmazonCloudFrontUrlSigner.GetCannedSignedURL(
                        "https://" + CloudFronDomain + "/" +  file,
                        new StreamReader(@"PrivateKey.pem"),
                        CloudFrontKeyId,
                        DateTime.Now.AddDays(7)
                    );
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
