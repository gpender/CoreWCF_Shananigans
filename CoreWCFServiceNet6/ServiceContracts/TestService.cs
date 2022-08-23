using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotConnected.Services;

namespace CoreWCFServiceNet6.ServiceContracts
{
	public class TestService : ITestService
	{
		public TestService()
		{
				
		}
		public ServiceResponse<Person> GetPerson()
		{
			var serviceResponse = new ServiceResponse<Person>();
			return serviceResponse.FinaliseSuccess(new Person());
		}

		public int GetCount()
		{
			return 99;
		}
	}
}
