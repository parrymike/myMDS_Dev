﻿using System;
using System.IO;
using System.Net;
//using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Text;

//http://www.strathweb.com/2013/01/replace-json-net-with-servicestack-text-in-asp-net-web-api/
namespace eMotive.CMS.Core
{
   /* public class ServiceStackTextFormatter : MediaTypeFormatter
    {
        public ServiceStackTextFormatter()
        {
            JsConfig.DateHandler = DateHandler.ISO8601;
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));

            SupportedEncodings.Add(new UTF8Encoding(false, true));
            SupportedEncodings.Add(new UnicodeEncoding(false, true, true));
        }

        public override bool CanReadType(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            return true;
        }

        public override bool CanWriteType(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            return true;
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, System.Net.Http.HttpContent content, IFormatterLogger formatterLogger)
        {
            var task = Task<object>.Factory.StartNew(() => JsonSerializer.DeserializeFromStream(type, readStream));
            return task;
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, System.Net.Http.HttpContent content, TransportContext transportContext)
        {
            var task = Task.Factory.StartNew(() => JsonSerializer.SerializeToStream(value, type, writeStream));
            return task;
        }
    }*/
}