using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FastMember;

namespace FastMemberTests
{
    [TestFixture]
    public class Anons
    {
        [Test]
        public void TestAnonTypeAccess()
        {
            var obj = new {A = 123, B = "def"};

            var accessor = ObjectAccessor.Create(obj);
            Assert.AreEqual(123, accessor["A"]);
            Assert.AreEqual("def", accessor["B"]);
        }
        [Test]
        public void TestAnonCtor()
        {
            var obj = new {A = 123, B = "def"};

            var accessor = TypeAccessor.Create(obj.GetType());
            Assert.IsFalse(accessor.CreateNewSupported);
        }

        [Test]
        public void TestPrivateTypeAccess()
        {
            var obj = new Private { A = 123, B = "def" };

            var accessor = ObjectAccessor.Create(obj);
            Assert.AreEqual(123, accessor["A"]);
            Assert.AreEqual("def", accessor["B"]);
        }

        [Test]
        public void TestPrivateTypeCtor()
        {
            var accessor = TypeAccessor.Create(typeof (Private));
            Assert.IsTrue(accessor.CreateNewSupported);
            object obj = accessor.CreateNew();
            Assert.IsNotNull(obj);
            Assert.IsInstanceOf(typeof(Private), obj);
        }

        private sealed class Private
        {
            public int A { get; set; }
            public string B { get; set; }
        }
    }


}
