using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.XPath;
using Microsoft.Win32;
using XPathNavigatorsBase;

namespace RegistryXPathNavigator
{
    public class RegistryKeyNode : ParentNodeContainer<string>
    {
        private static readonly char[] DirectorySeperators = new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };

        private RegistryKey _root;
        public string FullPath { get; private set; }

        private Dictionary<string, RegistryValueContainer> _values;

        public RegistryKeyNode(RegistryKey root, string fullPath, XPathNodeBase parent)
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

            RegistryKey tmpKey = root;
            if (!string.IsNullOrEmpty(fullPath))
            {
                try
                {
                    tmpKey = root.OpenSubKey(fullPath);
                }
                catch (Exception)
                {
                    throw new ArgumentException("Invalid key");
                }
            }

            try
            {
                List<KeyValuePair<string, string>> attributes = new List<KeyValuePair<string, string>>();
                KeyValuePair<ChildType, string>[] children = new KeyValuePair<ChildType, string>[0];

                attributes.Add(new KeyValuePair<string, string>("Name", name));

                if (tmpKey == null)
                {
                    attributes.Add(new KeyValuePair<string, string>("Missing", "true"));
                }
                else
                {
                    string[] keys = tmpKey.GetSubKeyNames();
                    string[] valueNames = tmpKey.GetValueNames();

                    children = new KeyValuePair<ChildType, string>[keys.Length + valueNames.Length];

                    for (int i = 0; i < keys.Length; i++)
                        children[i] = new KeyValuePair<ChildType, string>(ChildType.Key, keys[i]);

                    _values = new Dictionary<string, RegistryValueContainer>(StringComparer.InvariantCultureIgnoreCase);
                    for (int i = 0; i < valueNames.Length; i++)
                    {
                        string valueName = valueNames[i];
                        children[keys.Length + i] = new KeyValuePair<ChildType, string>(ChildType.Value, valueName);

                        object value = tmpKey.GetValue(valueName);
                        RegistryValueKind type = tmpKey.GetValueKind(valueName);

                        RegistryValueContainer container = new RegistryValueContainer(valueName, type, value);

                        _values[valueName] = container;
                    }

                    if (tmpKey.SubKeyCount > 0)
                        attributes.Add(new KeyValuePair<string, string>("SubKeys", tmpKey.SubKeyCount.ToString()));
                    if (tmpKey.ValueCount > 0)
                        attributes.Add(new KeyValuePair<string, string>("Values", tmpKey.ValueCount.ToString()));

                }

                Init(children, 0, attributes);
            }
            finally
            {
                if (!string.IsNullOrEmpty(fullPath) && tmpKey != null)
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

                    return new RegistryKeyNode(_root, newPath, this);
                case ChildType.Value:
                    RegistryValueContainer value = _values[child.Value];
                    return new RegistryValueNode(this, Children[ChildIndex].Value, value.Type, value.Data);
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
            RegistryKeyNode other = item as RegistryKeyNode;
            return other != null && other.FullPath == FullPath;
        }

        public object GetValue(string name, object fallBack = null)
        {
            RegistryValueContainer tmpVal;
            if (!_values.TryGetValue(name, out tmpVal))
                return fallBack;

            return tmpVal.Data;
        }

        public string GetValue(string name, string fallBack = null)
        {
            return (GetValue(name, (object)null) as string) ?? fallBack;
        }

        public string[] GetValue(string name, string[] fallBack = null)
        {
            return (GetValue(name, (object)null) as string[]) ?? fallBack;
        }
    }
}