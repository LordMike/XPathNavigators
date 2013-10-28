using System.Collections.Generic;
using System.Xml.XPath;
using Alphaleonis.Win32.Filesystem;
using XPathNavigatorsBase;

namespace FilesystemXPathNavigator
{
    public class FileNode : ParentNodeContainer<string>
    {
        public FileInfo File { get; private set; }

        public FileNode(FileInfo file)
            : this(null, file)
        {
        }

        public FileNode(XPathNodeBase parent, FileInfo file)
            : base(parent)
        {
            File = file;

            List<KeyValuePair<string, string>> attributes = new List<KeyValuePair<string, string>>();
            attributes.Add(new KeyValuePair<string, string>("Name", file.Name));
            attributes.Add(new KeyValuePair<string, string>("CreatedUtc", file.CreationTimeUtc.ToString("yyyy-MM-dd HH:mm:ss")));
            attributes.Add(new KeyValuePair<string, string>("LastAccessTimeUtc", file.LastAccessTimeUtc.ToString("yyyy-MM-dd HH:mm:ss")));
            attributes.Add(new KeyValuePair<string, string>("LastWriteTimeUtc", file.LastWriteTimeUtc.ToString("yyyy-MM-dd HH:mm:ss")));

            attributes.Add(new KeyValuePair<string, string>("Size", file.Length.ToString()));
            attributes.Add(new KeyValuePair<string, string>("Attribs", file.Attributes.ToString()));

            Init(new KeyValuePair<ChildType, string>[0], 0, attributes);
        }

        public override string Name
        {
            get { return "File"; }
        }

        public override XPathNodeType NodeType
        {
            get { return XPathNodeType.Element;}
        }

        public override bool IsSamePosition(XPathNodeBase item)
        {
            FileNode file = item as FileNode;
            return file != null && file.File.FullName == File.FullName;
        }

        protected override XPathNodeBase Get(int index)
        {
            return null;
        }
    }
}