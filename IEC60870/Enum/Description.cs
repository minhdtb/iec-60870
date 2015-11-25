using System;

namespace IEC60870.Enum
{
    public class Description : Attribute
    {
        internal Description(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public static Description GetAttr<T>(T p)
        {
            var info = typeof(T).GetField(System.Enum.GetName(typeof(T), p));
            return (Description)GetCustomAttribute(info, typeof(Description));
        }
    }
}
