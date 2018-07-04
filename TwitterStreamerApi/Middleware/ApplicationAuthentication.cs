using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterStreamerApi.Middleware
{
    public class ApplicationAuthentication
    {
        private readonly RequestDelegate _next;
        private readonly Options.ApplicationConfiguration _options;

        public ApplicationAuthentication(RequestDelegate next, IOptions<Options.ApplicationConfiguration> options)
        {
            _next = next;
            _options = options.Value; 
        }

        public async Task Invoke(HttpContext context)
        {
            string authHeader = context.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Basic "))
            {
                string encodedUsernamePassword = authHeader.Substring("Basic ".Length)?.Trim();
                Encoding encoding = Encoding.GetEncoding("iso-8859-1");

                if (encodedUsernamePassword == _options.AuthenticationString)
                {
                    await _next.Invoke(context);
                }
                else
                {
                    context.Response.StatusCode = 401;
                    return;
                }
            }
            else
            {
                context.Response.Headers.Add("message", "This service requires basic authentication"); 
                context.Response.StatusCode = 401;
                return;
            }
        }
    }
}
