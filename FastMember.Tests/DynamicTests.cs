using System.Dynamic;
using Microsoft.CSharp.RuntimeBinder;
using NUnit.Framework;

namespace FastMember.Tests
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
            var wrap = MemberAccess.Wrap((object)expando);

            Assert.AreEqual(123, wrap["A"]);
            Assert.AreEqual("def", wrap["B"]);
        }
        [Test, ExpectedException(typeof(RuntimeBinderException))]
        public void TestReadInvalid()
        {
            dynamic expando = new ExpandoObject();
            var wrap = MemberAccess.Wrap((object)expando);
            Assert.AreEqual(123, wrap["C"]);
        }
        [Test]
        public void TestWrite()
        {
            dynamic expando = new ExpandoObject();
            var wrap = MemberAccess.Wrap((object)expando);
            wrap["A"] = 123;
            wrap["B"] = "def";
            
            Assert.AreEqual(123, expando.A);
            Assert.AreEqual("def", expando.B);
        }
    }
}
