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
        [HttpGet("{id}")]
        public async Task<string> Get(string id)
        {
            var key = await Uploader.UploadAsync("test_function", "abcabc");

            return key;
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(string id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
        }
    }
}
