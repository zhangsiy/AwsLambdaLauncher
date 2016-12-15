using System.Threading.Tasks;

namespace MyWebService.Models.Lambda
{
    public class LambdaCodePackageUploader
    {
        private AwsS3Client S3Client { get; set; }

        public LambdaCodePackageUploader(AwsS3Client s3Client)
        {
            S3Client = s3Client;
        }

        public async Task<string> UploadAsync(string functionName, string codeBody)
        {
            var packageBuilder = new LambdaCodePackageBuilder();
            return await S3Client.WriteToS3(
                "temp-jeff-test-attribute-calculation-lambda-bucket1",
                functionName,
                packageBuilder.BuildZipStream(codeBody)
            );
        }
    }
}
