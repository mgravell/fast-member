using FastMember;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Xunit;

namespace FastMemberTests
{
    public class BasicTests
    {
        [Fact]
        public void BasicReadTest_PropsOnClass()
        {
            var now = DateTime.Now;

            var obj = new PropsOnClass() { A = 123, B = "abc", C = now, D = null };

            var access = TypeAccessor.Create(typeof(PropsOnClass));

            Assert.Equal(123, access[obj, "A"]);
            Assert.Equal("abc", access[obj, "B"]);
            Assert.Equal(now, access[obj, "C"]);
            Assert.Null(access[obj, "D"]);
        }

        [Fact]
        public void BasicWriteTest_PropsOnClass()
        {
            var now = DateTime.Now;

            var obj = new PropsOnClass();

            var access = TypeAccessor.Create(typeof(PropsOnClass));

            access[obj, "A"] = 123;
            access[obj, "B"] = "abc";
            access[obj, "C"] = now;
            access[obj, "D"] = null;

            Assert.Equal(123, obj.A);
            Assert.Equal("abc", obj.B);
            Assert.Equal(now, obj.C);
            Assert.Null(obj.D);
        }

        [Fact]
        public void Getmembers()
        {
            var access = TypeAccessor.Create(typeof(PropsOnClass));
            Assert.True(access.GetMembersSupported);
            var members = access.GetMembers();
            Assert.Equal(4, members.Count);
            Assert.Equal("A", members[0].Name);
            Assert.Equal("B", members[1].Name);
            Assert.Equal("C", members[2].Name);
            Assert.Equal("D", members[3].Name);
        }

        [Fact]
        public void BasicReadTest_PropsOnClass_ViaWrapper()
        {
            var now = DateTime.Now;

            var obj = new PropsOnClass() { A = 123, B = "abc", C = now, D = null };

            var wrapper = ObjectAccessor.Create(obj);

            Assert.Equal(123, wrapper["A"]);
            Assert.Equal("abc", wrapper["B"]);
            Assert.Equal(now, wrapper["C"]);
            Assert.Null(wrapper["D"]);
        }

        [Fact]
        public void BasicWriteTest_PropsOnClass_ViaWrapper()
        {
            var now = DateTime.Now;

            var obj = new PropsOnClass();

            var wrapper = ObjectAccessor.Create(obj);

            wrapper["A"] = 123;
            wrapper["B"] = "abc";
            wrapper["C"] = now;
            wrapper["D"] = null;

            Assert.Equal(123, obj.A);
            Assert.Equal("abc", obj.B);
            Assert.Equal(now, obj.C);
            Assert.Null(obj.D);
        }

        [Fact]
        public void BasicReadTest_FieldsOnClass()
        {
            var now = DateTime.Now;

            var obj = new FieldsOnClass() { A = 123, B = "abc", C = now, D = null };

            var access = TypeAccessor.Create(typeof(FieldsOnClass));

            Assert.Equal(123, access[obj, "A"]);
            Assert.Equal("abc", access[obj, "B"]);
            Assert.Equal(now, access[obj, "C"]);
            Assert.Null(access[obj, "D"]);
        }

        [Fact]
        public void BasicWriteTest_FieldsOnClass()
        {
            var now = DateTime.Now;

            var obj = new FieldsOnClass();

            var access = TypeAccessor.Create(typeof(FieldsOnClass));

            access[obj, "A"] = 123;
            access[obj, "B"] = "abc";
            access[obj, "C"] = now;
            access[obj, "D"] = null;

            Assert.Equal(123, obj.A);
            Assert.Equal("abc", obj.B);
            Assert.Equal(now, obj.C);
            Assert.Null(obj.D);
        }

        [Fact]
        public void BasicReadTest_PropsOnStruct()
        {
            var now = DateTime.Now;

            var obj = new PropsOnStruct() { A = 123, B = "abc", C = now, D = null };

            var access = TypeAccessor.Create(typeof(PropsOnStruct));

            Assert.Equal(123, access[obj, "A"]);
            Assert.Equal("abc", access[obj, "B"]);
            Assert.Equal(now, access[obj, "C"]);
            Assert.Null(access[obj, "D"]);
        }

