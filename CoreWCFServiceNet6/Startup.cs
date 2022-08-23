using Autofac;
using Autofac.Extensions.DependencyInjection;
using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using CoreWCFServiceNet6.ServiceContracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Steeltoe.Extensions.Configuration.Placeholder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Autofac.Diagnostics;
using dotConnected.Swagger.Filters;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;

namespace CoreWCFServiceNet6
{
	public class Startup
	{
		private IContainer? AutofacContainer { get; set; }

		private readonly IConfiguration _configuration;

		public Startup(IConfiguration configuration)
		{
			_configuration = configuration.AddPlaceholderResolver();
		}

		public void ConfigureServices(IServiceCollection services)
		{
			// Register logging for our service using Serilog instead of the default .net logger
			services.AddLogging(builder =>
			{
				Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(_configuration).CreateLogger();
			});

			// WCF services
			services.AddServiceModelServices();
			services.AddServiceModelMetadata();

			services.AddTransient<ITestService, TestService>();


			services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();




			// WebApi services
			// AddNewtonSoftJson ensures that WebApi Controllers observe the IgnoreDataMember attributes on serialised objects (SEE BELOW FOR SCHEMA)
			//services.AddControllers().AddNewtonsoftJson().AddJsonOptions(options =>
			//{
			//	options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
			//});

			//services.AddSwaggerGen(options =>
			//{
			//	options.SwaggerDoc("v1", new OpenApiInfo { Title = "Pegasus Security Service", Version = "v1" });

			//	options.MapType<TimeSpan>(() => new OpenApiSchema { Type = "string", Format = "time-span" });

			//	// Add SwaggerDataAttributeFilter to ensure Swagger Schema observes IgnoreDataMember attributes.(SEE ABOVE FOR DATA)
			//	options.SchemaFilter<SwaggerJsonAttributeFilter>();
			//	options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
			//	{
			//		Name = "Authorization",
			//		Type = SecuritySchemeType.ApiKey,
			//		Scheme = "Bearer",
			//		BearerFormat = "JWT",
			//		In = ParameterLocation.Header,
			//		Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
			//	});
			//	options.AddSecurityRequirement(new OpenApiSecurityRequirement
			//	{
			//		{
			//			new OpenApiSecurityScheme
			//			{
			//				Reference = new OpenApiReference
			//				{
			//					Type = ReferenceType.SecurityScheme,
			//					Id = "Bearer"
			//				}
			//			},
			//			new string[] {}
			//		}
			//	});
			//});

			// In CoreWCF, the serializers write to streams synchronously, so you need to enable synchronous IO in ASP.NET Core
			// https://githubhot.com/repo/CoreWCF/CoreWCF/issues/535
			// If using Kestrel:
			services.Configure<KestrelServerOptions>(options =>
			{
				options.AllowSynchronousIO = true;
			});

			// If using IIS:
			services.Configure<IISServerOptions>(options =>
			{
				options.AllowSynchronousIO = true;
			});
		}
		//public void ConfigureContainer(ContainerBuilder builder)
		//{
		//	builder.RegisterBuildCallback(ContainerBuildEvent);
		//	builder.RegisterType<Person>();
		//	builder.RegisterType<TestService>();
		//	builder.RegisterType<TestService>().As<ITestService>();
		//	builder.RegisterInstance(Log.Logger).AsImplementedInterfaces();
		//}

		//private void ContainerBuildEvent(ILifetimeScope container)
		//{
		//	// This callback is simply a way to set the AutofacContainer property which can then be used to configure the WCF to use Autofac DI
		//	AutofacContainer = container as IContainer;



		//	var tracer = new DefaultDiagnosticTracer();
		//	tracer.OperationCompleted += (sender, args) =>
		//	{
		//		Console.WriteLine(args.TraceContent);
		//	};

		//	var cont = container as IContainer;
		//	cont.SubscribeToDiagnostics(tracer);

		//}
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			var serviceConfig = _configuration.GetSection("WcfServiceConfig").Get<WcfServiceConfig>();
			Log.Logger.Debug("Using ServiceBaseAddress of {} for TestService.svc", serviceConfig.ServiceBaseAddress);
			
			//app.UseSwagger();
			//app.UseSwaggerUI(options =>
			//{
			//	options.SwaggerEndpoint("/swagger/v1/swagger.json", "Test Service");
			//	options.EnablePersistAuthorization();
			//});

			app.UseServiceModel(builder =>
			{
				AddWcfService<TestService, ITestService>(builder, serviceConfig, "TestService.svc");
			});
			//app.UseRouting();
			//app.UseAuthorization();
			//app.UseEndpoints(endpoints =>
			//{
			//	endpoints.MapControllers();
			//});

		}

		private void AddWcfService<T, U>(IServiceBuilder builder, WcfServiceConfig serviceConfig, string endPoint) where T : class
		{
			builder.AddService<T>(options =>
			{
				options.DebugBehavior.HttpHelpPageEnabled = true;
				options.BaseAddresses.Add(new Uri(serviceConfig.ServiceBaseAddress ?? throw new ArgumentException("ServiceBaseAddress not specified")));
			})
			.AddServiceEndpoint<T, U>(
				new BasicHttpBinding() { MaxBufferSize = 2147483647, MaxReceivedMessageSize = 2147483647 },
				endPoint)
			.ConfigureServiceHostBase<T>(serviceHost =>
			{
				//serviceHost.Description.Behaviors.Add(
				//	new DependencyInjectionServiceBehavior(new AutofacServiceProvider(AutofacContainer)));
				var serviceMetadataBehavior = serviceHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
				if (serviceMetadataBehavior == null)
					serviceMetadataBehavior = new ServiceMetadataBehavior();
				serviceMetadataBehavior.HttpGetEnabled = true;
			});
		}
	}
}
