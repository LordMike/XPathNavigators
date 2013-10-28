using System.Xml.XPath;

namespace XPathNavigatorsBase
{
    public class TextNode : XPathNodeBase
    {
        private XPathNodeBase _parent;
        private string _text;

        public TextNode(XPathNodeBase parent, string text)
        {
            _parent = parent;
            _text = text;
        }

        public override string Name
        {
            get { return null; }
        }

        public override string Value
        {
            get { return _text; }
        }

        public override XPathNodeBase MoveToFirstAttribute()
        {
            return null;
        }

        public override XPathNodeBase MoveToNextAttribute()
        {
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
            get { return XPathNodeType.Text;}
        }

        public override bool IsEmptyElement
        {
            get { return true; }
        }

        public override XPathNodeBase MoveToParent()
        {
            return _parent;
        }

        public override XPathNodeBase MoveToAttribute(string name)
        {
            return null;
        }

        public override bool IsSamePosition(XPathNodeBase item)
        {
            return false;
        }
    }
}