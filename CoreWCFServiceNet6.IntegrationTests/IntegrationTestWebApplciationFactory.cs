using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace CoreWCFServiceNet6.IntegrationTests
{
	public class IntegrationTest<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
	{
		protected override TestServer CreateServer(IWebHostBuilder builder)
		{
			var addresses = new ServerAddressesFeature();
			addresses.Addresses.Add("http://localhost:5000/");
			var features = new FeatureCollection();
			features.Set<IServerAddressesFeature>(addresses);

			var server = new TestServer(builder, features);
			return server;
		}

		protected override IWebHostBuilder CreateWebHostBuilder()
		{
			return WebHost.CreateDefaultBuilder()
				.UseStartup<TStartup>();
		}
	}
}
