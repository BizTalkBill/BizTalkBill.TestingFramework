using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizTalkBill.TestingFramework.Helpers.EventLog
{
    public class ApplicationEventLog
    {
        public static Boolean GetErrors(List<string> sources)
        {
            Debug.WriteLine("BizTalkBill.TestingFramework.Helpers.EventLog:ApplicationEventLog:GetErrors(sources)");

            return false;
        }

        public static Boolean GetErrors(List<string> sources, DateTime LastApplicationLogEventDateTime)
        {
            Debug.WriteLine("BizTalkBill.TestingFramework.Helpers.EventLog:ApplicationEventLog:GetErrors(sources, LastApplicationLogEventDateTime)");

            System.Diagnostics.EventLog eventLog = new System.Diagnostics.EventLog("Application");
            foreach (EventLogEntry entry in eventLog.Entries)
            {
                if (entry.TimeWritten > LastApplicationLogEventDateTime)
                {
                    if (entry.EntryType == EventLogEntryType.Error)
                    {
                        if (sources.Contains(entry.Source))
                        {
                            Debug.WriteLine("Application EventLog Errors Found");
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static DateTime GetLastEventDateTime()
        {
            DateTime LastApplicationLogEventDateTime = new DateTime(1900, 01, 01);

            System.Diagnostics.EventLog eventLog = new System.Diagnostics.EventLog("Application");

            foreach (EventLogEntry entry in eventLog.Entries)
            {
                if (entry.TimeGenerated > LastApplicationLogEventDateTime)
                {
                    LastApplicationLogEventDateTime = entry.TimeGenerated;
                }
            }

            return LastApplicationLogEventDateTime;
        }

        public static void CheckForDiagnosticEvent(string message)
        {
        }

    }
}
