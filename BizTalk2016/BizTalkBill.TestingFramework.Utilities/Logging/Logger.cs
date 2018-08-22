using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizTalkBill.TestingFramework.Utilities.Logging
{
    public interface ILog
    {
        void Write(string message);
    }
    public class Logger : ILog
    {
        public void Write(string message)
        {
            Trace.WriteLine(message);
            Console.WriteLine(message);
        }
    }

}
