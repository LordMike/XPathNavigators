using System.Xml.XPath;

namespace XPathNavigatorsBase
{
    /// <summary>
    /// Abstract class that defines behaviour of the XPath navigator positioned on a specific item
    /// </summary>
    public abstract class XPathNodeBase
    {
        public abstract string Name { get; }
        public abstract XPathNodeBase MoveToFirstAttribute();
        public abstract XPathNodeBase MoveToNextAttribute();
        public abstract XPathNodeBase MoveToFirstChild();
        public abstract XPathNodeBase MoveToNext();
        public abstract XPathNodeBase MoveToPrevious();
        public abstract XPathNodeType NodeType { get; }
        public virtual string Value { get { return string.Empty; } }
        public abstract bool IsEmptyElement { get; }
        public abstract XPathNodeBase MoveToParent();
        public abstract XPathNodeBase MoveToAttribute(string name);
        public abstract bool IsSamePosition(XPathNodeBase item);
    }
}