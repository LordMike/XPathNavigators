using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.XPath;
using OffregLib;
using XPathNavigatorsBase;

namespace OffregXPathNavigator
{
    public class OffregKeyNode : ParentNodeContainer<string>
    {
        private static readonly char[] DirectorySeperators = new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };

        private OffregKey _root;
        public string FullPath { get; private set; }

        private Dictionary<string, ValueContainer> _values;

        public OffregKeyNode(OffregKey root, string fullPath, XPathNodeBase parent)
            : base(parent)
        {
            _root = root;
            FullPath = fullPath;

            string name = string.Empty;

            if (!string.IsNullOrEmpty(fullPath))
            {
                name = fullPath;
                int lastIndex = fullPath.LastIndexOfAny(DirectorySeperators);

                if (lastIndex > -1)
                    name = fullPath.Substring(lastIndex + 1);
            }

            OffregKey tmpKey = root;
            if (!string.IsNullOrEmpty(fullPath) && !root.TryOpenSubKey(fullPath, out tmpKey))
                throw new ArgumentException("Invalid key");

            try
            {
                string[] keys = tmpKey.GetSubKeyNames();
                ValueContainer[] values = tmpKey.EnumerateValues();

                KeyValuePair<ChildType, string>[] children = new KeyValuePair<ChildType, string>[keys.Length + values.Length];

                for (int i = 0; i < keys.Length; i++)
                    children[i] = new KeyValuePair<ChildType, string>(ChildType.Key, keys[i]);

                _values = new Dictionary<string, ValueContainer>(StringComparer.InvariantCultureIgnoreCase);
                for (int i = 0; i < values.Length; i++)
                {
                    children[keys.Length + i] = new KeyValuePair<ChildType, string>(ChildType.Value, values[i].Name);
                    _values[values[i].Name] = values[i];
                }

                List<KeyValuePair<string, string>> attributes = new List<KeyValuePair<string, string>>();
                attributes.Add(new KeyValuePair<string, string>("Name", name));

                if (tmpKey.SubkeyCount > 0)
                    attributes.Add(new KeyValuePair<string, string>("SubKeys", tmpKey.SubkeyCount.ToString()));
                if (tmpKey.ValueCount > 0)
                    attributes.Add(new KeyValuePair<string, string>("Values", tmpKey.ValueCount.ToString()));

                Init(children, 0, attributes);
            }
            finally
            {
                if (!string.IsNullOrEmpty(fullPath))
                    // Not root
                    tmpKey.Close();
            }
        }

        public override string Name
        {
            get { return "Key"; }
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
                    string newPath = child.Value;
                    if (!string.IsNullOrEmpty(FullPath))
                        newPath = FullPath + "\\" + newPath;

                    return new OffregKeyNode(_root, newPath, this);
                case ChildType.Value:
                    ValueContainer value = _values[child.Value];
                    return new OffregValueNode(this, Children[ChildIndex].Value, value.Type, value.Data);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override XPathNodeType NodeType
        {
            get { return XPathNodeType.Element; }
        }

        public override bool IsSamePosition(XPathNodeBase item)
        {
            OffregKeyNode other = item as OffregKeyNode;
            return other != null && other.FullPath == FullPath;
        }

        public object GetValue(string name, object fallBack = null)
        {
            ValueContainer tmpVal;
            if (!_values.TryGetValue(name, out tmpVal))
                return fallBack;

            return tmpVal.Data;
        }

        public string GetValue(string name, string fallBack = null)
        {
            return (GetValue(name,(object)null) as string) ?? fallBack;
        }

        public string[] GetValue(string name, string[] fallBack = null)
        {
            return (GetValue(name, (object)null) as string[]) ?? fallBack;
        }
    }
}