using System;

namespace Fissoft.Core.Tests
{
    internal class MyClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? Age { get; set; }
        public int AddTime { get; set; }
        public int? CanNull { get; set; }
        public DateTime? Time1 { get; set; }
        public DateTime Time { get; set; }
        public MyClass Class { get; set; }
        public string NullString { get; set; }
        public MyEnum MyEnum { get; set; }
        public MyEnum? NMyEnum { get; set; }
        public string Name1 { get; set; }
    }
}