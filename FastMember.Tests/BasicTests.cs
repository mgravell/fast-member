using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace FastMember.Tests
{
    [TestFixture]
    public class BasicTests
    {
        [Test]
        public void BasicReadTest_PropsOnClass()
        {
            var now = DateTime.Now;

            var obj = new PropsOnClass() { A = 123, B = "abc", C = now, D = null };

            var access = MemberAccess.GetAccessor(typeof(PropsOnClass));

            Assert.AreEqual(123, access[obj, "A"]);
            Assert.AreEqual("abc", access[obj, "B"]);
            Assert.AreEqual(now, access[obj, "C"]);
            Assert.AreEqual(null, access[obj, "D"]);
        }

        [Test]
        public void BasicWriteTest_PropsOnClass()
        {
            var now = DateTime.Now;

            var obj = new PropsOnClass();

            var access = MemberAccess.GetAccessor(typeof(PropsOnClass));

            access[obj, "A"] = 123;
            access[obj, "B"] = "abc";
            access[obj, "C"] = now;
            access[obj, "D"] = null;

            Assert.AreEqual(123, obj.A);
            Assert.AreEqual("abc", obj.B);
            Assert.AreEqual(now, obj.C);
            Assert.AreEqual(null, obj.D);
        }

        [Test]
        public void BasicReadTest_PropsOnClass_ViaWrapper()
        {
            var now = DateTime.Now;

            var obj = new PropsOnClass() { A = 123, B = "abc", C = now, D = null };

            var wrapper = MemberAccess.Wrap(obj);

            Assert.AreEqual(123, wrapper["A"]);
            Assert.AreEqual("abc", wrapper["B"]);
            Assert.AreEqual(now, wrapper["C"]);
            Assert.AreEqual(null, wrapper["D"]);
        }

        [Test]
        public void BasicWriteTest_PropsOnClass_ViaWrapper()
        {
            var now = DateTime.Now;

            var obj = new PropsOnClass();

            var wrapper = MemberAccess.Wrap(obj);

            wrapper["A"] = 123;
            wrapper["B"] = "abc";
            wrapper["C"] = now;
            wrapper["D"] = null;

            Assert.AreEqual(123, obj.A);
            Assert.AreEqual("abc", obj.B);
            Assert.AreEqual(now, obj.C);
            Assert.AreEqual(null, obj.D);
        }

        [Test]
        public void BasicReadTest_FieldsOnClass()
        {
            var now = DateTime.Now;

            var obj = new FieldsOnClass() { A = 123, B = "abc", C = now, D = null };

            var access = MemberAccess.GetAccessor(typeof(FieldsOnClass));

            Assert.AreEqual(123, access[obj, "A"]);
            Assert.AreEqual("abc", access[obj, "B"]);
            Assert.AreEqual(now, access[obj, "C"]);
            Assert.AreEqual(null, access[obj, "D"]);
        }

        [Test]
        public void BasicWriteTest_FieldsOnClass()
        {
            var now = DateTime.Now;

            var obj = new FieldsOnClass();

            var access = MemberAccess.GetAccessor(typeof(FieldsOnClass));

            access[obj, "A"] = 123;
            access[obj, "B"] = "abc";
            access[obj, "C"] = now;
            access[obj, "D"] = null;

            Assert.AreEqual(123, obj.A);
            Assert.AreEqual("abc", obj.B);
            Assert.AreEqual(now, obj.C);
            Assert.AreEqual(null, obj.D);
        }

        [Test]
        public void BasicReadTest_PropsOnStruct()
        {
            var now = DateTime.Now;

            var obj = new PropsOnStruct() { A = 123, B = "abc", C = now, D = null };

            var access = MemberAccess.GetAccessor(typeof(PropsOnStruct));

            Assert.AreEqual(123, access[obj, "A"]);
            Assert.AreEqual("abc", access[obj, "B"]);
            Assert.AreEqual(now, access[obj, "C"]);
            Assert.AreEqual(null, access[obj, "D"]);
        }

        [Test, ExpectedException(typeof(NotSupportedException))]
        public void BasicWriteTest_PropsOnStruct()
        {
            var now = DateTime.Now;

            var obj = new PropsOnStruct();

            var access = MemberAccess.GetAccessor(typeof(PropsOnStruct));

            access[obj, "A"] = 123;
        }

        [Test]
        public void BasicReadTest_FieldsOnStruct()
        {
            var now = DateTime.Now;

            var obj = new FieldsOnStruct() { A = 123, B = "abc", C = now, D = null };

            var access = MemberAccess.GetAccessor(typeof(FieldsOnStruct));

            Assert.AreEqual(123, access[obj, "A"]);
            Assert.AreEqual("abc", access[obj, "B"]);
            Assert.AreEqual(now, access[obj, "C"]);
            Assert.AreEqual(null, access[obj, "D"]);
        }

        [Test, ExpectedException(typeof(NotSupportedException))]
        public void BasicWriteTest_FieldsOnStruct()
        {
            var now = DateTime.Now;

            object obj = new FieldsOnStruct();
            
            var access = MemberAccess.GetAccessor(typeof(FieldsOnStruct));

            access[obj, "A"] = 123;
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void WriteInvalidMember()
        {
            var access = MemberAccess.GetAccessor(typeof(PropsOnClass));
            var obj = new PropsOnClass();
            access[obj, "doesnotexist"] = "abc";
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ReadInvalidMember()
        {
            var access = MemberAccess.GetAccessor(typeof(PropsOnClass));
            var obj = new PropsOnClass();
            object value = access[obj, "doesnotexist"];
        }

        [Test]
        public void GetSameAccessor()
        {
            var x = MemberAccess.GetAccessor(typeof(PropsOnClass));
            var y = MemberAccess.GetAccessor(typeof(PropsOnClass));
            Assert.AreSame(x, y);
        }

        public class PropsOnClass
        {
            public int A { get; set; }
            public string B { get; set; }
            public DateTime? C { get; set; }
            public decimal? D { get; set; }
        }
        public class FieldsOnClass
        {
            public int A;
            public string B;
            public DateTime? C;
            public decimal? D;
        }
        public struct PropsOnStruct
        {
            public int A { get; set; }
            public string B { get; set; }
            public DateTime? C { get; set; }
            public decimal? D { get; set; }
        }
        public struct FieldsOnStruct
        {
            public int A;
            public string B;
            public DateTime? C;
            public decimal? D;
        }
    }
}
