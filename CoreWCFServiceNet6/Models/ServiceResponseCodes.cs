using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotConnected.Services
{

	public enum GenericResultCode
	{
		Unknown = 0,
		Success = 1,
		ServiceNotAvailable = 2
	}

	/// <summary>
	/// Some extras added to deal with the merging of other "ServiceOperationStatus" with slightly different members!?
	/// </summary>
	public enum ServiceOperationStatus
	{
		Unknown = 0,
		Success = 1,
		Error = 3,
		Failed = 4,
		Unavailable = 5,
		NotAvailable = 6,
		Unauthorised = 7
	}
}
