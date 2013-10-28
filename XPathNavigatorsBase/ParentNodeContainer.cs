using System.Collections.Generic;

namespace XPathNavigatorsBase
{
    public abstract class ParentNodeContainer<T> : XPathNodeBase
    {
        protected XPathNodeBase Parent{ get; private set; }
        protected KeyValuePair<ChildType, T>[] Children { get; private set; }
        protected int ChildIndex{ get; set; }
        protected List<KeyValuePair<string, string>> Attributes{ get; private set; }

        public ParentNodeContainer(XPathNodeBase parent)
        {
            Parent = parent;
        }

        protected void Init(KeyValuePair<ChildType, T>[] children, int childIndex, List<KeyValuePair<string, string>> attributes)
        {
            Children = children;
            ChildIndex = childIndex;

            Attributes = attributes;
        }

        public override bool IsEmptyElement
        {
            get { return Children.Length == 0; }
        }

        public override XPathNodeBase MoveToFirstAttribute()
        {
            return Attributes.Count == 0 ? null : new AttributesItem(this, Attributes);
        }

        public override XPathNodeBase MoveToNextAttribute()
        {
            return null;
        }

        public override XPathNodeBase MoveToAttribute(string name)
        {
            foreach (KeyValuePair<string, string> attribute in Attributes)
            {
                if (attribute.Key == name)
                    return new AttributesItem(this, Attributes, name);
            }

            return null;
        }

        protected abstract XPathNodeBase Get(int index);

        public override XPathNodeBase MoveToFirstChild()
        {
            return Get(0);
        }

        public override XPathNodeBase MoveToNext()
        {
            ParentNodeContainer<T> parentKey = Parent as ParentNodeContainer<T>;
            return parentKey == null ? null : parentKey.GetNext();
        }

        public override XPathNodeBase MoveToPrevious()
        {
            ParentNodeContainer<T> parentKey = Parent as ParentNodeContainer<T>;
            return parentKey == null ? null : parentKey.GetPrevious();
        }

        public XPathNodeBase GetNext()
        {
            return Get(ChildIndex + 1);
        }

        internal XPathNodeBase GetPrevious()
        {
            return Get(ChildIndex - 1);
        }

        public override XPathNodeBase MoveToParent()
        {
            return Parent;
        }
    }
}