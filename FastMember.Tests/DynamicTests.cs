#if !NO_DYNAMIC
using System.Dynamic;
using Microsoft.CSharp.RuntimeBinder;
using NUnit.Framework;
using FastMember;

namespace FastMemberTests
{
    [TestFixture]
    public class DynamicTests
    {
        [Test]
        public void TestReadValid()
        {
            dynamic expando = new ExpandoObject();
            expando.A = 123;
            expando.B = "def";
            var wrap = ObjectAccessor.Create((object)expando);

            Assert.AreEqual(123, wrap["A"]);
            Assert.AreEqual("def", wrap["B"]);
        }
        [Test]
        public void TestReadInvalid()
        {
            Assert.Throws<RuntimeBinderException>(() =>
            {
                dynamic expando = new ExpandoObject();
                var wrap = ObjectAccessor.Create((object)expando);
                Assert.AreEqual(123, wrap["C"]);
            });
        }
        [Test]
        public void TestWrite()
        {
            dynamic expando = new ExpandoObject();
            var wrap = ObjectAccessor.Create((object)expando);
            wrap["A"] = 123;
            wrap["B"] = "def";
            
            Assert.AreEqual(123, expando.A);
            Assert.AreEqual("def", expando.B);
        }

        [Test]
        public void DynamicByTypeWrapper()
        {
            var obj = new ExpandoObject();
            ((dynamic)obj).Foo = "bar";
            var accessor = TypeAccessor.Create(obj.GetType());

            Assert.AreEqual("bar", accessor[obj, "Foo"]);
            accessor[obj, "Foo"] = "BAR";
            string result = ((dynamic) obj).Foo;
            Assert.AreEqual("BAR", result);
        }
    }
}
#endif