using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyWebService.Models.Lambda;

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

        // GET api/values/5
        [HttpGet("{functionName}")]
        public async Task<string> Get(string functionName)
        {
            string codeBody = 
                "console.log('Loading function');" +
                "var AWS = require('aws-sdk');" + 

                "exports.handler = function(event, context) {" +
                    "var eventText = '3333333333#####' + JSON.stringify(event, null, 2);" +
                    "console.log('Received event:', eventText);" +
                    "var sns = new AWS.SNS();" + 
                    "var params = {" + 
                        "Message: eventText," +
                        "Subject: 'Test SNS From Lambda'," + 
                        "TopicArn: 'arn:aws:sns:us-east-1:119381170469:Temp_Jeff_Test_Lambda_Output_Topic'" + 
                    "};" + 
                    "sns.publish(params, context.done);" + 
                "};";

            var key = await Uploader.UploadAsync("temp_jeff_test_lambda_to_sns_3", codeBody);

            return key;
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{functionName}")]
        public void Put(string functionName, [FromBody]string value)
        {
        }
    }
}
