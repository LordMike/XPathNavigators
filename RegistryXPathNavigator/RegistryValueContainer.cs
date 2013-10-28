using Microsoft.Win32;

namespace RegistryXPathNavigator
{
    public class RegistryValueContainer
    {
        public string Name { get; set; }
        public RegistryValueKind Type { get; set; }
        public object Data { get; set; }

        public RegistryValueContainer(string name, RegistryValueKind type, object value)
        {
            Name = name;
            Type = type;
            Data = value;
        }
    }
}