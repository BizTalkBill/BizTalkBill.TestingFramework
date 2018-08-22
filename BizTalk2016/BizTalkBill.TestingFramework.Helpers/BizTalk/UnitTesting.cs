using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.BizTalk.TestTools.Mapper;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Threading;


namespace BizTalkBill.TestingFramework.Helpers.BizTalk
{
    public class UnitTesting
    {

        public static void ExecuteMapTest(AssemblyName[] refList, string dllName, string mapName, string sourceFileName, string sourceFileType, string destFileName, string destFileType, bool validateInput, bool validateOutput )
        {
            //var refList = typeof(Program).Assembly.GetReferencedAssemblies();

            Debug.WriteLine("dllName = " + dllName);
            Debug.WriteLine("mapName = " + mapName);
            Debug.WriteLine("sourceFileName = " + sourceFileName);
            Debug.WriteLine("sourceFileType = " + sourceFileType);
            Debug.WriteLine("destFileName = " + destFileName);
            Debug.WriteLine("destFileType = " + destFileType);
            Debug.WriteLine("validateInput = " + validateInput);
            Debug.WriteLine("validateOutput = " + validateOutput);


            Assembly mapASM = null;

            //try loading locally (failed always loads from GAC)
            //try
            //{
            //    string asmPath = Assembly.GetExecutingAssembly().Location;
            //    Debug.WriteLine(asmPath);
            //    FileInfo asmInfo = new FileInfo(asmPath);
            //    Debug.WriteLine(asmInfo.DirectoryName);
            //    //mapASM = Assembly.LoadFile(asmInfo.DirectoryName + "\\" + dllName + ".dll");
            //    mapASM = Assembly.LoadFrom(asmInfo.DirectoryName + "\\" + dllName + ".dll");
            //    Debug.WriteLine("mapASM loaded from local file");
            //    Debug.WriteLine("From GAC = " + mapASM.GlobalAssemblyCache.ToString());
            //}
            //catch (Exception ex1)
            //{
            //    Debug.WriteLine(ex1.ToString());
            //}

            // otherwise from gac
            if (mapASM == null)
            {
                foreach (var refASM in refList)
                {
                    if (refASM.Name == dllName)
                    {
                        mapASM = Assembly.Load(refASM);
                        Debug.WriteLine("Found DLL - " + dllName);
                    }
                }
            }

            //AssemblyName myAssemblyName = AssemblyName.GetAssemblyName(dllName);
            //var mapASM = Assembly.Load(myAssemblyName);

            //FileInfo fiDLL = new FileInfo(dllName);
            //string baseDLLNamee = fiDLL.Name.Replace(fiDLL.Extension, "");

            Debug.WriteLine("Finding MAP - " + dllName + "." + mapName);

            var mapObject = mapASM.GetType(dllName + "." + mapName);

            if (mapObject != null)
            {
                Debug.WriteLine("MAP Found - " + dllName + "." + mapName);

                TestableMapBase map = (TestableMapBase)Activator.CreateInstance(mapObject);

                map.ValidateInput = validateInput;
                map.ValidateOutput = validateOutput;

                Debug.WriteLine("Validation Set");

                CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                TextInfo textInfo = cultureInfo.TextInfo;

                Microsoft.BizTalk.TestTools.Schema.InputInstanceType inType = (Microsoft.BizTalk.TestTools.Schema.InputInstanceType)Enum.Parse(typeof(Microsoft.BizTalk.TestTools.Schema.InputInstanceType), textInfo.ToTitleCase(sourceFileType));
                Microsoft.BizTalk.TestTools.Schema.OutputInstanceType outType = (Microsoft.BizTalk.TestTools.Schema.OutputInstanceType)Enum.Parse(typeof(Microsoft.BizTalk.TestTools.Schema.OutputInstanceType), (destFileType.ToUpper() == "XML") ? "XML" : destFileType);

                Debug.WriteLine("Enums Processed");

                try
                {
                    Debug.WriteLine("Testing Map - " + sourceFileName + ", " + inType.ToString() + ", " + destFileName + ", " + outType.ToString());
                    map.TestMap(sourceFileName, inType, destFileName, outType);
                }
                catch (Exception Ex1)
                {
                    throw new ApplicationException("Map Execution Failed - " + Ex1.ToString());
                }
            }

        }
    }
}
