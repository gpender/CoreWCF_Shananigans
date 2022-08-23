using CoreWCF;
using dotConnected.Services;

namespace CoreWCFServiceNet6.ServiceContracts
{
	[ServiceContract]
	public interface ITestService
	{
		[OperationContract]
		ServiceResponse<Person> GetPerson();
		[OperationContract]
		int GetCount();
	}
}