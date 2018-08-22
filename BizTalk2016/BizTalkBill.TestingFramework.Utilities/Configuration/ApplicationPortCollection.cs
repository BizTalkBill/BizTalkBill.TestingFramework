using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizTalkBill.TestingFramework.Utilities.Configuration
{
    public class ApplicationPortCollection : ConfigurationElementCollection
    {
        public ApplicationPortElement this[int index]
        {
            get { return (ApplicationPortElement)BaseGet(index); }
        }

        public new ApplicationPortElement this[string key]
        {
            get { return (ApplicationPortElement)BaseGet(key); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ApplicationPortElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ApplicationPortElement)element).PortName;
        }

        public ApplicationPortElement Get(string applicationName, string portName)
        {
            for (var index = 0; index < this.Count; index++)
            {
                Debug.WriteLine("index = " + index.ToString());
                var item = this[index];
                if (portName.StartsWith("*"))
                {
                    if (item.ApplicationName == applicationName && item.PortName.EndsWith(portName.Replace("*","")))
                    {
                        return item;
                    }
                }
                if (portName.EndsWith("*"))
                {
                    if (item.ApplicationName == applicationName && item.PortName.StartsWith(portName.Replace("*","")))
                    {
                        return item;
                    }
                }
                if (item.ApplicationName == applicationName && item.PortName == portName)
                {
                    return item;
                }
            }
            return null;
        }
    }
}
