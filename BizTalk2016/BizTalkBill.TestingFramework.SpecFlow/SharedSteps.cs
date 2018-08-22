using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using BizTalkBill.TestingFramework.Helpers;
using BizTalkBill.TestingFramework.Helpers.IO;
using BizTalkBill.TestingFramework.Helpers.EventLog;
using BizTalkBill.TestingFramework.Helpers.BizTalk;
using System.IO;
using System.Diagnostics;
using BizTalkBill.TestingFramework.Utilities.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using Microsoft.Win32;
using System.Reflection;
using System.Xml;

namespace BizTalkBill.TestingFramework.SpecFlow
{
    [Binding]
    public class SharedSteps
    {
        [Given(@"the system is in a clean state")]
        public void GivenTheSystemIsInACleanState()
        {
            Logger logger = new Logger();

            // need to delete suspend instances

            ScenarioContext.Current.Add("LastMessageTrackingDateTime", Tracking.GetLastMessageTrackingDateTime());
            ScenarioContext.Current.Add("LastApplicationLogEventDateTime", ApplicationEventLog.GetLastEventDateTime());
        }

        [Then(@"BizTalk will process request/response on port '(.*)'")]
        public void ThenBizTalkWillProcessRequestResponseOnPort(string portName)
        {
            ThenBizTalkWillReceiveTheMessage(portName);
            ThenBizTalkWillSendTheMessage(portName);
        }

