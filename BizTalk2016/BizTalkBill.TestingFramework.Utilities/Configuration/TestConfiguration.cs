using BizTalkBill.TestingFramework.Utilities.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizTalkBill.TestingFramework.Utilities.Configuration
{
    public class TestConfiguration : ConfigurationSection
    {
        public static TestConfiguration Current
        {
            get
            {
                var current = ConfigurationManager.GetSection("BizTalkBill.TestingFramework.Utilities") as TestConfiguration;
                if (current == null)
                {
                    Debug.WriteLine("Did Not Find Section");

                    new Logger().Write("The BizTalkBill.TestingFramework.Utilities configuration section has not been specified");
                }
                else
                {
                    Debug.WriteLine("Found Section");
                }
                return current;
            }
        }

        /// <summary>
        /// The type for the default message handler if no other handler deals with the message
        /// </summary>
        [ConfigurationProperty("SendPorts")]
        public ApplicationPortCollection SendPorts
        {
            get { return (ApplicationPortCollection)this[@"SendPorts"]; }
        }

        [ConfigurationProperty("ReceivePorts")]
        public ApplicationPortCollection ReceivePorts
        {
            get { return (ApplicationPortCollection)this[@"ReceivePorts"]; }
        }
    }
}
