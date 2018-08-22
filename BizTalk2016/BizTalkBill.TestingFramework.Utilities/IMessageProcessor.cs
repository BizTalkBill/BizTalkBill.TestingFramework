using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizTalkBill.TestingFramework.Utilities
{

        public interface IMessageProcessor
        {
            string ExecuteWithResponse(string message);

            void Execute(string message);

            bool ReturnsResponse { get; }
        }
 
}
