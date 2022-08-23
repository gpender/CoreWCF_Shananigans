using ServiceReference1;
using System;

namespace WcfClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var asc = new AuditingServiceClient(new AuditingServiceClient.EndpointConfiguration(), "http://localhost:8088/Auditing.Services/WCFAuditingService.svc");

            var guy = asc.GetAuditSources();
        }
    }
}