        [Then(@"BizTalk will receive the message on port '(.*)'")]
        public void ThenBizTalkWillReceiveTheMessage(string portName)
        {
            Debug.WriteLine("BizTalkBill.TestingFramework.SpecFlow:SharedSteps:ThenBizTalkWillReceiveTheMessage(" + portName + ")");

            ApplicationEventLog.CheckForDiagnosticEvent("BizTalk has received a message on port - " + portName);

            System.Threading.Thread.Sleep(100);

            Int32 ReceiveCount = 0;
            string LastMessageTrackingDateTime = (string)ScenarioContext.Current["LastMessageTrackingDateTime"];
            new Logger().Write("LastMessageTrackingDateTime = " + LastMessageTrackingDateTime);

            string dbServerName = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\BizTalk Server\3.0\Administration", "MgmtDBServer", "localhost");

            using (SqlConnection connection2 = new SqlConnection("Data Source=" + dbServerName + ";Initial Catalog=BizTalkDTADb;Integrated Security=True"))
            {
                string sCommand2 = string.Format("SELECT TOP 1 convert(varchar(50),[Event/Timestamp],126)  FROM [BizTalkDTADb].[dbo].[dtav_MessageFacts] where [Event/Port] = '{0}' order by [Event/Timestamp] desc", portName);
                new Logger().Write("SQL Command = " + sCommand2);

                using (SqlCommand command2 = new SqlCommand(sCommand2, connection2))
                {
                    try
                    {
                        connection2.Open();

                        var lastfound = (string)command2.ExecuteScalar();

                        if (lastfound != null)
                        {
                            new Logger().Write("LastMessageFoundTrackingDateTime = " + lastfound);
                        }
                        else
                        {
                            new Logger().Write("returned null");
                        }
                    }
                    catch (Exception Ex1)
                    {
                        new Logger().Write("Error in LastMessageFoundTrackingDateTime - " + Ex1.ToString());
                        //throw;
                    }
                }
            }
            using (SqlConnection connection = new SqlConnection("Data Source=" + dbServerName + ";Initial Catalog=BizTalkDTADb;Integrated Security=True"))
            {
                string sCommand = string.Format("select count(*) FROM [BizTalkDTADb].[dbo].[dtav_MessageFacts] where [Event/Timestamp] > '{0}' and [Event/Port] = '{1}'", LastMessageTrackingDateTime, portName);
                new Logger().Write("SQL Command = " + sCommand);

                new Logger().Write("Starting Receive Check = " + DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                for (int i = 1; i <= 20; i++)
                {
                    using (SqlCommand command = new SqlCommand(sCommand, connection))
                    {
                        try
                        {
                            connection.Open();
                            ReceiveCount = (Int32)command.ExecuteScalar();
                            connection.Close();

                            new Logger().Write("ReceiveCount = " + ReceiveCount.ToString());
                        }
                        catch (Exception Ex1)
                        {
                            new Logger().Write("Exception = " + Ex1.Message);
                            //throw;
                        }
                    }
                    new Logger().Write("Ending Receive Check = " + DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fff"));

                    if (ReceiveCount > 0)
                    {
                        Debug.WriteLine("Break");
                        break;
                    }
                    Debug.WriteLine("Looping");
                    if (i <= 5)
                    {
                        System.Threading.Thread.Sleep(500);
                    }
                    else
                    {
                        Int32 sleepTime = 3000 * (i - 5);
                        System.Threading.Thread.Sleep(sleepTime);
                    }
                }

            }

            Assert.IsFalse(ReceiveCount < 1, "Message Not Received on Port - " + portName);
        }

        [Then(@"BizTalk will send the message on port '(.*)'")]
        public void ThenBizTalkWillSendTheMessage(string portName)
        {
            Debug.WriteLine("BizTalkBill.TestingFramework.SpecFlow:SharedSteps:ThenBizTalkWillSendTheMessage(" + portName + ")");

            ApplicationEventLog.CheckForDiagnosticEvent("BizTalk has Sent a message on port - " + portName);

            System.Threading.Thread.Sleep(100);

            Int32 SendCount = 0;
            string LastMessageTrackingDateTime = (string)ScenarioContext.Current["LastMessageTrackingDateTime"];
            new Logger().Write("LastMessageTrackingDateTime = " + LastMessageTrackingDateTime);

            string dbServerName = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\BizTalk Server\3.0\Administration", "MgmtDBServer", "localhost");

            using (SqlConnection connection2 = new SqlConnection("Data Source=" + dbServerName + ";Initial Catalog=BizTalkDTADb;Integrated Security=True"))
            {
                string sCommand2 = string.Format("SELECT TOP 1 [Event/Timestamp]  FROM [BizTalkDTADb].[dbo].[dtav_MessageFacts] where [Event/Port] = '{0}' order by [Event/Timestamp] desc", portName);
                new Logger().Write("SQL Command = " + sCommand2);

                using (SqlCommand command2 = new SqlCommand(sCommand2, connection2))
                {
                    try
                    {
                        connection2.Open();

                        var lastfound = (DateTime)command2.ExecuteScalar();

                        if (lastfound != null)
                        {
                            new Logger().Write("LastMessageFoundTrackingDateTime = " + lastfound.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                        }
                        else
                        {
                            new Logger().Write("returned null");
                        }
                    }
                    catch (Exception Ex1)
                    {
                        new Logger().Write("Error in LastMessageFoundTrackingDateTime - " + Ex1.ToString());
                        //throw;
                    }
                }
            }
            using (SqlConnection connection = new SqlConnection("Data Source=" + dbServerName + ";Initial Catalog=BizTalkDTADb;Integrated Security=True"))
            {
                string sCommand = string.Format("select count(*) FROM [BizTalkDTADb].[dbo].[dtav_MessageFacts] where [Event/Timestamp] > '{0}' and [Event/Port] = '{1}'", LastMessageTrackingDateTime, portName);
                new Logger().Write("SQL Command = " + sCommand);

                new Logger().Write("Starting Send Check = " + DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                for (int i = 1; i <= 20; i++)
                {
                    using (SqlCommand command = new SqlCommand(sCommand, connection))
                    {
                        try
                        {
                            connection.Open();
                            SendCount = (Int32)command.ExecuteScalar();
                            connection.Close();

                            new Logger().Write("SendCount = " + SendCount.ToString());
                        }
                        catch (Exception Ex1)
                        {
                            new Logger().Write("Exception = " + Ex1.Message);
                            //throw;
                        }
                    }
                    new Logger().Write("Ending Send Check = " + DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fff"));

                    if (SendCount > 0)
                    {
                        Debug.WriteLine("Break");
                        break;
                    }
                    Debug.WriteLine("Looping");
                    if (i <= 5)
                    {
                        System.Threading.Thread.Sleep(500);
                    }
                    else
                    {
                        Int32 sleepTime = 3000 * (i - 5);
                        System.Threading.Thread.Sleep(sleepTime);
                    }
                }

            }

            Assert.IsFalse(SendCount < 1, "Message Not Sent on Port - " + portName);
        }

        [Then(@"there will be no suspended messages in BizTalk Application '(.*)'")]
        public void ThenThereWillBeNoSuspendedMessagesInBizTalkApplication(string appName)
        {
            Debug.WriteLine("ThenThereWillBeNoSuspendedMessagesInBizTalkApplication");
            if (appName.Contains(','))
            {
                List<string> appNames = appName.Split(',').ToList<string>();
                foreach (string appName1 in appNames)
                {
                    new Logger().Write("ThenThereWillBeNoSuspendedMessagesInBizTalkApplication: " + appName1);
                    Assert.IsFalse(MessageBox.AnySuspendedInstances(appName1));
                }
            }
            else
            {
                new Logger().Write("ThenThereWillBeNoSuspendedMessagesInBizTalkApplication: " + appName);
                Assert.IsFalse(MessageBox.AnySuspendedInstances(appName));
            }
        }

        [Given(@"there is a '(.*)' to process")]
        public void GivenThereIsToProcess(string fileType, Table table)
        {
            Debug.WriteLine("GivenThereIsToProcess(" + fileType + ")");

            // clean out output directory
            string outDirectory = "";
            string filePrefix = "";
            string fileSuffix = "";
            try
            {
                Debug.WriteLine("Finding SendPorts");
                var sendPort = BizTalkBill.TestingFramework.Utilities.Configuration.TestConfiguration.Current.SendPorts.Get("BizTalk", "*" + fileType + "*");
                if (sendPort != null)
                {
                    outDirectory = sendPort.URL;
                    filePrefix = sendPort.FilePrefix;
                    fileSuffix = sendPort.FileSuffix;

                    if (!string.IsNullOrWhiteSpace(outDirectory))
                    {
                        DirectoryInfo outDir = new DirectoryInfo(outDirectory);
                        var fileInfo = outDir.EnumerateFiles(filePrefix + "*" + fileSuffix);
                        ScenarioContext.Current.Add("ProcessOutputFileDirectory", outDirectory);
                        ScenarioContext.Current.Add("ProcessOutputFileMask", filePrefix + "*" + fileSuffix);
                        foreach (var file in fileInfo)
                        {
                            Debug.WriteLine("File to Delete = " + file.FullName);

                            File.Delete(file.FullName);
                        }
                    }
                }
            }
            catch (Exception Ex1)
            {
                Debug.WriteLine(Ex1.ToString());
            }

            // get input file
            var filePath = table.Rows[0]["Message Path"];
            ScenarioContext.Current.Add("TestDataFile", filePath);

            Debug.WriteLine(filePath);

            var asm = ScenarioContext.Current["AssemblyContext"] as Assembly;
            string moduleName = asm.ManifestModule.Name.Replace(".dll", "");

            Debug.WriteLine(moduleName);

            ManifestResourceInfo riTestData = asm.GetManifestResourceInfo(moduleName + ".TestData." + filePath);
            Assert.IsNotNull(riTestData, "Test File not found as a Resource");
        }

        [When(@"the '(.*)' is sent to BizTalk")]
        public void WhenTheIsSentToBizTalk(string fileType, Table table)
        {
            Debug.WriteLine("GivenThereIsToProcess(" + fileType + ")");

            var asm = ScenarioContext.Current["AssemblyContext"] as Assembly;
            string moduleName = asm.ManifestModule.Name.Replace(".dll", "");

            var portName = table.Rows[0]["Port Name"];
            var applicationName = table.Rows[0]["Application Name"];

            var receivePort = BizTalkBill.TestingFramework.Utilities.Configuration.TestConfiguration.Current.ReceivePorts.Get(applicationName, portName + "." + fileType);

            Debug.WriteLine(receivePort.URL);
            //ScenarioContext.Current.Add(SharedSteps.InPortURIKey, receivePort.URL);

            new Logger().Write("Copying File: " + ScenarioContext.Current["TestDataFile"] as string + " To: " + receivePort.URL + "\\" + ScenarioContext.Current["TestDataFile"] as string);

            ResourceHelper.CopyResource(asm, moduleName + ".TestData." + ScenarioContext.Current["TestDataFile"] as string, receivePort.URL + "\\" + ScenarioContext.Current["TestDataFile"] as string);
        }

        [When(@"the '(.*)' is sent to BizTalk with WCF BasicHTTP")]
        public void WhenTheIsSentToBizTalkwithWCFBasicHTTP(string fileType, Table table)
        {
            Debug.WriteLine("WhenTheIsSentToBizTalkwithWCFBasicHTTP(" + fileType + ")");

            var asm = ScenarioContext.Current["AssemblyContext"] as Assembly;
            string moduleName = asm.ManifestModule.Name.Replace(".dll", "");

            var portName = table.Rows[0]["Port Name"];
            var applicationName = table.Rows[0]["Application Name"];

            var receivePort = BizTalkBill.TestingFramework.Utilities.Configuration.TestConfiguration.Current.ReceivePorts.Get(applicationName, portName + "." + fileType);

            Debug.WriteLine(receivePort.URL);
            //ScenarioContext.Current.Add(SharedSteps.InPortURIKey, receivePort.URL);

            new Logger().Write("Copying File: " + ScenarioContext.Current["TestDataFile"] as string + " To: " + receivePort.URL);

            string response = WCFHelper.BasicHTTPTwoWay(asm, moduleName + ".TestData." + ScenarioContext.Current["TestDataFile"] as string, receivePort.URL);
            ScenarioContext.Current.Add("WCFBasicHTTPResponse", response);
            new Logger().Write("Response: " + response);
            Debug.WriteLine("Response: " + response);
        }

        [Then(@"The WCF BasicHTTP Response will Contain '(.*)'")]
        public void ThenTheWCFBasicHTTPResponseWillContain(string containString)
        {
            Debug.WriteLine("ThenTheWCFBasicHTTPResponseWillContain(" + containString + ")");

            string response = ScenarioContext.Current["WCFBasicHTTPResponse"] as string;
            new Logger().Write("Response: '" + response + "'");
            Debug.WriteLine("Response: '" + response + "'");
            Assert.IsFalse(string.IsNullOrEmpty(response), "The response is null or empty");

            Assert.IsTrue(response.ToLower().Contains(containString.ToLower()), string.Format("The response does not match looking for '{0}' in Response '{1}'", containString, response));
        }

        [Then(@"BizTalk will send the message to unwanted")]
        public void ThenBizTalkWillSendTheMessageToUnwanted()
        {
            // wait for the file, may take a little while
            for (int i = 1; i <= 5; i++)
            {
                if (FileHelper.FileExists(@"C:\Demos\BizTalkHL7\Unwanted\unwanted.hl7"))
                {
                    break;
                }
                System.Threading.Thread.Sleep(500);
            }

            Assert.IsTrue(FileHelper.FileExists(@"C:\Demos\BizTalkHL7\Unwanted\unwanted.hl7"), "The Expected unwanted.hl7 File not found");
        }

        [Given(@"there is a input file to test map with")]
        public void GivenThereIsAInputFileToTestMapWith(Table table)
        {
            Debug.WriteLine("ThereIsAInputFileToTestMapWith");

            var filePath = table.Rows[0]["Message Path"];
            ScenarioContext.Current.Add("MapInputFile", filePath);

            Debug.WriteLine(filePath);

            var asm = ScenarioContext.Current["AssemblyContext"] as Assembly;
            string moduleName = asm.ManifestModule.Name.Replace(".dll", "");

            Debug.WriteLine(moduleName);

            ManifestResourceInfo riTestData = asm.GetManifestResourceInfo(moduleName + ".TestData." + filePath);
            Assert.IsNotNull(riTestData, "Map Input File not found as a Resource");
        }

        [When(@"the map is executed")]
        public void WhenTheMapIsExecuted(Table table)
        {
            var mapDLL = table.Rows[0]["Map DLL"];
            var mapName = table.Rows[0]["Map Name"];
            var inputType = table.Rows[0]["Input Type"];
            var outputType = table.Rows[0]["Output Type"];
            var valdiateInput = Convert.ToBoolean(table.Rows[0]["Validate Input"]);
            var validateOutput = Convert.ToBoolean(table.Rows[0]["Validate Output"]);

            string runGUID = Guid.NewGuid().ToString();
            string sourceFileName = "";
            string destFileName = "";

            AssemblyName[] refList = ScenarioContext.Current["MapRefList"] as AssemblyName[];
            // create temp source file 
            sourceFileName = runGUID + "_" + ScenarioContext.Current["MapInputFile"] as string;

            var asm = ScenarioContext.Current["AssemblyContext"] as Assembly;
            string moduleName = asm.ManifestModule.Name.Replace(".dll", "");
            ResourceHelper.CopyResource(asm, moduleName + ".TestData." + ScenarioContext.Current["MapInputFile"] as string, sourceFileName);

            // create dest file
            string outExtension = "txt";
            if (outputType.ToUpper() == "XML")
                outExtension = "xml";
            destFileName = runGUID + "_" + mapName + "_output." + outExtension;
            ScenarioContext.Current.Add("MapOutputFile", destFileName);

            UnitTesting.ExecuteMapTest(refList, mapDLL, mapName, sourceFileName, inputType, destFileName, outputType, valdiateInput, validateOutput);
        }

        [Then(@"the Map will produce output")]
        public void ThenTheMapWillProduceOutput()
        {
            Assert.IsTrue(File.Exists(ScenarioContext.Current["MapOutputFile"] as string), "Map Output file not found");
            ScenarioContext.Current.Add("OutputFile", ScenarioContext.Current["MapOutputFile"] as string);
        }

        [Then(@"XPath Tests")]
        public void ThenXPathTests(Table table)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(ScenarioContext.Current["OutputFile"] as string);
            var nav = xDoc.CreateNavigator();

            foreach (TableRow test in table.Rows)
            {
                Debug.WriteLine(test["Name"]);
                Debug.WriteLine(test["Test"]);
                switch (test["Test"])
                {
                    case "AreEqual":
                        Assert.AreEqual(test["Value"], nav.Evaluate(test["Expression"]).ToString(), test["Name"]);
                        break;
                    default:
                        throw new ApplicationException("unsuport test type");
                }
            }
        }

        [Then(@"The '(.*)' Output will match")]
        public void ThenTheOutputWillMatch(string fileType, Table table)
        {
            var compareFile = table.Rows[0]["Compare File"];

            string runGUID = Guid.NewGuid().ToString();
            string compareFileName = runGUID + "_" + compareFile;

            var asm = ScenarioContext.Current["AssemblyContext"] as Assembly;
            string moduleName = asm.ManifestModule.Name.Replace(".dll", "");
            ResourceHelper.CopyResource(asm, moduleName + ".TestData." + compareFile, compareFileName);

            // find output
            string outFileName = "";
            string outDirectory = "";
            string filePrefix = "";
            string fileSuffix = "";
            try
            {
                Debug.WriteLine("Finding SendPorts");
                var sendPort = BizTalkBill.TestingFramework.Utilities.Configuration.TestConfiguration.Current.SendPorts.Get("BizTalk", "*" + fileType);
                if (sendPort != null)
                {
                    outDirectory = sendPort.URL;
                    filePrefix = sendPort.FilePrefix;
                    fileSuffix = sendPort.FileSuffix;

                    if (!string.IsNullOrWhiteSpace(outDirectory))
                    {
                        DirectoryInfo outDir = new DirectoryInfo(outDirectory);
                        var fileInfo = outDir.EnumerateFiles(filePrefix + "*" + fileSuffix);
                        Int32 foundCount = 0;
                        foreach (var file in fileInfo)
                        {
                            Debug.WriteLine("Output File Found = " + file.FullName);
                            outFileName = file.FullName;
                            foundCount++;
                        }
                        if (foundCount != 1)
                        {
                            Assert.Fail(string.Format("{0} Output Files Found", foundCount));
                        }
                    }
                }
            }
            catch (Exception Ex1)
            {
                Debug.WriteLine(Ex1.ToString());
            }

            if (compareFileName.ToUpper().EndsWith("XML"))
            {
                Assert.IsTrue(BizTalkBill.TestingFramework.Helpers.XML.FileCompare.CompareXMLFiles(outFileName, compareFileName), "The File Compare Failed");
            }
            else
            {
                Assert.IsTrue(BizTalkBill.TestingFramework.Helpers.IO.FileHelper.FileCompare(outFileName, compareFileName), "The File Compare Failed");
            }
        }

        [Then(@"there will be output")]
        public void ThenThereWillBeOutput()
        {
            DirectoryInfo outDir = new DirectoryInfo(ScenarioContext.Current["ProcessOutputFileDirectory"] as string);
            var fileInfo = outDir.EnumerateFiles(ScenarioContext.Current["ProcessOutputFileMask"] as string);
            Int16 nFound = 0;
            string foundFile = "";
            foreach (var file in fileInfo)
            {
                Debug.WriteLine("File Found = " + file.FullName);
                foundFile = file.FullName;
                nFound++;
            }

            Debug.WriteLine("Found Count = " + nFound.ToString());

            Assert.IsFalse((nFound < 1), "Process Output file not found");
            Assert.IsFalse((nFound > 1), "To many Process Output files found");
            ScenarioContext.Current.Add("OutputFile", foundFile);
        }

    }
}
