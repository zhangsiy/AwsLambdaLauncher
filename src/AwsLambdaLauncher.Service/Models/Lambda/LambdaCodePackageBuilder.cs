using System.IO;
using System.Text;

namespace MyWebService.Models.Lambda
{
    public class LambdaCodePackageBuilder
    {
        public Stream BuildZipStream(string codeBody)
        {
            var stream =  new MemoryStream();
            StreamWriter sw = new StreamWriter(stream, Encoding.UTF8);
            sw.Write(codeBody);
            sw.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }
}
