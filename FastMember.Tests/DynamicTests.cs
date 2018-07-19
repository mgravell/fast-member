using FastMember;
using Microsoft.CSharp.RuntimeBinder;
using System.Dynamic;
using Xunit;

namespace FastMemberTests
{
    public class DynamicTests
    {
        [Fact]
        public void TestReadValid()
        {
            dynamic expando = new ExpandoObject();
            expando.A = 123;
            expando.B = "def";
            var wrap = ObjectAccessor.Create((object)expando);

            Assert.Equal(123, wrap["A"]);
            Assert.Equal("def", wrap["B"]);
        }
        [Fact]
        public void TestReadInvalid()
        {
            Assert.Throws<RuntimeBinderException>(() =>
            {
                dynamic expando = new ExpandoObject();
                var wrap = ObjectAccessor.Create((object)expando);
                Assert.Equal(123, wrap["C"]);
            });
        }
        [Fact]
        public void TestWrite()
        {
            dynamic expando = new ExpandoObject();
            var wrap = ObjectAccessor.Create((object)expando);
            wrap["A"] = 123;
            wrap["B"] = "def";
            
            Assert.Equal(123, expando.A);
            Assert.Equal("def", expando.B);
        }

        [Fact]
        public void DynamicByTypeWrapper()
        {
            var obj = new ExpandoObject();
            ((dynamic)obj).Foo = "bar";
            var accessor = TypeAccessor.Create(obj.GetType());

            Assert.Equal("bar", accessor[obj, "Foo"]);
            accessor[obj, "Foo"] = "BAR";
            string result = ((dynamic) obj).Foo;
            Assert.Equal("BAR", result);
        }
    }
}