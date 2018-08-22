using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using BizTalkBill.TestingFramework.Helpers.BizTalk;
using BizTalkBill.TestingFramework.Helpers.EventLog;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;


namespace BizTalkBill.TestingFramework.SpecFlow.SharedBindings
{
    [Binding]
    public class EventLogSteps
    {
        [Then(@"there will be no errors in the application eventlog")]
        public void ThenThereWillBeNoErrorsInTheApplicationEventog()
        {
            var btsServer = Config.BizTalkServerName;
            var sources = new List<string>();
            sources.Add("BizTalk Server");

            Debug.WriteLine("Looging for Events");

            DateTime LastApplicationLogEventDateTime = (DateTime)ScenarioContext.Current["LastApplicationLogEventDateTime"];

            Debug.WriteLine("LastApplicationLogEventDateTime = " + LastApplicationLogEventDateTime.ToLongTimeString());

            var errorsExist = ApplicationEventLog.GetErrors(sources, LastApplicationLogEventDateTime);

            Debug.WriteLine("errorsExist = " + errorsExist.ToString());

            Assert.IsFalse(errorsExist,"Errors found in the Application Event Log");
        }
    }
}
