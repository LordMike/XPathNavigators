using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using FilesystemXPathNavigator;
using Microsoft.Win32;
using OffregLib;
using OffregXPathNavigator;
using RegistryXPathNavigator;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TestProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            FilesystemNavigator naviFs = new FilesystemNavigator(new DirectoryInfo(@"C:\Windows\System32"), true);
            Process(naviFs);

            RegistryNavigator naviRegistry = new RegistryNavigator(Registry.ClassesRoot);
            Process(naviRegistry);

            using (OffregHive hive = OffregHive.Open("ExampleHive"))
            {
                OffregNavigator naviOffreg = new OffregNavigator(hive.Root);
                Process(naviOffreg);
            }

            if (Debugger.IsAttached)
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadLine();
            }
        }

        private static void Process(XPathNavigator navigator)
        {
            string name = navigator.GetType().Name;
            Console.WriteLine("Processing " + name);

            try
            {
                Console.WriteLine("\tMaking XML document");
                XmlDocument doc = GetDocument(navigator);

                string xmlName = name + ".xml";
                doc.Save(xmlName);

                Console.WriteLine("\tSaved to {0}, size: {1:N0}", xmlName, new FileInfo(xmlName).Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\tFailed: " + ex.Message);
            }
        }

        private static XmlDocument GetDocument(XPathNavigator navigator)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(navigator.ReadSubtree());

            return doc;
        }
    }
}
