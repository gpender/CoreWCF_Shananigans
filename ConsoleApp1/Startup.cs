using System;
using System.ServiceModel.Description;
using Autofac.Extensions.DependencyInjection;
using CoreWCF;
using CoreWCF.Configuration;
using dotConnected.WCF.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp1
{
    internal class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddServiceModelServices();
        }
        public void Configure(IApplicationBuilder app)
        {
            app.UseServiceModel(builder =>
            {
                builder.AddService<Service>();
                builder.AddServiceEndpoint<Service, IService>(new BasicHttpBinding(), "/DemoService/basicHttp");
            });
        }
    }
}