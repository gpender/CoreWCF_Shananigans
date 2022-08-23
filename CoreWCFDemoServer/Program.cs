using Microsoft.AspNetCore;

namespace CoreWCFDemoServer
{
	class Program
	{
		static void Main(string[] args)
		{
			IWebHost host = CreateWebHostBuilder(args).Build();
			host.Run();
		}

		// Listen on 8088 for http, and 8443 for https, 8089 for NetTcp.
		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseKestrel(options => {
					options.ListenAnyIP(Startup.HTTP_PORT);
				})
				.UseStartup<Startup>();
	}
}