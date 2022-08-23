using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreWCFServiceNet6.ServiceContracts;
using dotConnected.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoreWCFServiceNet6.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TestController : ControllerBase
	{
		[HttpGet]
		public ServiceResponse<Person> GetPerson()
		{
			var serviceResponse = new ServiceResponse<Person>();
			return serviceResponse.FinaliseSuccess(new Person());
		}
	}
}
