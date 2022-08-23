using System.Runtime.Serialization;
using System.ServiceModel;

[DataContract]
public class EchoFault
{
	private string text;

	[DataMember]
	public string Text
	{
		get { return text; }
		set { text = value; }
	}
}

[ServiceContract]
public interface IEchoService
{
	[OperationContract]
	string Echo(string text);

	[OperationContract]
	string ComplexEcho(EchoMessage echoMessage);

	[OperationContract]
	[FaultContract(typeof(EchoFault))]
	string FailEcho(string text);

	[OperationContract]
	string EchoForPermission(string text);
}

[DataContract]
public class EchoMessage
{
	[DataMember]
	public string Text { get; set; }
	[DataMember]
	public int Number { get; set; }
}