using System;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using CoreWCF;
using Newtonsoft.Json;

namespace dotConnected.Services
{
	/// <summary>
	/// Class wrapper for containing extra detail for service operations
	/// </summary>
	[DataContract()]
	[XmlSerializerFormat()]
	[Description("Contains additional information about the service operation request.")]
	public class ServiceResponseMeta
	{
		public ServiceResponseMeta()
		{
			// Default to OK
			ResultCode = ServiceOperationStatus.Success;
			StatusCode = HttpStatusCode.OK;


			//Log The Time
			Initiated = DateTime.UtcNow;
		}
		/// <summary>
		/// The client computer name 
		/// </summary>
		[DataMember()]
		//[DataMember(Name = "computerName")]
		[Description("The client computer name")]
		[XmlElement(ElementName = "ComputerName")]
		public string ComputerName { get; set; }


		/// <summary>
		/// A descriptive message for the service operation
		/// </summary>
		[DataMember()]
		//[DataMember(Name="message")]
		[Description("A descriptive message for the service operation")]
		public string Message { get; set; }

		/// <summary>
		/// The exception message is there is one
		/// </summary>
		[DataMember()]
		//[DataMember(Name= "exceptionMessage")]
		[Description("The exception message is there is one")]
		public string ExceptionMessage { get; set; }

		/// <summary>
		/// The templated result code for the service operation
		/// </summary>
		[DataMember()]
		//[DataMember(Name= "resultCode")]
		public ServiceOperationStatus ResultCode { get; set; }

		#region Properties
		[DataMember()]
		//[DataMember(Name = "initiated")]
		public DateTime Initiated { get; set; }
		[DataMember()]
		//[DataMember(Name = "initiatedFormatted")]
		public string InitiatedFormatted
		{
			get
			{
				return Initiated.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.CurrentUICulture);
			}
			set
			{

			}
		}
		//[DataMember()]
		//[JsonProperty("durationbaby")]
		[DataMember(Name = "durationbaby")]
		public TimeSpan Duration { get; set; }
		[DataMember()]
		//[DataMember(Name = "durationInSecs")]
		public decimal DurationInSecs { get; set; }

		/// <summary>
		/// The 3-digit HTTP Status-Code (e.g., 200)
		/// </summary>
		[DataMember()]
		//[DataMember(Name = "status")]
		[Description("The 3-digit HTTP Status-Code (e.g., 200)")]
		public string Status
		{
			get { return ((int)StatusCode).ToString(); }
			// ** Do not be tempted to remove these as it causes WCF to blow up and not tell you why!
			set
			{
				try
				{
					StatusCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), value);
				}
				catch (Exception)
				{
					StatusCode = HttpStatusCode.InternalServerError;
				}
			}
		}

		[DataMember()]
		//[DataMember(Name = "statusCode")]
		public HttpStatusCode StatusCode
		{
			get;
			set;
		}
		#endregion
	}
}
