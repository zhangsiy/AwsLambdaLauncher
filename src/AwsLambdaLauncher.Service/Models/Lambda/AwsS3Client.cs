using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;

namespace MyWebService.Models.Lambda
{
    public class AwsS3Client
    {
        private IAmazonS3 Client { get; set; }

        public AwsS3Client(IAmazonS3 client)
        {
            Client = client;
        }

        public async Task<string> WriteToS3(string bucket, string key, Stream dataStream)
        {
            try
            {
                var request = new PutObjectRequest
                {
                    BucketName = bucket,
                    Key = $"{key}.zip",
                    InputStream = dataStream
                };

                await Client.PutObjectAsync(request);
                return key;

            }
            catch (AmazonS3Exception ex)
            {
                throw new Exception("Writing to S3 Failed", ex);
            }
        }
    }
}
