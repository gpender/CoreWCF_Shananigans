using System.ServiceModel;
using System.Threading.Tasks;

public class EchoService : IEchoService
{
	public string Echo(string text)
	{
		//System.Console.WriteLine($"Received {text} from client!");
		return text;
	}
	public string Echo2Things(string text1, string text2)
	{
		//System.Console.WriteLine($"Received {text1} & {text2} from client!");
		return text1 +text2;
	}

	public string ComplexEcho(EchoMessage echoMessage)
	{
		//System.Console.WriteLine($"Received {echoMessage.Text} from client!");
		return echoMessage.Text;
	}
	public string FailEcho(string text)
	{
		throw new FaultException<EchoFault>(new EchoFault() { Text = "WCF Fault OK" }, new FaultReason("FailReason"));
	}
            

	//[AuthorizeRole("CoreWCFGroupAdmin")]
	public string EchoForPermission(string echo)
	{
		return echo;
	}

}