using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace EMT.Web.Api.Common.Formatters
{
    public class CSVMediaTypeFormatter : BufferedMediaTypeFormatter
    {

        public CSVMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/csv"));
        }

        public CSVMediaTypeFormatter(MediaTypeMapping mediaTypeMapping) : this()
        {
            MediaTypeMappings.Add(mediaTypeMapping);
        }

        public CSVMediaTypeFormatter(IEnumerable<MediaTypeMapping> mediaTypeMappings) : this()
        {
            foreach (var mediaTypeMapping in mediaTypeMappings)
            {
                MediaTypeMappings.Add(mediaTypeMapping);
            }
        }

        public override bool CanWriteType(Type type)
        {                            
            Type _type = typeof(IEnumerable);
            return _type.IsAssignableFrom(type);
        }

        public override bool CanReadType(Type type)
        {
            return (type == typeof(string));
        }

        public override object ReadFromStream(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            using (StreamReader sr = new StreamReader(readStream))
            {
                string data = sr.ReadToEnd();
                return data;
            }
        }
    }
}