        [Fact]
        public void BasicWriteTest_PropsOnStruct()
        {
            var now = DateTime.Now;

            object obj = new PropsOnStruct { A = 1 };

            var access = TypeAccessor.Create(typeof(PropsOnStruct));

            access[obj, "A"] = 123;
            
            Assert.Equal(123, ((PropsOnStruct)obj).A);
        }

        [Fact]
        public void BasicReadTest_FieldsOnStruct()
        {
            var now = DateTime.Now;

            var obj = new FieldsOnStruct() { A = 123, B = "abc", C = now, D = null };

            var access = TypeAccessor.Create(typeof(FieldsOnStruct));

            Assert.Equal(123, access[obj, "A"]);
            Assert.Equal("abc", access[obj, "B"]);
            Assert.Equal(now, access[obj, "C"]);
            Assert.Null(access[obj, "D"]);
        }

        [Fact]
        public void BasicWriteTest_FieldsOnStruct()
        {
            var now = DateTime.Now;

            object obj = new FieldsOnStruct();
            
            var access = TypeAccessor.Create(typeof(FieldsOnStruct));

            access[obj, "A"] = 123;
            Assert.Equal(123, ((FieldsOnStruct)obj).A);
        }

        [Fact]
        public void WriteInvalidMember()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var access = TypeAccessor.Create(typeof(PropsOnClass));
                var obj = new PropsOnClass();
                access[obj, "doesnotexist"] = "abc";
            });
        }

        [Fact]
        public void ReadInvalidMember()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var access = TypeAccessor.Create(typeof(PropsOnClass));
                var obj = new PropsOnClass();
                object value = access[obj, "doesnotexist"];
            });
        }

        [Fact]
        public void GetSameAccessor()
        {
            var x = TypeAccessor.Create(typeof(PropsOnClass));
            var y = TypeAccessor.Create(typeof(PropsOnClass));
            Assert.Same(x, y);
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

        public interface IPropsOnInterfaceBase
        {
            int A { get; set; }
            string B { get; set; }
            DateTime? C { get; set; }
            decimal? D { get; set; }
        }
        public interface IPropsOnInterfaceBase2
        {
            int E { get; set; }
            string F { get; set; }
            DateTime? G { get; set; }
            decimal? H { get; set; }
        }
        public interface IPropsOnInheritedInterface : IPropsOnInterfaceBase
        {
            int I { get; set; }
            string J { get; set; }
            DateTime? K { get; set; }
            decimal? L { get; set; }
        }
        public interface IPropseOnComposedInterface : IPropsOnInterfaceBase, IPropsOnInterfaceBase2
        {
            int M { get; set; }
            string N { get; set; }
            DateTime? O { get; set; }
            decimal? P { get; set; }
        }
        public interface IPropseOnInheritedComposedInterface : IPropsOnInheritedInterface, IPropsOnInterfaceBase2
        {
            int M { get; set; }
            string N { get; set; }
            DateTime? O { get; set; }
            decimal? P { get; set; }
        }


        public class HasDefaultCtor { }
        public class HasNoDefaultCtor { public HasNoDefaultCtor(string s) { } }
        public abstract class IsAbstract { }   

        [Fact]
        public void TestCtor()
        {
            var accessor = TypeAccessor.Create(typeof(HasNoDefaultCtor));
            Assert.False(accessor.CreateNewSupported);

            accessor = TypeAccessor.Create(typeof(IsAbstract));
            Assert.False(accessor.CreateNewSupported);

            Assert.NotEqual("DynamicAccessor", accessor.GetType().Name);
            Assert.NotEqual("DelegateAccessor", accessor.GetType().Name);

            accessor = TypeAccessor.Create(typeof (HasDefaultCtor));
            Assert.True(accessor.CreateNewSupported);
            object obj = accessor.CreateNew();
            Assert.IsType<HasDefaultCtor>(obj);
        }

        public class HasGetterNoSetter
        {
            public int Foo { get { return 5; } }
        }
        [Fact]
        public void TestHasGetterNoSetter()
        {
            var obj = new HasGetterNoSetter();
            var acc = TypeAccessor.Create(typeof (HasGetterNoSetter));
            Assert.Equal(5, acc[obj, "Foo"]);
        }
        public class HasGetterPrivateSetter
        {
            public int Foo { get; private set; }
            public HasGetterPrivateSetter(int value) { Foo = value; }
        }
        [Fact]
        public void TestHasGetterPrivateSetter()
        {
            var obj = new HasGetterPrivateSetter(5);
            var acc = TypeAccessor.Create(typeof(HasGetterPrivateSetter));
            Assert.Equal(5, acc[obj, "Foo"]);
        }
        public class HasInternalGetterSetter
        {
            internal int Foo { get; set; }
        }
        [Fact]
        public void TestHasInternalGetterSetter()
        {
            var obj = new HasInternalGetterSetter{Foo = 5};
            var acc = TypeAccessor.Create(typeof(HasInternalGetterSetter), true);
            Assert.Equal(5, acc[obj, "Foo"]);
        }
        public class HasPrivateGetterSetter
        {
            public HasPrivateGetterSetter(int foo)
            {
                Foo = foo;
            }
            private int Foo { get; set; }
        }
        [Fact]
        public void TestHasPrivateGetterSetter()
        {
            var obj = new HasPrivateGetterSetter(5);
            var acc = TypeAccessor.Create(typeof(HasPrivateGetterSetter), true);
            Assert.Equal(5, acc[obj, "Foo"]);
        }

        public class MixedAccess
        {
            public MixedAccess()
            {
                Foo = Bar = Alpha = Beta = 2;
            }
            public int Foo { get; private set; }
            public int Bar { private get; set; }
            public readonly int Alpha;
            public int Beta { get; }
            public int Theta { get { return 5; } }
        }

        [Fact]
        public void TestMixedAccess()
        {
            TypeAccessor acc0 = TypeAccessor.Create(typeof(MixedAccess)),
                         acc1 = TypeAccessor.Create(typeof(MixedAccess), false),
                         acc2 = TypeAccessor.Create(typeof(MixedAccess), true);

            Assert.Same(acc0, acc1);
            Assert.NotSame(acc0, acc2);

            var obj = new MixedAccess();
            Assert.Equal(2, acc0[obj, "Foo"]);
            Assert.Equal(2, acc2[obj, "Foo"]);
            Assert.Equal(2, acc2[obj, "Bar"]);

            acc0[obj, "Bar"] = 3;
            Assert.Equal(3, acc2[obj, "Bar"]);
            acc2[obj, "Bar"] = 4;
            Assert.Equal(4, acc2[obj, "Bar"]);
            acc2[obj, "Foo"] = 5;
            Assert.Equal(5, acc0[obj, "Foo"]);
            acc2[obj, "Alpha"] = 6;
            Assert.Equal(6, acc2[obj, "Alpha"]);
            acc2[obj, "Beta"] = 7;
            Assert.Equal(7, acc2[obj, "Beta"]);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                int i = (int)acc0[obj, "Bar"];
            });
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                acc0[obj, "Foo"] = 6;
            });
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                acc0[obj, "Beta"] = 7;
            });
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                acc0[obj, "Theta"] = 8;
            });
        }

        public class ObjectReaderType {
            public int A {get;set;}
            public string B {get;set;}
            public byte C {get;set;}
            public int? D { get; set; }
        }

        public class ObjectReaderWithDefinedColumnsOrderType
        {
            [Ordinal(3)]
            public int A { get; set; }
            [Ordinal(2)]
            public string B { get; set; }
            [Ordinal(1)]
            public byte C { get; set; }
            [Ordinal(0)]
            public int? D { get; set; }
        }

        public class ObjectReaderWithPartiallyDefinedColumnsOrderType
        {
            // Column A is supposed to be added in the end since
            // it is the only column with the ordinal set.
            [Ordinal(10)]
            public int A { get; set; }

            public string B { get; set; }

            public byte C { get; set; }

            public int? D { get; set; }
        }

        /// <summary>
        /// Tests <see cref="ObjectReader"/> class when the columns are not explicitly provided.
        /// In this case <see cref="OrdinalAttribute"/> attribute is used to define column order.
        /// </summary>
        [Fact]
        public void TestReaderAllColumnsWithPartiallyDefinedOrder()
        {
            var source = new[] {
                new ObjectReaderWithPartiallyDefinedColumnsOrderType { A = 123, B = "abc", C = 1, D = 123},
                new ObjectReaderWithPartiallyDefinedColumnsOrderType { A = 456, B = "def", C = 2, D = null},
                new ObjectReaderWithPartiallyDefinedColumnsOrderType { A = 789, B = "ghi", C = 3, D = 789}
            };
            var table = new DataTable();
            using (var reader = ObjectReader.Create(source))
            {
                table.Load(reader);
            }

            Assert.Equal(4, table.Columns.Count); //, "col count");
            Assert.Equal("A", table.Columns["A"].ColumnName); //, "A/name");
            Assert.Equal("B", table.Columns["B"].ColumnName); //, "B/name");
            Assert.Equal("C", table.Columns["C"].ColumnName); //, "C/name");
            Assert.Equal("D", table.Columns["D"].ColumnName); //, "D/name");
            Assert.Same(typeof(int), table.Columns["A"].DataType); //, "A/type");
            Assert.Same(typeof(string), table.Columns["B"].DataType); //, "B/type");
            Assert.Same(typeof(byte), table.Columns["C"].DataType); //, "C/type");
            Assert.Same(typeof(int), table.Columns["D"].DataType); //, "D/type");
            Assert.False(table.Columns["A"].AllowDBNull, "A/null");
            Assert.True(table.Columns["B"].AllowDBNull, "B/null");
            Assert.False(table.Columns["C"].AllowDBNull, "C/null");
            Assert.True(table.Columns["D"].AllowDBNull, "D/null");

            // Check column order
            Assert.Equal(3, table.Columns["A"].Ordinal); //, "A/ordinal");
            Assert.Equal(0, table.Columns["B"].Ordinal); //, "B/ordinal");
            Assert.Equal(1, table.Columns["C"].Ordinal); //, "C/ordinal");
            Assert.Equal(2, table.Columns["D"].Ordinal); //, "D/ordinal");


            Assert.Equal(3, table.Rows.Count); //, "row count");
            Assert.Equal("abc", table.Rows[0][0]); //,"0,0");       // B column
            Assert.Equal((byte)1, table.Rows[0][1]); //, "0,1")     // C column;
            Assert.Equal(123, table.Rows[0][2]); //, "0,2")         // D column;
            Assert.Equal(123, table.Rows[0][3]); //, "0,2");        // A column
            Assert.Equal("def", table.Rows[1][0]); //, "1,0");          // B column
            Assert.Equal((byte)2, table.Rows[1][1]); //, "1,1");        // C column
            Assert.Equal(DBNull.Value, table.Rows[1][2]); //, "1,2");   // D column
            Assert.Equal(456, table.Rows[1][3]); //, "1,2");            // A column
            Assert.Equal("ghi", table.Rows[2][0]); //, "2,0");      // B column
            Assert.Equal((byte)3, table.Rows[2][1]); //, "2,1");    // C column
            Assert.Equal(789, table.Rows[2][2]); //, "2,2");        // D column
            Assert.Equal(789, table.Rows[2][3]); //, "2,2");        // A column

        }

        /// <summary>
        /// Tests <see cref="ObjectReader"/> class when the columns are not explicitly provided.
        /// In this case <see cref="OrdinalAttribute"/> attribute is used to define column order.
        /// </summary>
        [Fact]
        public void TestReaderAllColumnsWithDefinedOrder()
        {
            var source = new[] {
                new ObjectReaderWithDefinedColumnsOrderType { A = 123, B = "abc", C = 1, D = 123},
                new ObjectReaderWithDefinedColumnsOrderType { A = 456, B = "def", C = 2, D = null},
                new ObjectReaderWithDefinedColumnsOrderType { A = 789, B = "ghi", C = 3, D = 789}
            };
            var table = new DataTable();
            using (var reader = ObjectReader.Create(source))
            {
                table.Load(reader);
            }

            Assert.Equal(4, table.Columns.Count); //, "col count");
            Assert.Equal("A", table.Columns["A"].ColumnName); //, "A/name");
            Assert.Equal("B", table.Columns["B"].ColumnName); //, "B/name");
            Assert.Equal("C", table.Columns["C"].ColumnName); //, "C/name");
            Assert.Equal("D", table.Columns["D"].ColumnName); //, "D/name");
            Assert.Same(typeof(int), table.Columns["A"].DataType); //, "A/type");
            Assert.Same(typeof(string), table.Columns["B"].DataType); //, "B/type");
            Assert.Same(typeof(byte), table.Columns["C"].DataType); //, "C/type");
            Assert.Same(typeof(int), table.Columns["D"].DataType); //, "D/type");
            Assert.False(table.Columns["A"].AllowDBNull, "A/null");
            Assert.True(table.Columns["B"].AllowDBNull, "B/null");
            Assert.False(table.Columns["C"].AllowDBNull, "C/null");
            Assert.True(table.Columns["D"].AllowDBNull, "D/null");

            // Check column order
            Assert.Equal(3, table.Columns["A"].Ordinal); //, "A/ordinal");
            Assert.Equal(2, table.Columns["B"].Ordinal); //, "B/ordinal");
            Assert.Equal(1, table.Columns["C"].Ordinal); //, "C/ordinal");
            Assert.Equal(0, table.Columns["D"].Ordinal); //, "D/ordinal");


            Assert.Equal(3, table.Rows.Count); //, "row count");
            Assert.Equal(123, table.Rows[0][0]); //,"0,0");     // D column
            Assert.Equal((byte)1, table.Rows[0][1]); //, "0,1") // C column;
            Assert.Equal("abc", table.Rows[0][2]); //, "0,2")   // B column;
            Assert.Equal(123, table.Rows[0][3]); //, "0,2");    // A column
            Assert.Equal(DBNull.Value, table.Rows[1][0]); //, "1,0");   // D column
            Assert.Equal((byte)2, table.Rows[1][1]); //, "1,1");        // C column
            Assert.Equal("def", table.Rows[1][2]); //, "1,2");          // B column
            Assert.Equal(456, table.Rows[1][3]); //, "1,2");            // A column
            Assert.Equal(789, table.Rows[2][0]); //, "2,0");        // D column
            Assert.Equal((byte)3, table.Rows[2][1]); //, "2,1");    // C column
            Assert.Equal("ghi", table.Rows[2][2]); //, "2,2");      // B column
            Assert.Equal(789, table.Rows[2][3]); //, "2,2");        // A column

        }

        /// <summary>
        /// Tests <see cref="ObjectReader"/> class when the columns are explicitly provided.
        /// In this case <see cref="OrdinalAttribute"/> attribute is ignored.
        /// </summary>
        [Fact]
        public void TestReaderSpecifiedColumnsWithDefinedOrder()
        {
            var source = new[] {
                new ObjectReaderWithDefinedColumnsOrderType { A = 123, B = "abc", C = 1, D = 123},
                new ObjectReaderWithDefinedColumnsOrderType { A = 456, B = "def", C = 2, D = null},
                new ObjectReaderWithDefinedColumnsOrderType { A = 789, B = "ghi", C = 3, D = 789}
            };
            var table = new DataTable();
            using (var reader = ObjectReader.Create(source, "B", "A", "D"))
            {
                table.Load(reader);
            }

            Assert.Equal(3, table.Columns.Count); //, "col count");
            Assert.Equal("B", table.Columns[0].ColumnName); //, "B/name");
            Assert.Equal("A", table.Columns[1].ColumnName); //, "A/name");
            Assert.Equal("D", table.Columns[2].ColumnName); //, "D/name");
            Assert.Same(typeof(string), table.Columns[0].DataType); //, "B/type");
            Assert.Same(typeof(int), table.Columns[1].DataType); //, "A/type");
            Assert.Same(typeof(int), table.Columns[2].DataType); //, "D/type");
            Assert.True(table.Columns[0].AllowDBNull, "B/null");
            Assert.False(table.Columns[1].AllowDBNull, "A/null");
            Assert.True(table.Columns[2].AllowDBNull, "D/null");

            // Check column order
            Assert.Equal(1, table.Columns["A"].Ordinal); //, "A/ordinal");
            Assert.Equal(0, table.Columns["B"].Ordinal); //, "B/ordinal");
            Assert.Equal(2, table.Columns["D"].Ordinal); //, "D/ordinal");

            Assert.Equal(3, table.Rows.Count); //, "row count");
            Assert.Equal("abc", table.Rows[0][0]); //,"0,0");
            Assert.Equal(123, table.Rows[0][1]); //, "0,1");
            Assert.Equal(123, table.Rows[0][2]); //, "0,2");
            Assert.Equal("def", table.Rows[1][0]); //, "1,0");
            Assert.Equal(456, table.Rows[1][1]); //, "1,1");
            Assert.Equal(DBNull.Value, table.Rows[1][2]); //, "1,2");
            Assert.Equal("ghi", table.Rows[2][0]); //, "2,0");
            Assert.Equal(789, table.Rows[2][1]); //, "2,1");
            Assert.Equal(789, table.Rows[2][2]); //, "2,2");

        }

        [Fact]
        public void TestReaderAllColumns()
        {
            var source = new[] {
                new ObjectReaderType { A = 123, B = "abc", C = 1, D = 123},
                new ObjectReaderType { A = 456, B = "def", C = 2, D = null},
                new ObjectReaderType { A = 789, B = "ghi", C = 3, D = 789}
            };
            var table = new DataTable();
            using (var reader = ObjectReader.Create(source))
            {
                table.Load(reader);
            }

            Assert.Equal(4, table.Columns.Count); //, "col count");
            Assert.Equal("A", table.Columns["A"].ColumnName); //, "A/name");
            Assert.Equal("B", table.Columns["B"].ColumnName); //, "B/name");
            Assert.Equal("C", table.Columns["C"].ColumnName); //, "C/name");
            Assert.Equal("D", table.Columns["D"].ColumnName); //, "D/name");
            Assert.Same(typeof(int), table.Columns["A"].DataType); //, "A/type");
            Assert.Same(typeof(string), table.Columns["B"].DataType); //, "B/type");
            Assert.Same(typeof(byte), table.Columns["C"].DataType); //, "C/type");
            Assert.Same(typeof(int), table.Columns["D"].DataType); //, "D/type");
            Assert.False(table.Columns["A"].AllowDBNull, "A/null");
            Assert.True(table.Columns["B"].AllowDBNull, "B/null");
            Assert.False(table.Columns["C"].AllowDBNull, "C/null");
            Assert.True(table.Columns["D"].AllowDBNull, "D/null");

            Assert.Equal(3, table.Rows.Count); //, "row count");
            Assert.Equal(123, table.Rows[0]["A"]); //, "0,A");
            Assert.Equal("abc", table.Rows[0]["B"]); //, "0,B");
            Assert.Equal((byte)1, table.Rows[0]["C"]); //, "0,C");
            Assert.Equal(123, table.Rows[0]["D"]); //, "0,D");
            Assert.Equal(456, table.Rows[1]["A"]); //, "1,A");
            Assert.Equal("def", table.Rows[1]["B"]); //, "1,B");
            Assert.Equal((byte)2, table.Rows[1]["C"]); //, "1,C");
            Assert.Equal(DBNull.Value, table.Rows[1]["D"]); //, "1,D");
            Assert.Equal(789, table.Rows[2]["A"]); //, "2,A");
            Assert.Equal("ghi", table.Rows[2]["B"]); //, "2,B");
            Assert.Equal((byte)3, table.Rows[2]["C"]); //, "2,C");
            Assert.Equal(789, table.Rows[2]["D"]); //, "2,D");
        }

        [Fact]
        public void TestReaderSpecifiedColumns()
        {
            var source = new[] {
                new ObjectReaderType { A = 123, B = "abc", C = 1, D = 123},
                new ObjectReaderType { A = 456, B = "def", C = 2, D = null},
                new ObjectReaderType { A = 789, B = "ghi", C = 3, D = 789}
            };
            var table = new DataTable();
            using (var reader = ObjectReader.Create(source, "B", "A", "D"))
            {
                table.Load(reader);
            }

            Assert.Equal(3, table.Columns.Count); //, "col count");
            Assert.Equal("B", table.Columns[0].ColumnName); //, "B/name");
            Assert.Equal("A", table.Columns[1].ColumnName); //, "A/name");
            Assert.Equal("D", table.Columns[2].ColumnName); //, "D/name");
            Assert.Same(typeof(string), table.Columns[0].DataType); //, "B/type");
            Assert.Same(typeof(int), table.Columns[1].DataType); //, "A/type");
            Assert.Same(typeof(int), table.Columns[2].DataType); //, "D/type");
            Assert.True(table.Columns[0].AllowDBNull, "B/null");
            Assert.False(table.Columns[1].AllowDBNull, "A/null");
            Assert.True(table.Columns[2].AllowDBNull, "D/null");


            Assert.Equal(3, table.Rows.Count); //, "row count");
            Assert.Equal("abc", table.Rows[0][0]); //,"0,0");
            Assert.Equal(123, table.Rows[0][1]); //, "0,1");
            Assert.Equal(123, table.Rows[0][2]); //, "0,2");
            Assert.Equal("def", table.Rows[1][0]); //, "1,0");
            Assert.Equal(456, table.Rows[1][1]); //, "1,1");
            Assert.Equal(DBNull.Value, table.Rows[1][2]); //, "1,2");
            Assert.Equal("ghi", table.Rows[2][0]); //, "2,0");
            Assert.Equal(789, table.Rows[2][1]); //, "2,1");
            Assert.Equal(789, table.Rows[2][2]); //, "2,2");

        }

        public class HazStaticProperty
        {
            public int Foo { get; set; }
            public static int Bar { get; set; }

            public int Foo2 => 2;
            public static int Bar2 => 4;
        }

        [Fact]
        public void IgnoresStaticProperty()
        {
            var obj = new HazStaticProperty();
            var acc = TypeAccessor.Create(typeof(HazStaticProperty));
            var memberNames = string.Join(",", acc.GetMembers().Select(x => x.Name).OrderBy(_ => _));
            Assert.Equal("Foo,Foo2", memberNames);
        }

        [Fact]
        public void TestGetMembersOnInterface()
        {
            var access = TypeAccessor.Create(typeof(IPropsOnInterfaceBase));
            Assert.True(access.GetMembersSupported);
            var members = access.GetMembers();
            Assert.Equal(4, members.Count);
            Assert.Equal("A", members[0].Name);
            Assert.Equal("B", members[1].Name);
            Assert.Equal("C", members[2].Name);
            Assert.Equal("D", members[3].Name);
        }

        [Fact]
        public void TestGetMembersOnInheritedInterface()
        {
            var access = TypeAccessor.Create(typeof(IPropsOnInheritedInterface));
            Assert.True(access.GetMembersSupported);
            var members = access.GetMembers();
            Assert.Equal(8, members.Count);
            Assert.Equal("A", members[0].Name);
            Assert.Equal("B", members[1].Name);
            Assert.Equal("C", members[2].Name);
            Assert.Equal("D", members[3].Name);
            Assert.Equal("I", members[4].Name);
            Assert.Equal("J", members[5].Name);
            Assert.Equal("K", members[6].Name);
            Assert.Equal("L", members[7].Name);
        }

        [Fact]
        public void TestGetMembersOnComposedInterface()
        {
            var access = TypeAccessor.Create(typeof(IPropseOnComposedInterface));
            Assert.True(access.GetMembersSupported);
            var members = access.GetMembers();
            Assert.Equal(12, members.Count);
            Assert.Equal("A", members[0].Name);
            Assert.Equal("B", members[1].Name);
            Assert.Equal("C", members[2].Name);
            Assert.Equal("D", members[3].Name);
            Assert.Equal("E", members[4].Name);
            Assert.Equal("F", members[5].Name);
            Assert.Equal("G", members[6].Name);
            Assert.Equal("H", members[7].Name);
            Assert.Equal("M", members[8].Name);
            Assert.Equal("N", members[9].Name);
            Assert.Equal("O", members[10].Name);
            Assert.Equal("P", members[11].Name);
        }

        [Fact]
        public void TestGetMembersOnInheritedComposedInterface()
        {
            var access = TypeAccessor.Create(typeof(IPropseOnInheritedComposedInterface));
            Assert.True(access.GetMembersSupported);
            var members = access.GetMembers();
            Assert.Equal(16, members.Count);
            Assert.Equal("A", members[0].Name);
            Assert.Equal("B", members[1].Name);
            Assert.Equal("C", members[2].Name);
            Assert.Equal("D", members[3].Name);
            Assert.Equal("E", members[4].Name);
            Assert.Equal("F", members[5].Name);
            Assert.Equal("G", members[6].Name);
            Assert.Equal("H", members[7].Name);
            Assert.Equal("I", members[8].Name);
            Assert.Equal("J", members[9].Name);
            Assert.Equal("K", members[10].Name);
            Assert.Equal("L", members[11].Name);
            Assert.Equal("M", members[12].Name);
            Assert.Equal("N", members[13].Name);
            Assert.Equal("O", members[14].Name);
            Assert.Equal("P", members[15].Name);
        }

    }
}
