using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.SignalR;
using TwitterStreamerApi;

namespace TwitterStreamerApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.Configure<Options.ApplicationConfiguration>(Configuration.GetSection(nameof(Options.ApplicationConfiguration)));
            services.Configure<Options.TwitterApiConfiguration>(Configuration.GetSection(nameof(Options.TwitterApiConfiguration)));
            services.AddTransient<Repositories.Interfaces.ITwitterStreamer, Repositories.TwitterStreamer>();

            //Change to transient
            services.AddSingleton<Repositories.Interfaces.IUserDataManager, Repositories.Development.UserDataManager_Dev>();

            //Change to specific cors
            services.AddCors(o => o.AddPolicy("GlobalCorsPolicy", builder =>
            {
                builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().AllowCredentials();
            }));

            services.AddSignalR();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            app.UseCors("GlobalCorsPolicy");

            app.UseSignalR((routes) =>
            {
                routes.MapHub<SignalHubs.TwitterStreamerHub>("/twitterStreamerHub");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseMiddleware<Middleware.ApplicationAuthentication>();
            app.UseMvc();
        }
    }
}
