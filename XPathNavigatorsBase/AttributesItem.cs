using System.Collections.Generic;
using System.Xml.XPath;

namespace XPathNavigatorsBase
{
    public class AttributesItem : XPathNodeBase
    {
        private int _valueIndex;
        private List<KeyValuePair<string, string>> _attributes;
        private XPathNodeBase _item;

        public AttributesItem(XPathNodeBase item, List<KeyValuePair<string, string>> attributes, string name = null)
        {
            _item = item;
            _valueIndex = 0;
            _attributes = attributes;

            if (name != null)
                MoveToAttribute(name);
        }

        public override string Name
        {
            get { return _attributes[_valueIndex].Key; }
        }

        public override string Value
        {
            get { return _attributes[_valueIndex].Value; }
        }

        public override XPathNodeBase MoveToFirstAttribute()
        {
            _valueIndex = 0;
            return this;
        }

        public override XPathNodeBase MoveToNextAttribute()
        {
            if (_valueIndex >= _attributes.Count - 1)
                return null;

            _valueIndex++;
            return this;
        }

        public override XPathNodeBase MoveToAttribute(string name)
        {
            for (int i = 0; i < _attributes.Count; i++)
            {
                if (!name.Equals(_attributes[i].Key))
                    continue;

                _valueIndex = i;
                return this;
            }

            return null;
        }

        public override XPathNodeBase MoveToFirstChild()
        {
            return null;
        }

        public override XPathNodeBase MoveToNext()
        {
            return null;
        }

        public override XPathNodeBase MoveToPrevious()
        {
            return null;
        }

        public override XPathNodeType NodeType
        {
            get { return XPathNodeType.Attribute; }
        }

        public override bool IsEmptyElement
        {
            get { return true; }
        }

        public override XPathNodeBase MoveToParent()
        {
            return _item;
        }

        public override bool IsSamePosition(XPathNodeBase item)
        {
            return false;
        }
    }
}