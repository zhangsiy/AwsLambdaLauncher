using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;

namespace MyWebService.Common
{
    /// <summary>
    /// Simple AWS S3 client wrapper
    /// </summary>
    public class AwsS3Client
    {
        /// <summary>
        /// AWS SDK's S3 client
        /// </summary>
        private IAmazonS3 Client { get; set; }

        public AwsS3Client(IAmazonS3 client)
        {
            Client = client;
        }

        /// <summary>
        /// Upload/Write to S3, based on the specifications passed in
        /// </summary>
        /// <param name="bucket">Target S3 bucket name</param>
        /// <param name="key">Intended S3 object key name</param>
        /// <param name="dataStream">The stream of data representing the content of the file to upload</param>
        /// <returns>A task wrapping the object key name, to indicate a successful upload</returns>
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
