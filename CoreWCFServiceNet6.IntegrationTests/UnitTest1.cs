using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CoreWCFServiceNet6.IntegrationTests
{
	public class UnitTest1 : IClassFixture<WebApplicationFactory<Startup>>
	{
		private readonly WebApplicationFactory<Startup> _factory;
		private readonly HttpClient _httpClient;

		public UnitTest1(WebApplicationFactory<Startup> factory)
		{
			_httpClient = factory.CreateClient();
		}
		
		[Fact]
		public async Task Test1()
		{
			var request = @"
<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
    <s:Body> 
         <Echo xmlns = ""http://tempuri.org/"">
            <text>hello world</text>
        </Echo>
    </s:Body>
</s:Envelope>";

			var content = new StringContent(request, Encoding.UTF8);
			content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");

			HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, "/EchoService/basicHttp");

			message.Headers.Add("SOAPAction", "http://tempuri.org/IEchoService/Echo");
			message.Content = content;

			using (var client = _factory.CreateClient())
			{
				var response = await client.SendAsync(message);
				var details = await response.Content.ReadAsStringAsync();

				response.EnsureSuccessStatusCode();
			}
			//var response = _httpClient.GetAsync("http://localhost:5000/testservice.svc")
		}
	}
}
