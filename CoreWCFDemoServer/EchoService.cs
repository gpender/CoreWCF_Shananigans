using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using CoreWCF;

namespace CoreWCFDemoServer;

[DataContract]
public class EchoFault
{
	[AllowNull]
	private string _text;

	[DataMember]
	[AllowNull]
	public string Text
	{
		get { return _text; }
		set { _text = value; }
	}
}

[ServiceContract]
public interface IEchoService
{
	[OperationContract]
	string Echo(string text);

	[OperationContract]
	string ComplexEcho(EchoMessage text);

	[OperationContract]
	[FaultContract(typeof(EchoFault))]
	string FailEcho(string text);

}

[DataContract]
public class EchoMessage
{
	[AllowNull]
	[DataMember]
	public string Text { get; set; }
}

public class EchoService : IEchoService
{
	public string Echo(string text)
	{
		System.Console.WriteLine($"Received {text} from client!");
		return text;
	}

	public string ComplexEcho(EchoMessage text)
	{
		System.Console.WriteLine($"Received {text.Text} from client!");
		return text.Text;
	}

	public string FailEcho(string text)
		=> throw new FaultException<EchoFault>(new EchoFault() { Text = "WCF Fault OK" }, new FaultReason("FailReason"));

}