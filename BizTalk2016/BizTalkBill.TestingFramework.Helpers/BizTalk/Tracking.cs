using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizTalkBill.TestingFramework.Helpers.BizTalk
{
    public class Tracking
    {

        public static string GetLastMessageTrackingDateTime()
        {
            Debug.WriteLine("BizTalkBill.TestingFramework.Helpers.BizTalk:Tracking:GetLastMessageTrackingDateTime()");

            string LastMessageTrackingDateTime = new DateTime(1900, 01, 01).ToString("yyyy-MM-ddTHH:mm:ss.fff");

            string dbServerName = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\BizTalk Server\3.0\Administration", "MgmtDBServer", "localhost");

            using (SqlConnection connection = new SqlConnection("Data Source=" + dbServerName + ";Initial Catalog=BizTalkDTADb;Integrated Security=True"))
            {
                string sCommand = "SELECT TOP 1 convert(varchar(50),[Event/Timestamp],126) FROM [BizTalkDTADb].[dbo].[dtav_MessageFacts] order by [Event/Timestamp] desc";
                using (SqlCommand command = new SqlCommand(sCommand, connection))
                {
                    try
                    {
                        connection.Open();
                        LastMessageTrackingDateTime = (string)command.ExecuteScalar();
                    }
                    catch (Exception)
                    {
                        //throw;
                    }
                }

            }

            Debug.WriteLine("LastMessageTrackingDateTime - " + LastMessageTrackingDateTime);

            return LastMessageTrackingDateTime;
        }
    }
}
