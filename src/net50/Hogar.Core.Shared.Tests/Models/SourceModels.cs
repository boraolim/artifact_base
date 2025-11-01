using System.Collections.Generic;

using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.WebUtilities;

namespace Utilities.Core.Shared.Tests.Models
{
    public class SourceModel
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public List<string> Tags { get; set; }
    }

    public class TargetModel
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public List<string> Tags { get; set; }
    }

    public class ComplexTargetModel
    {
        public string Name { get; set; }
        public long Age { get; set; }  // Diferente tipo
        public List<string> Tags { get; set; }
    }

    public class SourceWithDict
    {
        public Dictionary<string, int> Data { get; set; }
    }
    public class TargetWithDict
    {
        public Dictionary<string, long> Data { get; set; }
    }

    public class TargetWithCtor
    {
        public string FullName { get; }
        public int Years { get; }

        public TargetWithCtor(string fullName, int years)
        {
            FullName = fullName;
            Years = years;
        }
    }

    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
    }

    public class SourceNested
    {
        public string Name { get; set; }
        public Address Address { get; set; }
    }

    public class TargetNested
    {
        public string Name { get; set; }
        public Address Address { get; set; } // Mismo tipo, se puede copiar directamente
    }

    public class SourceCtor
    {
        public string FullName { get; set; }
        public int Years { get; set; }
    }

    public class TargetWithConstructor
    {
        public string FullName { get; }
        public int Years { get; }

        public TargetWithConstructor(string fullName, int years)
        {
            FullName = fullName;
            Years = years;
        }
    }

    public sealed class FakeResult<T>
    {
        public bool Succeded { get; set; }
        public T Data { get; set; }
    }

    public class FakeMultipartSection
    {
        public string ContentDisposition { get; set; }
    }

    public class Source
    {
        public List<int> Numbers { get; set; }
    }
    public class Target
    {
        public List<long> Numbers { get; set; }
    }

    public class SourceWithProperty
    {
        public string Name { get; set; }
    }

    public class SourceWithField
    {
        public int Age;
    }

    public class SourceWithMethod
    {
        public static void DoSomething() { }
    }

    public class TestClassForProp
    {
        public string SomeProperty { get; set; }
        public int SomeField;
        public static void SomeMethod() { }
    }

    public class Dummy
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
