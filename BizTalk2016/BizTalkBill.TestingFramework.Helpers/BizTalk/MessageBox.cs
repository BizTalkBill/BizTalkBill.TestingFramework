using Microsoft.BizTalk.Operations;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizTalkBill.TestingFramework.Helpers.BizTalk
{
    public class MessageBox
    {
        public static bool AnySuspendedInstances(string appName)
        {
            Trace.WriteLine("AnySuspendedInstances: " + appName);
            
            bool result = false;

            string dbServerName = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\BizTalk Server\3.0\Administration", "MgmtDBServer", "localhost");
            string dbDatabaseName = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\BizTalk Server\3.0\Administration", "MgmtDBName", "BizTalkMgmtDb");

            Trace.WriteLine("AnySuspendedInstances: dbServerName = " + dbServerName);
            Trace.WriteLine("AnySuspendedInstances: dbDatabaseName = " + dbDatabaseName);

            Microsoft.BizTalk.Operations.BizTalkOperations btsOps = new Microsoft.BizTalk.Operations.BizTalkOperations(dbServerName, dbDatabaseName);

            var serviceInstances = btsOps.GetServiceInstances();

            Int32 nSuspended = 0;
            foreach (MessageBoxServiceInstance instance in serviceInstances)
            {
                if ((instance.Application == appName) || (appName == "*"))
                {
                    if (instance.InstanceStatus == InstanceStatus.Suspended)
                    {
                        nSuspended++;
                    }
                    if (instance.InstanceStatus == InstanceStatus.SuspendedNotResumable)
                    {
                        nSuspended++;
                    }
                }
            }

            if (nSuspended > 0)
                result = true;

            return result;
        }
    }
}
