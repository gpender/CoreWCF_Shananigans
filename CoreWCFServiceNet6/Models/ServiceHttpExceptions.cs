using System;
using System.Net;

namespace dotConnected.Services
{
	public class ServiceHttpException : Exception
	{
		public ServiceHttpException()
			: base(string.Empty)
		{
			StatusCode = HttpStatusCode.OK;
		}

		public HttpStatusCode StatusCode { get; set; }
		public ServiceHttpException(HttpStatusCode statusCode)
			: base(string.Empty)
		{
			StatusCode = statusCode;
		}
		public ServiceHttpException(HttpStatusCode statusCode, string message)
			: base(message)
		{
			StatusCode = statusCode;
		}
	}

	/// <summary>
	/// An exception to represent Something not found, 
	/// </summary>
	public class NotFoundException : ServiceHttpException
	{
		private const HttpStatusCode ExceptionHttpStatusCode = HttpStatusCode.NotFound;

		public NotFoundException() : base(ExceptionHttpStatusCode) { }
		public NotFoundException(string message)
			: base(ExceptionHttpStatusCode, message)
		{
		}
	}
	/// <summary>
	/// An exception to represent a client not being authorised 
	/// </summary>
	public class NotAuthorisedException : ServiceHttpException
	{
		private const HttpStatusCode ExceptionHttpStatusCode = HttpStatusCode.Unauthorized;

		public NotAuthorisedException() : base(ExceptionHttpStatusCode) { }
		public NotAuthorisedException(string message)
			: base(ExceptionHttpStatusCode, message)
		{
		}
	}
	/// <summary>
	/// An exception to represent a client not being authorised 
	/// </summary>
	public class VersionMismatchException : ServiceHttpException
	{
		private const HttpStatusCode ExceptionHttpStatusCode = HttpStatusCode.Conflict;
		public VersionMismatchException() : base(ExceptionHttpStatusCode) { }
		public VersionMismatchException(string message)
			: base(ExceptionHttpStatusCode, message)
		{
		}
	}
	/// <summary>
	/// An exception to represent Something not found, 
	/// </summary>
	public class ServiceUnavailableException : ServiceHttpException
	{
		private const HttpStatusCode ExceptionHttpStatusCode = HttpStatusCode.ServiceUnavailable;

		public ServiceUnavailableException() : base(ExceptionHttpStatusCode) { }

		/// <summary>
		/// Constructor for NotFound Exception,
		///  An exception to represent Something not found, 
		/// </summary>
		/// <param name="message">Message displayed to the client</param>
		public ServiceUnavailableException(string message)
			: base(ExceptionHttpStatusCode, message)
		{
		}
	}

}
