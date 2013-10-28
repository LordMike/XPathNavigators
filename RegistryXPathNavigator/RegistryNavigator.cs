using System;
using System.Xml;
using System.Xml.XPath;
using Microsoft.Win32;
using XPathNavigatorsBase;

namespace RegistryXPathNavigator
{
    public class RegistryNavigator : XPathNavigator
    {
        public override string NamespaceURI { get { return String.Empty; } }
        public override string Prefix { get { return String.Empty; } }
        public override string BaseURI { get { return String.Empty; } }
        public override string Name { get { return LocalName; } }
        public override XmlNameTable NameTable { get { return null; } }

        public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope) { return false; }
        public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope) { return false; }

        public XPathNodeBase Item { get; private set; }

        public RegistryNavigator(RegistryKey rootKey)
        {
            Item = new RegistryKeyNode(rootKey, null, null);
        }

        public RegistryNavigator(XPathNodeBase other)
        {
            Item = other;
        }

        public override bool MoveToId(string id)
        {
            return false;
        }

        public override XPathNavigator Clone()
        {
            return new RegistryNavigator(Item);
        }

        public override bool IsEmptyElement
        {
            get { return Item.IsEmptyElement; }
        }

        public override bool IsSamePosition(XPathNavigator other)
        {
            var o = other as RegistryNavigator;
            return o != null && o.Item.IsSamePosition(Item);
        }

        public override string LocalName
        {
            get { return Item.Name; }
        }

        public override bool MoveTo(XPathNavigator other)
        {
            var o = other as RegistryNavigator;
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