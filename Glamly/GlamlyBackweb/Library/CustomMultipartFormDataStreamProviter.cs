using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace GlamlyBackweb.Library
{
    public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        private string nameFormat = string.Empty;

        public CustomMultipartFormDataStreamProvider(string RootPath) : base(RootPath)
        {
        }

        public CustomMultipartFormDataStreamProvider(string RootPath, int BufferSize) : base(RootPath, BufferSize)
        {
        }

        public CustomMultipartFormDataStreamProvider(string RootPath, string NameFormat) : base(RootPath)
        {
            nameFormat = NameFormat;
        }

        public CustomMultipartFormDataStreamProvider(string RootPath, string NameFormat, int BufferSize) : base(RootPath, BufferSize)
        {
            nameFormat = NameFormat;
        }

        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            //Make the file name URL safe and then use it & is the only disallowed url character allowed in a windows filename
            if (string.IsNullOrWhiteSpace(nameFormat))
                nameFormat = "{0}";

            string fullName = (!string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName) ? headers.ContentDisposition.FileName : "NoName").Trim('"');
            string name = Path.GetFileNameWithoutExtension(fullName);
            string extension = Path.GetExtension(fullName);
            name = (string.Format(nameFormat, name) + extension).Trim('"').Replace("&", "and");
            return name;
        }
    }
}