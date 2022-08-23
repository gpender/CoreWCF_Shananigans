
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using dotConnected.Extensions;
using Newtonsoft.Json;

// This is for backwards compatibility with the one from Data.Types.Services
using ServiceStatus = dotConnected.Services.ServiceOperationStatus;

namespace dotConnected.Services
{
	/// <summary>
	/// A simple class wrapper for service response containing the response payload object and status information
	/// </summary>
	/// <typeparam name="TServiceResult">The result of the service operation</typeparam>
	/// <typeparam name="TResultCode">Service operation specfic result codes, this must be an enum</typeparam>
	[Description("Service response is a simple envelope containing the response object and status information")]
	[DataContract]
	public class ServiceResponse<TServiceResult> : ServiceResponse
	{
		public ServiceResponse() : base()
		{
		}

		public ServiceResponse(Exception ex, string friendlyMessage = null) : base()
		{
			Requires.NotNull(ex, "ex");

			Meta = new ServiceResponseMeta { ExceptionMessage = ex.Message, StatusCode = HttpStatusCode.InternalServerError, Message = friendlyMessage ?? "Missing Message" };
		}

		/// <summary>
		/// The result of the service operation
		/// </summary>
		[DataMember()]//Name = "serviceResult")]
		//[DataMember(Name = "serviceResult")]
		[Description("The payload of the service operation")]
		[JsonProperty(Required = Required.AllowNull)]
		public TServiceResult ServiceResult { get; set; }

		/// <summary>
		/// This is the payload of the service method that was called
		/// </summary>
		[IgnoreDataMember]
		[JsonIgnore]
		public TServiceResult Response { get => ServiceResult; set => ServiceResult = value; }


		#region Methods
		public ServiceResponse<TServiceResult> FinaliseSuccess(TServiceResult response, string message = null)
		{
			Meta.Message = message;
			ServiceResult = response;
			Meta.StatusCode = HttpStatusCode.OK;
			Meta.Duration = DateTime.UtcNow - Meta.Initiated;
			Meta.DurationInSecs = (decimal)Meta.Duration.TotalSeconds;
			Meta.ComputerName = Environment.MachineName;
			Meta.ResultCode = ServiceOperationStatus.Success;


			return this;
		}

		public ServiceResponse<TServiceResult> FinaliseError(Exception exception)
		{
			var exBuilder = new StringBuilder();

			if (exception == null)
				return FinaliseError(string.Empty);

			Meta.Message = exception.Message;

			if (exception is ServiceHttpException)
			{
				Meta.StatusCode = ((ServiceHttpException)exception).StatusCode;
			}
			else
				Meta.StatusCode = HttpStatusCode.InternalServerError;

			exBuilder = WalkException(exBuilder, exception, 0);
			Meta.ExceptionMessage = exBuilder.ToString();

			Meta.Duration = DateTime.UtcNow - Meta.Initiated;
			Meta.DurationInSecs = (decimal)Meta.Duration.TotalSeconds;

			Meta.ComputerName = Environment.MachineName;
			Meta.ResultCode = ServiceOperationStatus.Error;

			return this;
		}

		public ServiceResponse<TServiceResult> FinaliseError(string message)
		{

			Meta.Message = message;

			Meta.Duration = DateTime.UtcNow - Meta.Initiated;
			Meta.DurationInSecs = (decimal)Meta.Duration.TotalSeconds;
			Meta.ComputerName = Environment.MachineName;
			Meta.ResultCode = ServiceOperationStatus.Error;
			return this;
		}

		[IgnoreDataMember]
		[JsonIgnore]
		public override string InitiatedFormatted
		{
			get
			{
				return Initiated.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.CurrentUICulture);
			}
			set
			{

			}
		}
		#endregion

		#region protected methods
		protected StringBuilder WalkException(StringBuilder exBuilder, Exception ex, int level)
		{
			if (ex == null)
				return exBuilder;
			else
			{
				var indentString = new string(' ', level);
				exBuilder.AppendFormat("{0}{1}", indentString, ex.Message).AppendLine();
				exBuilder.AppendFormat("{0}{1}", indentString, ex.Source).AppendLine();
				exBuilder.AppendFormat("{0}{1}", indentString, ex.StackTrace).AppendLine();
				exBuilder.AppendLine();
				return WalkException(exBuilder, ex.InnerException, level + 1);
			}
		}
		#endregion
	}

	[DataContract]
	public class ServiceResponse
	{
		public ServiceResponse()
		{
			Meta = new ServiceResponseMeta();
		}

		#region Properties
		[IgnoreDataMember]
		[JsonIgnore]
		public DateTime Initiated { get => Meta.Initiated; set => Meta.Initiated = value; }

		public virtual string InitiatedFormatted
		{
			get
			{
				return Initiated.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.CurrentUICulture);
			}
			set
			{

			}
		}

		[IgnoreDataMember]
		[JsonIgnore]
		public TimeSpan Duration { get => Meta.Duration; set => Meta.Duration = value; }

		[IgnoreDataMember]
		[JsonIgnore]

		public decimal DurationInSecs { get => Meta.DurationInSecs; set => Meta.DurationInSecs = value; }

		/// <summary>
		/// This is here only to provide compatibility with Arinc.Data.Types.Services version of ServiceResponse
		/// </summary>
		[IgnoreDataMember]
		[JsonIgnore]
		public ServiceOperationStatus Status { get => Meta.ResultCode; set => Meta.ResultCode = value; }

		/// <summary>
		/// This is here only to provide compatibility with Arinc.Data.Types.Services version of ServiceResponse
		/// </summary>
		[IgnoreDataMember]
		[JsonIgnore]
		public string ErrorMessage { get => Meta.ExceptionMessage; set => Meta.ExceptionMessage = value; }

		/// <summary>
		/// This is here only to provide compatibility with Arinc.Data.Types.Services version of ServiceResponse
		/// </summary>
		[IgnoreDataMember]
		[JsonIgnore]
		public string Message { get => Meta.Message; set => Meta.Message = value; }

		//[DataMember()]
		[DataMember(Name = "guysmeta")]
		[JsonProperty("guysmeta")]
		public ServiceResponseMeta Meta { get; set; }
		#endregion
	}
}
