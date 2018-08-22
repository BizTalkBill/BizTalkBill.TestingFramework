using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BizTalkBill.TestingFramework.Helpers.IO
{
    public class ResourceHelper
    {
        public static void CopyResource(Assembly asm, string resourceName, string file)
        {
            using (Stream resource = asm.GetManifestResourceStream(resourceName))
            {
                if (resource == null)
                {
                    throw new ArgumentException("No such resource", "resourceName");
                }
                using (Stream output = File.OpenWrite(file))
                {
                    resource.CopyTo(output);
                }
            }
        }

    }
}
