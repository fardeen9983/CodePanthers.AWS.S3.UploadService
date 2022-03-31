using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace CodePanthers.AWS.S3.UploadService.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IAmazonS3 _client;
        private readonly string BucketName;
        private readonly long MB = (long)Math.Pow(2, 20);
        private readonly int ExpirationDurationInHours;

        public FileUploadService(IAmazonS3 client, IConfiguration Configuration)
        {
            _client = client;
            BucketName = Configuration["S3BucketName"];
            ExpirationDurationInHours = int.Parse(Configuration["S3ObjectExpirationHours"]);
        }
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

                    return _client.GetPreSignedURL(getUrlRequest);
                }

                return null;
            }
            catch (AmazonS3Exception e)
            {
                throw;
            }
            catch (Exception e)
            {
                throw;
            }
        }

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
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
