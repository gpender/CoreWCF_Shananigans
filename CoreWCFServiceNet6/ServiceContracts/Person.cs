using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace CoreWCFServiceNet6.ServiceContracts
{
	[DataContract]
	public class Person
	{
		public Person()
		{
				
		}
		[DataMember()]
		public string Name { get; set; } = "Guy";
		[DataMember()]
		public int Age { get; set; } = 55;

	}
}
