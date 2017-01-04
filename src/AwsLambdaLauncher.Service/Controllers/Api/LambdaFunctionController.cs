using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyWebService.Common.AwsLambda;
using MyWebService.Dtos;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MyWebService.Controllers.Api
{
    [Route("api/[controller]")]
    public class LambdaFunctionController : ControllerBase
    {
        private LambdaCodePackageUploader Uploader { get; set; }

        public LambdaFunctionController(LambdaCodePackageUploader uploader)
            : base()
        {
            Uploader = uploader;
        }

        [HttpPost]
        public async Task<string> Post([FromBody]LambdaFunctionData functionData)
        {
            var key = await Uploader.UploadAsync(functionData.FunctionName, functionData.FunctionCode);
            return key;
        }

        [HttpPut("{functionName}")]
        public async Task<string> Put(string functionName, [FromBody]LambdaFunctionData functionData)
        {
            var key = await Uploader.UploadAsync(functionName, functionData.FunctionCode);
            return key;
        }
    }
}
