using System;

namespace RunningWater.Raspberry.Attributes
{
    public class MethodNameAttribute : Attribute
    {
        public string Name { get; private set; }

        public MethodNameAttribute(string name)
            => Name = name;
    }
}
