using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.XPath;
using Alphaleonis.Win32.Filesystem;
using XPathNavigatorsBase;
using System.Linq;

namespace FilesystemXPathNavigator
{
    public class DirectoryNode : ParentNodeContainer<string>
    {
        private bool _ignoreAccessDenied;
        public DirectoryInfo Directory { get; private set; }

        public DirectoryNode(DirectoryInfo dir, bool ignoreAccessDenied)
            : this(null, dir, ignoreAccessDenied)
        {
        }

        public DirectoryNode(XPathNodeBase parent, DirectoryInfo dir, bool ignoreAccessDenied)
            : base(parent)
        {
            _ignoreAccessDenied = ignoreAccessDenied;
            Directory = dir;

            List<KeyValuePair<string, string>> attribs = new List<KeyValuePair<string, string>>();
            KeyValuePair<ChildType, string>[] children = new KeyValuePair<ChildType, string>[0];
            int files = 0, directories = 0;

            try
            {
                attribs.Add(new KeyValuePair<string, string>("Name", dir.Name));
                attribs.Add(new KeyValuePair<string, string>("CreatedUtc", dir.CreationTimeUtc.ToString("yyyy-MM-dd HH:mm:ss")));

                FileSystemInfo[] childItems = dir.GetFileSystemInfos();
                children = new KeyValuePair<ChildType, string>[childItems.Length];

                for (int i = 0; i < childItems.Length; i++)
                {
                    ChildType type;
                    if (childItems[i] is DirectoryInfo)
                    {
                        type = ChildType.Key;
                        directories++;
                    }
                    else if (childItems[i] is FileInfo)
                    {
                        type = ChildType.Value;
                        files++;
                    }
                    else
                    {
                        throw new ArgumentException();
                    }

                    children[i] = new KeyValuePair<ChildType, string>(type, childItems[i].Name);
                }
            }
            catch (UnauthorizedAccessException)
            {
                if (!ignoreAccessDenied)
                    throw;

                attribs.Add(new KeyValuePair<string, string>("AccessDenied", "true"));
            }

            if (files > 0)
                attribs.Add(new KeyValuePair<string, string>("Files", files.ToString()));

            if (directories > 0)
                attribs.Add(new KeyValuePair<string, string>("Directories", directories.ToString()));

            Debug.Assert(children.All(s => s.Value != null));

            Init(children, 0, attribs);
        }

        public override string Name
        {
            get { return "Directory"; }
        }

        public override XPathNodeType NodeType
        {
            get { return XPathNodeType.Element; }
        }

        public override bool IsSamePosition(XPathNodeBase item)
        {
            DirectoryNode dir = item as DirectoryNode;
            return dir != null && dir.Directory.FullName == Directory.FullName;
        }

        protected override XPathNodeBase Get(int index)
        {
            if (Children.Length == 0 || index >= Children.Length)
                return null;

            ChildIndex = index;
            KeyValuePair<ChildType, string> child = Children[ChildIndex];

            switch (child.Key)
            {
                case ChildType.Key:
                    return new DirectoryNode(this, new DirectoryInfo(Path.Combine(Directory.FullName, child.Value)), _ignoreAccessDenied);
                case ChildType.Value:
                    return new FileNode(this, new FileInfo(Path.Combine(Directory.FullName, child.Value)));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    //public class DirectoryNode : ParentNodeContainer<FileSystemInfo>
    //{
    //    private bool _ignoreAccessDenied;
    //    public DirectoryInfo Directory { get; private set; }

    //    public DirectoryNode(DirectoryInfo dir, bool ignoreAccessDenied)
    //        : this(null, dir, ignoreAccessDenied)
    //    {
    //    }

    //    public DirectoryNode(XPathNodeBase parent, DirectoryInfo dir, bool ignoreAccessDenied)
    //        : base(parent)
    //    {
    //        _ignoreAccessDenied = ignoreAccessDenied;
    //        Directory = dir;

    //        List<KeyValuePair<string, string>> attribs = new List<KeyValuePair<string, string>>();
    //        KeyValuePair<ChildType, FileSystemInfo>[] children = new KeyValuePair<ChildType, FileSystemInfo>[0];
    //        int files = 0, directories = 0;

    //        try
    //        {
    //            attribs.Add(new KeyValuePair<string, string>("Name", dir.Name));
    //            attribs.Add(new KeyValuePair<string, string>("CreatedUtc", dir.CreationTimeUtc.ToString("yyyy-MM-dd HH:mm:ss")));

    //            FileSystemInfo[] childItems = dir.GetFileSystemInfos();
    //            children = new KeyValuePair<ChildType, FileSystemInfo>[childItems.Length];

    //            for (int i = 0; i < childItems.Length; i++)
    //            {
    //                ChildType type;
    //                if (childItems[i] is DirectoryInfo)
    //                {
    //                    type = ChildType.Key;
    //                    directories++;
    //                }
    //                else if (childItems[i] is FileInfo)
    //                {
    //                    type = ChildType.Value;
    //                    files++;
    //                }
    //                else
    //                {
    //                    throw new ArgumentException();
    //                }

    //                children[i] = new KeyValuePair<ChildType, FileSystemInfo>(type, childItems[i]);
    //            }
    //        }
    //        catch (UnauthorizedAccessException)
    //        {
    //            if (!ignoreAccessDenied)
    //                throw;

    //            attribs.Add(new KeyValuePair<string, string>("AccessDenied", ""));
    //        }

    //        if (files > 0)
    //            attribs.Add(new KeyValuePair<string, string>("Files", files.ToString()));

    //        if (directories > 0)
    //            attribs.Add(new KeyValuePair<string, string>("Directories", directories.ToString()));

    //        Init(children, 0, attribs);
    //    }

    //    public override string Name
    //    {
    //        get { return "Directory"; }
    //    }

    //    public override XPathNodeType NodeType
    //    {
    //        get { return XPathNodeType.Element; }
    //    }

    //    public override bool IsSamePosition(XPathNodeBase item)
    //    {
    //        DirectoryNode dir = item as DirectoryNode;
    //        return dir != null && dir.Directory.FullName == Directory.FullName;
    //    }

    //    protected override XPathNodeBase Get(int index)
    //    {
    //        if (Children.Length == 0 || index >= Children.Length)
    //            return null;

    //        ChildIndex = index;
    //        KeyValuePair<ChildType, FileSystemInfo> child = Children[ChildIndex];

    //        switch (child.Key)
    //        {
    //            case ChildType.Key:
    //                return new DirectoryNode(this, (DirectoryInfo)child.Value, _ignoreAccessDenied);
    //            case ChildType.Value:
    //                return new FileNode(this, (FileInfo)child.Value);
    //            default:
    //                throw new ArgumentOutOfRangeException();
    //        }
    //    }
    //}
}