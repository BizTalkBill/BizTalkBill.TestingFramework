using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace BizTalkBill.TestingFramework.Utilities.Configuration
{
    public class ApplicationPortElement : ConfigurationElement
    {
        /// <summary>
        /// The name of the port on the application
        /// </summary>
        [ConfigurationProperty("portName", IsKey = true, IsRequired = true)]
        public string PortName
        {
            get { return (string)this[@"portName"]; }
        }
        /// <summary>
        /// The name of the application
        /// </summary>
        [ConfigurationProperty("applicationName", IsRequired = true)]
        public string ApplicationName
        {
            get { return (string)this[@"applicationName"]; }
        }
        /// <summary>
        /// The name of the application
        /// </summary>
        [ConfigurationProperty("server", IsRequired = false)]
        public string Server
        {
            get { return (string)this[@"server"]; }
        }
        /// <summary>
        /// The port number to listen on
        /// </summary>
        [ConfigurationProperty("URL", IsRequired = true)]
        public string URL
        {
            get { return (string)this[@"URL"]; }
        }
        /// <summary>
        /// The prefix of the file
        /// </summary>
        [ConfigurationProperty("filePrefix", IsRequired = false)]
        public string FilePrefix
        {
            get { return (string)this[@"filePrefix"]; }
        }
        /// <summary>
        /// The suffix of the file
        /// </summary>
        [ConfigurationProperty("fileSuffix", IsRequired = false)]
        public string FileSuffix
        {
            get { return (string)this[@"fileSuffix"]; }
        }
    }
}
