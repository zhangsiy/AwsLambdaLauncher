using System.IO;
using System.IO.Compression;

namespace MyWebService.Models.Lambda
{
    public class LambdaCodePackageBuilder
    {
        public Stream BuildZipStream(string codeBody)
        {
            var memoryStream = new MemoryStream();

            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                var demoFile = archive.CreateEntry("index.js");

                using (var entryStream = demoFile.Open())
                using (var streamWriter = new StreamWriter(entryStream))
                {
                    streamWriter.Write(codeBody);
                    streamWriter.Flush();
                }
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }
    }
}
