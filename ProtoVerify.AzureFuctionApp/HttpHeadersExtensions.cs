using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace ProtoVerify.AzureFuctionApp
{
    public static class HttpHeadersExtensions
    {
        public static string GetValueOrDefault(this IHeaderDictionary headers, string name)
        {
            if (headers.ContainsKey(name))
                return headers[name].FirstOrDefault();
            return null;
        }
    }
}
