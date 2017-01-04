using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyWebService.Common.AwsLambda
{
    /// <summary>
    /// A quick example implementation to upload lambda function code 
    /// </summary>
    public class LambdaCodePackageUploader
    {
        private AwsS3Client S3Client { get; }

        public LambdaCodePackageUploader(AwsS3Client s3Client)
        {
            S3Client = s3Client;
        }

        /// <summary>
        /// For the given AWS Lambda function specifications, compile a code package and upload 
        /// it to the intended S3 bucket 
        /// </summary>
        /// <param name="functionName">Intended name of the function.</param>
        /// <param name="codeBody">Code body of the Lambda function.</param>
        /// <returns>A Task wrapping the actual resultant S3 object key name to indicate an successful upload.</returns>
        public async Task<string> UploadAsync(string functionName, string codeBody)
        {
            return await S3Client.WriteToS3(
                "temp-jeff-test-attribute-calculation-lambda-bucket1", // The S3 bucket to hold lambda function code packages
                GetValidFileNameString(functionName),
                GetNodeJsZipStream(codeBody) // For demo purposes, support NodeJs only for now.
            );
        }

        /// <summary>
        /// Normailze a given string to a format that is valid to use as file name
        /// </summary>
        /// <param name="functionName"></param>
        /// <returns></returns>
        private string GetValidFileNameString(string originalString)
        {
            // This regex is extracted from AWS Lambda's specifications
            Regex regex = new Regex("[^a-zA-Z0-9-_]");
            return regex.Replace(originalString, "_");
        }

        /// <summary>
        /// Get a ZIP stream that compresses the given code body. 
        /// The ZIP stream is expected to represent a zip file that contains the right
        /// files, in the right structure, for AWS Lambda code package specs.
        /// </summary>
        /// <param name="codeBody"></param>
        /// <returns></returns>
        private Stream GetNodeJsZipStream(string nodeJsCodeBody)
        {
            var memoryStream = new MemoryStream();

            // Start pumping data into the memory stream, with ZIP support
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                var functionEntryFile = archive.CreateEntry("index.js");

                using (var entryStream = functionEntryFile.Open())
                using (var streamWriter = new StreamWriter(entryStream))
                {
                    streamWriter.Write(nodeJsCodeBody);
                    streamWriter.Flush();
                }
            }

            // Reset the seek position of the stream to beginning.
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }
    }
}
