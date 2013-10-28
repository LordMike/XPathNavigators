using System;
using System.Xml;
using System.Xml.XPath;
using Alphaleonis.Win32.Filesystem;
using XPathNavigatorsBase;

namespace FilesystemXPathNavigator
{
    public class FilesystemNavigator : XPathNavigator
    {
        public override string NamespaceURI { get { return String.Empty; } }
        public override string Prefix { get { return String.Empty; } }
        public override string BaseURI { get { return String.Empty; } }
        public override string Name { get { return LocalName; } }
        public override XmlNameTable NameTable { get { return null; } }

        public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope) { return false; }
        public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope) { return false; }

        public XPathNodeBase Item { get; private set; }

        public FilesystemNavigator(System.IO.FileSystemInfo root, bool ignoreAccessDenied = false)
        {
            System.IO.DirectoryInfo dir = root as System.IO.DirectoryInfo;

            if (dir != null)
            {
                Init(new DirectoryInfo(dir.FullName), ignoreAccessDenied);
                return;
            }

            System.IO.FileInfo file = root as System.IO.FileInfo;
            if (file != null)
            {
                Item = new FileNode(new FileInfo(file.FullName));
                return;
            }

            throw new ArgumentException();
        }

        public FilesystemNavigator(FileSystemInfo root, bool ignoreAccessDenied = false)
        {
            Init(root, ignoreAccessDenied);
        }

        private void Init(FileSystemInfo root, bool ignoreAccessDenied = false)
        {
            DirectoryInfo dir = root as DirectoryInfo;
            FileInfo file = root as FileInfo;

            if (dir != null)
                Item = new DirectoryNode(dir, ignoreAccessDenied);
            else if (file != null)
                Item = new FileNode(file);
            else
                throw new ArgumentException();
        }

        public FilesystemNavigator(XPathNodeBase other)
        {
            Item = other;
        }

        public override bool MoveToId(string id)
        {
            return false;
        }

        public override XPathNavigator Clone()
        {
            return new FilesystemNavigator(Item);
        }

        public override bool IsEmptyElement
        {
            get { return Item.IsEmptyElement; }
        }

        public override bool IsSamePosition(XPathNavigator other)
        {
            var o = other as FilesystemNavigator;
            return o != null && o.Item.IsSamePosition(Item);
        }

        public override string LocalName
        {
            get { return Item.Name; }
        }

        public override bool MoveTo(XPathNavigator other)
        {
            var o = other as FilesystemNavigator;
            if (o != null)
            {
                Item = o.Item;
                return true;
            }
            return false;
        }

        public override bool MoveToFirstAttribute()
        {
            return MoveToItem(Item.MoveToFirstAttribute());
        }

        private bool MoveToItem(XPathNodeBase newItem)
        {
            if (newItem == null)
                return false;

            Item = newItem;
            return true;
        }

        public override bool MoveToFirstChild()
        {
            return MoveToItem(Item.MoveToFirstChild());
        }

        public override bool MoveToNext()
        {
            return MoveToItem(Item.MoveToNext());
        }

        public override bool MoveToNextAttribute()
        {
            return MoveToItem(Item.MoveToNextAttribute());
        }

        public override bool MoveToParent()
        {
            return MoveToItem(Item.MoveToParent());
        }

        public override bool MoveToPrevious()
        {
            return MoveToItem(Item.MoveToPrevious());
        }

        public override XPathNodeType NodeType
        {
            get { return Item.NodeType; }
        }

        public override string Value
        {
            get { return Item.Value; }
        }

        public override bool MoveToAttribute(string localName, string namespaceUri)
        {
            if (namespaceUri != String.Empty)
                return false;

            return MoveToItem(Item.MoveToAttribute(localName));
        }
    }
}