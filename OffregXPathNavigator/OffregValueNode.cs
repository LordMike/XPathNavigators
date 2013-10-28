using System;
using System.Collections.Generic;
using System.Xml.XPath;
using OffregLib;
using XPathNavigatorsBase;

namespace OffregXPathNavigator
{
    public class OffregValueNode : ParentNodeContainer<string>
    {
       public string ValueName { get;private set;}
       public object ValueObject { get; private set; }
       public RegValueType ValueType { get; private set; }

       public OffregValueNode(XPathNodeBase parent, string name, RegValueType type, object value)
            : base(parent)
        {
            ValueName = name;
            ValueType = type;
            ValueObject = value;

            KeyValuePair<ChildType, string>[] children = new KeyValuePair<ChildType, string>[1];
            children[0] = new KeyValuePair<ChildType, string>(ChildType.Value, string.Empty);

            List<KeyValuePair<string, string>> attributes = new List<KeyValuePair<string, string>>();
            attributes.Add(new KeyValuePair<string, string>("Name", ValueName));
            attributes.Add(new KeyValuePair<string, string>("Type", ValueType.ToString()));

            Init(children, 0, attributes);
        }

        public override string Name
        {
            get { return "Value"; }
        }

        protected override XPathNodeBase Get(int index)
        {
            if (ValueObject is string)
                return new TextNode(this, (string)ValueObject);

            if (ValueObject is string[])
                return new TextNode(this, string.Join(Environment.NewLine, (string[])ValueObject));

            if (ValueObject is byte[])
                return new TextNode(this, Convert.ToBase64String((byte[])ValueObject));

            if (ValueObject is int)
                return new TextNode(this, ((int)ValueObject).ToString());

            if (ValueObject is long)
                return new TextNode(this, ((long)ValueObject).ToString());

            if (ValueObject == null)
                return new TextNode(this, string.Empty);

            throw new ArgumentOutOfRangeException();
        }

        public override XPathNodeType NodeType
        {
            get { return XPathNodeType.Element; }
        }

        public override bool IsSamePosition(XPathNodeBase item)
        {
            return false;
        }
    }
}