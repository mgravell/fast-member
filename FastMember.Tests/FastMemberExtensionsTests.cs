using System;
using NUnit.Framework;
using System.Data;
using FastMember;
// ReSharper disable InconsistentNaming

namespace FastMemberTests
{
    /// <summary> for testing access to nested things. 
    ///  <para /> https://github.com/mgravell/fast-member/issues/21 
    ///  <para /> http://stackoverflow.com/a/40364078/492 </summary> 
    [TestFixture]
    public class FastMemberExtensionsTests
    {
        [Test]
        public void ReadTest_NestedPropsOnClass_ViaWrapperExtension()
        {
            var now = DateTime.Now;

            var obj = new PropsOnClass() { A = 123, B = "abc", C = now, D = null };

            var access = TypeAccessor.Create(typeof(PropsOnClass));

            var nester = new NestedPropsOnClass() {PropsOnClass =   obj};
            var nesterAccessor = ObjectAccessor.Create(nester);

            Assert.AreEqual(123  , access[obj, "A"]);
            Assert.AreEqual("abc", access[obj, "B"]);
            Assert.AreEqual(now  , access[obj, "C"]);
            Assert.AreEqual(null , access[obj, "D"]);

            Assert.AreEqual(123  , nesterAccessor.GetValueOfDeepestNestedProperty("PropsOnClass.A"));
            Assert.AreEqual("abc", nesterAccessor.GetValueOfDeepestNestedProperty("PropsOnClass.B"));
            Assert.AreEqual(now  , nesterAccessor.GetValueOfDeepestNestedProperty("PropsOnClass.C"));
            Assert.AreEqual(null , nesterAccessor.GetValueOfDeepestNestedProperty("PropsOnClass.D"));

            Assert.AreEqual(nesterAccessor.GetValueOfDeepestNestedProperty("PropsOnClass.A")?.GetType(), typeof(int));
            Assert.AreEqual(nesterAccessor.GetValueOfDeepestNestedProperty("PropsOnClass.B")?.GetType(), obj.B?.GetType());
            Assert.AreEqual(nesterAccessor.GetValueOfDeepestNestedProperty("PropsOnClass.C")?.GetType(), obj.C?.GetType());
            Assert.AreEqual(nesterAccessor.GetValueOfDeepestNestedProperty("PropsOnClass.D")?.GetType(), obj.D?.GetType());
        }

        [Test]
        public void WriteTest_NestedPropsOnClass_ViaWrapperExtension()
        {
            var now = DateTime.Now;
            var Neil = DateTime.Parse("July 20 1969 20:18");

            var obj = new PropsOnClass();

            var access = TypeAccessor.Create(typeof(PropsOnClass));

            var nester = new NestedPropsOnClass() { PropsOnClass =   obj };
            var nesterAccessor = ObjectAccessor.Create(nester);

            access[obj, "A"] = 123;
            access[obj, "B"] = "abc";
            access[obj, "C"] = now;
            access[obj, "D"] = null;


            Assert.AreEqual(123  , obj.A);
            Assert.AreEqual("abc", obj.B);
            Assert.AreEqual(now  , obj.C);
            Assert.AreEqual(null , obj.D);

            Assert.IsNotNull(nesterAccessor.GetValueOfDeepestNestedProperty("PropsOnClass"));

            Assert.AreEqual(123  , nesterAccessor.GetValueOfDeepestNestedProperty("PropsOnClass.A"));
            Assert.AreEqual("abc", nesterAccessor.GetValueOfDeepestNestedProperty("PropsOnClass.B"));
            Assert.AreEqual(now  , nesterAccessor.GetValueOfDeepestNestedProperty("PropsOnClass.C"));
            Assert.AreEqual(null , nesterAccessor.GetValueOfDeepestNestedProperty("PropsOnClass.D"));

            nesterAccessor.SetValueOfDeepestNestedProperty("PropsOnClass.A", 456);
            nesterAccessor.SetValueOfDeepestNestedProperty("PropsOnClass.B", "cde");
            nesterAccessor.SetValueOfDeepestNestedProperty("PropsOnClass.C", Neil);
            nesterAccessor.SetValueOfDeepestNestedProperty("PropsOnClass.D", 3.14m);

            Assert.AreEqual(456  , obj.A);
            Assert.AreEqual("cde", obj.B);
            Assert.AreEqual(Neil , obj.C);
            Assert.AreEqual(3.14m, obj.D);

            Assert.AreEqual(456  , nesterAccessor.GetValueOfDeepestNestedProperty("PropsOnClass.A"));
            Assert.AreEqual("cde", nesterAccessor.GetValueOfDeepestNestedProperty("PropsOnClass.B"));
            Assert.AreEqual(Neil , nesterAccessor.GetValueOfDeepestNestedProperty("PropsOnClass.C"));
            Assert.AreEqual(3.14m, nesterAccessor.GetValueOfDeepestNestedProperty("PropsOnClass.D"));

            Assert.AreEqual(nesterAccessor.GetValueOfDeepestNestedProperty("PropsOnClass.A")?.GetType(), typeof(int));
            Assert.AreEqual(nesterAccessor.GetValueOfDeepestNestedProperty("PropsOnClass.B")?.GetType(), obj.B?.GetType());
            Assert.AreEqual(nesterAccessor.GetValueOfDeepestNestedProperty("PropsOnClass.C")?.GetType(), obj.C?.GetType());
            Assert.AreEqual(nesterAccessor.GetValueOfDeepestNestedProperty("PropsOnClass.D")?.GetType(), obj.D?.GetType());
        }



