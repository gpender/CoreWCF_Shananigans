using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CoreWCFDemoServer.IntegrationTests.Helpers
{
    /// <summary>
    /// Enables in-memory integration testing for CoreWCF (outside-in testing via <see cref="HttpClient"/>).
    ///
    /// Use these tests to exercise the entire HTTP stack, rather than create in-process ServiceModel channels.
    /// 
    /// <see href="https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-3.1"/>
    /// <seealso href="https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-2.1"/>
    /// </summary>
    /// <typeparam name="TStartup"></typeparam>
    public class WcfWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
	    private ILifetimeScope _container;

        //protected override TestServer CreateServer(IWebHostBuilder builder)
        //{
        //    var addresses = new ServerAddressesFeature();
        //    addresses.Addresses.Add("http://localhost/");
        //    var features = new FeatureCollection();
        //    features.Set<IServerAddressesFeature>(addresses); 

        //    var server = new TestServer(builder, features);
        //    return server;
        //}

		//protected override IWebHostBuilder CreateWebHostBuilder()
		//{
		//	SetSelfHostedContentRoot();

		//	return ServiceHelper.CreateWebHostBuilder<TStartup>();
		//}
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.ConfigureTestContainer<ContainerBuilder>(container =>
			{
				//container.RegisterType<MockClassicAeroService>().As<IClassicAeroService>();
				container.RegisterBuildCallback(BuildCallback);
			});


			builder.ConfigureTestServices(services =>
			{
				//services.Clear();//.AddSingleton(FakeCloudDatabase);
			});
		}
		private void BuildCallback(ILifetimeScope obj)
		{
			_container = obj;
		}
        protected override IHost CreateHost(IHostBuilder builder)
		{
			builder.UseServiceProviderFactory(new CustomServiceProviderFactory());
			return base.CreateHost(builder);
		}

        private static void SetSelfHostedContentRoot()
        {
            var contentRoot = Directory.GetCurrentDirectory();
            var assemblyName = typeof(WcfWebApplicationFactory<TStartup>).Assembly.GetName().Name;
            var settingSuffix = assemblyName.ToUpperInvariant().Replace(".", "_");
            var settingName = $"ASPNETCORE_TEST_CONTENTROOT_{settingSuffix}";
            Environment.SetEnvironmentVariable(settingName, contentRoot);
        }
    }



    /// <summary>
    /// Based upon https://github.com/dotnet/aspnetcore/issues/14907#issuecomment-620750841 - only necessary because of an issue in ASP.NET Core
    /// </summary>
    public class CustomServiceProviderFactory : IServiceProviderFactory<ContainerBuilder>
    {
	    private AutofacServiceProviderFactory _wrapped;
	    private IServiceCollection _services;

	    public CustomServiceProviderFactory()
	    {
		    _wrapped = new AutofacServiceProviderFactory();
	    }

	    public ContainerBuilder CreateBuilder(IServiceCollection services)
	    {
		    // Store the services for later.
		    _services = services;

		    return _wrapped.CreateBuilder(services);
	    }

	    public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
	    {
		    var sp = _services.BuildServiceProvider();
#pragma warning disable CS0612 // Type or member is obsolete
		    var filters = sp.GetRequiredService<IEnumerable<IStartupConfigureContainerFilter<ContainerBuilder>>>();
#pragma warning restore CS0612 // Type or member is obsolete

		    foreach (var filter in filters)
		    {
			    filter.ConfigureContainer(b => { })(containerBuilder);
		    }

		    return _wrapped.CreateServiceProvider(containerBuilder);
	    }
    }
}
