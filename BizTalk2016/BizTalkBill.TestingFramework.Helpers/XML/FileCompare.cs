using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.XmlDiffPatch;
using System.Diagnostics;

namespace BizTalkBill.TestingFramework.Helpers.XML
{
    public class FileCompare
    {
        public static Boolean CompareXMLFiles(string file1, string file2)
        {
            Debug.WriteLine("File1 = " + file1);
            Debug.WriteLine("File2 = " + file2);
            XmlDiff xmldiff = new XmlDiff();
            return xmldiff.Compare(file1, file2, false);
        }
    }
}
