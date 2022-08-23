using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CoreWCFDemoServer.IntegrationTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CoreWCFDemoServer.IntegrationTests
{
	public class EchoServiceTest : IClassFixture<WcfWebApplicationFactory<Startup>>
	{
		private readonly WcfWebApplicationFactory<Startup> _factory;
		private readonly HttpClient client;

		public EchoServiceTest(WcfWebApplicationFactory<Startup> factory)
		{
			_factory = factory;
			_factory.ClientOptions.BaseAddress = new Uri("http://localhost:5000/");
			//client = factory.CreateClient();
		}

		[Fact]
		public async Task Echo_ShouldReturnOK()
		{
			var client = _factory.CreateClient();
			const string action = "http://tempuri.org/IEchoService/Echo";

			var request = new HttpRequestMessage(HttpMethod.Post, new Uri("/EchoService/basichttp", UriKind.RelativeOrAbsolute));
			request.Headers.TryAddWithoutValidation("SOAPAction", $"\"{action}\"");

			const string requestBody = @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/"">
			   <s:Header/>
			   <s:Body>
			      <tem:Echo>
			         <tem:text>A</tem:text>
			      </tem:Echo>
			   </s:Body>
			</s:Envelope>";

			request.Content = new StringContent(requestBody, Encoding.UTF8, "text/xml");

			// FIXME: Commenting out this line will induce a chunked response, which will break the pre-read message parser
			request.Content.Headers.ContentLength = Encoding.UTF8.GetByteCount(requestBody);

			var response = await client.SendAsync(request);
			response.StatusCode.Should().Be(HttpStatusCode.OK);

			var responseBody = await response.Content.ReadAsStringAsync();

			const string expected = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\">" +
			                        "<s:Body>" +
			                        "<EchoResponse xmlns=\"http://tempuri.org/\">" +
			                        "<EchoResult>A</EchoResult>" +
			                        "</EchoResponse>" +
			                        "</s:Body>" +
			                        "</s:Envelope>";
			responseBody.Should().Be(expected);
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

			HttpRequestMessage
				message = new HttpRequestMessage(HttpMethod.Post, "http://localhost5000/EchoService/basicHttp");

			message.Headers.Add("SOAPAction", "http://tempuri.org/IEchoService/Echo");
			message.Content = content;

			//using (var client = _factory.CreateClient())
			//{
			var response = await client.SendAsync(message);
			var details = await response.Content.ReadAsStringAsync();

			response.EnsureSuccessStatusCode();
		}
	}

}
