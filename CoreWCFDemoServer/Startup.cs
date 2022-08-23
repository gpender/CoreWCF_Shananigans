using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Configuration;
using CoreWCF.Description;

namespace CoreWCFDemoServer
{
    public class Startup
    {
        public const int HTTP_PORT = 5000;
        // Only used on case that UseRequestHeadersForMetadataAddressBehavior is not used
        public const string HOST_IN_WSDL = "localhost";

        public void ConfigureServices(IServiceCollection services)
        {
            // Enable CoreWCF Services, enable metadata
            // Use the Url used to fetch WSDL as that service endpoint address in generated WSDL 
            services.AddServiceModelServices()
                    .AddServiceModelMetadata()
                    .AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseServiceModel(builder =>
            {
                // Add the Echo Service
                builder.AddService<EchoService>(serviceOptions =>
	                {
		                // Set the default host name:port in generated WSDL and the base path for the address 
		                serviceOptions.BaseAddresses.Add(new Uri($"http://{HOST_IN_WSDL}/EchoService"));
	                })
	                // Add a BasicHttpBinding endpoint
	                .AddServiceEndpoint<EchoService, IEchoService>(new BasicHttpBinding(), "/basichttp");

                // Configure WSDL to be available over http & https
                var serviceMetadataBehavior = app.ApplicationServices.GetRequiredService<CoreWCF.Description.ServiceMetadataBehavior>();
                serviceMetadataBehavior.HttpGetEnabled = serviceMetadataBehavior.HttpsGetEnabled = true;
            });
        }
    }
}