        [Test]
        public void ReadTest_NestedNestedPropsOnClass_ViaWrapperExtension()
        {
            var now = DateTime.Now;

            var obj = new PropsOnClass() { A = 123, B = "abc", C = now, D = null };

            var access = TypeAccessor.Create(typeof(PropsOnClass));

            NestedPropsOnClass nestedProps = new NestedPropsOnClass() { PropsOnClass =   obj };
            NestedPropsOnClass nestedClass  = new NestedPropsOnClass() { NestedNestedPropsOnClass = nestedProps };
            ObjectAccessor nesterAccessor = ObjectAccessor.Create(nestedClass);

            nestedClass.NestedNestedPropsOnClass.PropsOnClass.A = 911;

            Assert.AreEqual(911  , access[obj, "A"]);
            Assert.AreEqual("abc", access[obj, "B"]);
            Assert.AreEqual(now  , access[obj, "C"]);
            Assert.AreEqual(null , access[obj, "D"]);

            Assert.IsNotNull(nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass"));
            Assert.IsNotNull(nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.PropsOnClass"));
            Assert.IsNotNull(nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.PropsOnClass.A"));

            Assert.AreEqual(911  , nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.PropsOnClass.A"));
            Assert.AreEqual("abc", nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.PropsOnClass.B"));
            Assert.AreEqual(now  , nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.PropsOnClass.C"));
            Assert.AreEqual(null , nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.PropsOnClass.D"));

            Assert.AreEqual(nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.PropsOnClass.A")?.GetType(), typeof(int));
            Assert.AreEqual(nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.PropsOnClass.B")?.GetType(), obj.B?.GetType());
            Assert.AreEqual(nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.PropsOnClass.C")?.GetType(), obj.C?.GetType());
            Assert.AreEqual(nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.PropsOnClass.D")?.GetType(), obj.D?.GetType());
        }

        [Test]
        public void ReadWriteTest_NestedNestedNestedPropsOnClass_ViaWrapperExtension()
        {
            var now = DateTime.Now;
            var Neil = DateTime.Parse("July 20 1969 20:18");

            var obj = new PropsOnClass() { A = 123, B = "abc", C = now, D = null };

            var access = TypeAccessor.Create(typeof(PropsOnClass));

            NestedPropsOnClass nestedProps = new NestedPropsOnClass() { PropsOnClass =   obj };
            NestedPropsOnClass nestedClass = new NestedPropsOnClass() { NestedNestedPropsOnClass = nestedProps };
            NestedPropsOnClass nestedNestedClass = new NestedPropsOnClass() { NestedNestedPropsOnClass = nestedClass };
            ObjectAccessor nesterAccessor = ObjectAccessor.Create(nestedNestedClass);

            Assert.AreEqual(123, access[obj, "A"]);
            Assert.AreEqual("abc", access[obj, "B"]);
            Assert.AreEqual(now, access[obj, "C"]);
            Assert.AreEqual(null, access[obj, "D"]);

            nesterAccessor.SetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.NestedNestedPropsOnClass.PropsOnClass.A", 456);
            nesterAccessor.SetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.NestedNestedPropsOnClass.PropsOnClass.B", "cde");
            nesterAccessor.SetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.NestedNestedPropsOnClass.PropsOnClass.C", Neil);
            nesterAccessor.SetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.NestedNestedPropsOnClass.PropsOnClass.D", 3.14m);

            Assert.AreEqual(456  , access[obj, "A"]);
            Assert.AreEqual("cde", access[obj, "B"]);
            Assert.AreEqual(Neil , access[obj, "C"]);
            Assert.AreEqual(3.14m, access[obj, "D"]);

            Assert.IsNotNull(nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.NestedNestedPropsOnClass"));
            Assert.IsNotNull(nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.NestedNestedPropsOnClass.PropsOnClass"));
            Assert.IsNotNull(nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.NestedNestedPropsOnClass.PropsOnClass.A"));

            Assert.AreEqual(456  , nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.NestedNestedPropsOnClass.PropsOnClass.A"));
            Assert.AreEqual("cde", nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.NestedNestedPropsOnClass.PropsOnClass.B"));
            Assert.AreEqual(Neil , nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.NestedNestedPropsOnClass.PropsOnClass.C"));
            Assert.AreEqual(3.14m, nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.NestedNestedPropsOnClass.PropsOnClass.D"));

            Assert.AreEqual(nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.NestedNestedPropsOnClass.PropsOnClass.A")?.GetType(), typeof(int));
            Assert.AreEqual(nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.NestedNestedPropsOnClass.PropsOnClass.B")?.GetType(), obj.B?.GetType());
            Assert.AreEqual(nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.NestedNestedPropsOnClass.PropsOnClass.C")?.GetType(), obj.C?.GetType());
            Assert.AreEqual(nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.NestedNestedPropsOnClass.PropsOnClass.D")?.GetType(), obj.D?.GetType());
        }




        [Test]
        public void BasicReadTest_NestedNestedFieldsOnClass()
        {
            var now = DateTime.Now;
            var Neil = DateTime.Parse("July 20 1969 20:18");

            var obj = new FieldsOnClass() { A = 123, B = "abc", C = now, D = null };

            var access = TypeAccessor.Create(typeof(FieldsOnClass));

            NestedPropsOnClass nestedProps = new NestedPropsOnClass() { FieldsOnClass =   obj };
            NestedPropsOnClass nestedClass = new NestedPropsOnClass() { NestedNestedPropsOnClass = nestedProps };
            NestedPropsOnClass nestedNestedClass = new NestedPropsOnClass() { NestedNestedPropsOnClass = nestedClass };
            ObjectAccessor nesterAccessor = ObjectAccessor.Create(nestedNestedClass);

            Assert.AreEqual(123, access[obj, "A"]);
            Assert.AreEqual("abc", access[obj, "B"]);
            Assert.AreEqual(now, access[obj, "C"]);
            Assert.AreEqual(null, access[obj, "D"]);

            Assert.AreEqual(123, nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.NestedNestedPropsOnClass.FieldsOnClass.A"));
            Assert.AreEqual("abc", nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.NestedNestedPropsOnClass.FieldsOnClass.B"));
            Assert.AreEqual(now, nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.NestedNestedPropsOnClass.FieldsOnClass.C"));
            Assert.AreEqual(null, nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.NestedNestedPropsOnClass.FieldsOnClass.D"));


            // bug: can't set nested fields
             /*
            nesterAccessor.SetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.NestedNestedPropsOnClass.FieldsOnClass.A", 456);
            nesterAccessor.SetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.NestedNestedPropsOnClass.FieldsOnClass.B", "cde");
            nesterAccessor.SetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.NestedNestedPropsOnClass.FieldsOnClass.C", Neil);
            nesterAccessor.SetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.NestedNestedPropsOnClass.FieldsOnClass.D", 3.14m);
            */

           obj.A = 456  ;
           obj.B = "cde";
           obj.C = Neil ;
           obj.D = 3.14m;

            Assert.IsNotNull(nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.NestedNestedPropsOnClass"));
            Assert.IsNotNull(nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.NestedNestedPropsOnClass.FieldsOnClass"));
            Assert.IsNotNull(nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.NestedNestedPropsOnClass.FieldsOnClass.A"));

            Assert.AreEqual(456, nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.NestedNestedPropsOnClass.FieldsOnClass.A"));
            Assert.AreEqual("cde", nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.NestedNestedPropsOnClass.FieldsOnClass.B"));
            Assert.AreEqual(Neil, nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.NestedNestedPropsOnClass.FieldsOnClass.C"));
            Assert.AreEqual(3.14m, nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.NestedNestedPropsOnClass.FieldsOnClass.D"));

            Assert.AreEqual(nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.NestedNestedPropsOnClass.FieldsOnClass.A")?.GetType(), typeof(int));
            Assert.AreEqual(nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.NestedNestedPropsOnClass.FieldsOnClass.B")?.GetType(), obj.B?.GetType());
            Assert.AreEqual(nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.NestedNestedPropsOnClass.FieldsOnClass.C")?.GetType(), obj.C?.GetType());
            Assert.AreEqual(nesterAccessor.GetValueOfDeepestNestedProperty("NestedNestedPropsOnClass.NestedNestedPropsOnClass.FieldsOnClass.D")?.GetType(), obj.D?.GetType());

        }

        [Test]
        public void BasicWriteTest_FieldsOnClass()
        {
            var now = DateTime.Now;

            var obj = new FieldsOnClass();

            var access = TypeAccessor.Create(typeof(FieldsOnClass));

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

            var access = TypeAccessor.Create(typeof(PropsOnStruct));

            Assert.AreEqual(123, access[obj, "A"]);
            Assert.AreEqual("abc", access[obj, "B"]);
            Assert.AreEqual(now, access[obj, "C"]);
            Assert.AreEqual(null, access[obj, "D"]);
        }

        [Test]
        public void BasicWriteTest_PropsOnStruct()
        {
            var now = DateTime.Now;

            object obj = new PropsOnStruct { A = 1 };

            var access = TypeAccessor.Create(typeof(PropsOnStruct));

            access[obj, "A"] = 123;

            Assert.AreEqual(123, ((PropsOnStruct)obj).A);
        }

        [Test]
        public void BasicReadTest_FieldsOnStruct()
        {
            var now = DateTime.Now;

            var obj = new FieldsOnStruct() { A = 123, B = "abc", C = now, D = null };

            var access = TypeAccessor.Create(typeof(FieldsOnStruct));

            Assert.AreEqual(123, access[obj, "A"]);
            Assert.AreEqual("abc", access[obj, "B"]);
            Assert.AreEqual(now, access[obj, "C"]);
            Assert.AreEqual(null, access[obj, "D"]);
        }

        [Test]
        public void BasicWriteTest_FieldsOnStruct()
        {
            var now = DateTime.Now;

            object obj = new FieldsOnStruct();

            var access = TypeAccessor.Create(typeof(FieldsOnStruct));

            access[obj, "A"] = 123;
            Assert.AreEqual(123, ((FieldsOnStruct)obj).A);
        }

        [Test]
        public void WriteInvalidMember()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var access = TypeAccessor.Create(typeof(PropsOnClass));
                var obj = new PropsOnClass();
                access[obj, "doesnotexist"] = "abc";
            });
        }


        [Test]
        public void GetSameAccessor()
        {
            var x = TypeAccessor.Create(typeof(PropsOnClass));
            var y = TypeAccessor.Create(typeof(PropsOnClass));
            Assert.AreSame(x, y);
        }

        public class PropsOnClass
        {
            public int A { get; set; }
            public string B { get; set; }
            public DateTime? C { get; set; }
            public decimal? D { get; set; }
        }


        /// <summary> for testing access to nested things. 
        ///  <para /> https://github.com/mgravell/fast-member/issues/21 
        ///  <para /> http://stackoverflow.com/a/40364078/492 </summary> 
        public class NestedPropsOnClass
        {
            public PropsOnClass PropsOnClass  { get; set; }
            public FieldsOnClass FieldsOnClass  { get; set; }
            public PropsOnStruct PropsOnStruct  { get; set; }
            public FieldsOnStruct FieldsOnStruct  { get; set; }
            public NestedPropsOnClass NestedNestedPropsOnClass  { get; set; }
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