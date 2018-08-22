using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BizTalkBill.TestingFramework.Helpers.IO
{
    public class WCFHelper
    {
        public static string BasicHTTPTwoWay(Assembly asm, string resourceName, string url)
        {
            using (Stream resource = asm.GetManifestResourceStream(resourceName))
            {
                if (resource == null)
                {
                    throw new ArgumentException("No such resource", "resourceName");
                }

                MessageVersion mv = MessageVersion.Soap11; ;

                Binding binding = new BasicHttpBinding(); 

                Message msg = Message.CreateMessage(mv, "*", XmlReader.Create(resource));

                IUniversalTwoWayContract proxy =
                        new ChannelFactory<IUniversalTwoWayContract>(binding, url).CreateChannel();

                Message response = proxy.SubmitMessage(msg);

                return response.ToString();
            }
        }

    }

    [ServiceContract]
    public interface IUniversalTwoWayContract
    {
        [OperationContract(Action = "*", ReplyAction = "*")]
        Message SubmitMessage(Message inmsg);
    }

}
