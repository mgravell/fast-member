#if XUNIT
using System;

namespace NUnit.Framework
{
    public class TestFixtureAttribute : Attribute
    {
    }
    public class TestAttribute : Xunit.FactAttribute
    {
    }
    public static class Assert
    {
        public static void IsTrue(bool condition, string message = null)
        {
            Xunit.Assert.True(condition, message);
        }
        public static void IsFalse(bool condition, string message = null)
        {
            Xunit.Assert.False(condition, message);
        }
        public static void AreEqual<T>(T expected, T actual, string message = null)
        {
            Xunit.Assert.Equal<T>(expected, actual);
        }
        public static void AreNotEqual<T>(T expected, T actual, string message = null)
        {
            Xunit.Assert.NotEqual<T>(expected, actual);
        }
        public static void AreSame(object expected, object actual, string message = null)
        {
            Xunit.Assert.Same(expected, actual);
        }
        public static void AreNotSame(object expected, object actual, string message = null)
        {
            Xunit.Assert.NotSame(expected, actual);
        }
        public static void IsNull(object @object, string message = null)
        {
            Xunit.Assert.Null(@object);
        }
        public static void IsNotNull(object @object, string message = null)
        {
            Xunit.Assert.NotNull(@object);
        }

        public static void IsInstanceOf(Type type, object @object, string  message = null)
        {
            Xunit.Assert.IsType(type, @object);
        }

        public static void Fail(string message = null)
        {
            var pass = "pass";
            if (string.IsNullOrWhiteSpace(message)) message = "fail";
            else if (message == "pass") pass = "test pass"; // unlikely, but...
            AreEqual(pass, message);
        }
        public static void Throws<T>(Action action) where T : Exception
        {
            try
            {
                action();
                Fail($"should have thrown {typeof(T).Name}");
            }catch(T) { }
        }
    }
}
#endif