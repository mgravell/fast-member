using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FastMember.Tests
{
    public class ByRefProp
    {
        [Fact]
        public void CanGetByRef()
        {
            var foo = new Foo { Val = 42 };

            var acc = ObjectAccessor.Create(foo);
            Assert.Equal(42, (int)acc["Val"]);
            Assert.Equal(42, (int)acc["Ref"]);
            Assert.Equal(42, (int)acc["RefReadOnly"]);
        }

        [Fact]
        public void CanSetByRef()
        {
            var foo = new Foo { Val = 42 };
            var acc = ObjectAccessor.Create(foo);
            acc["Val"] = 43;
            Assert.Equal(43, foo.Val);

            acc["Ref"] = 44;
            Assert.Equal(44, foo.Val);

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                acc["RefReadOnly"] = 45;
            });
            Assert.Equal("name", ex.ParamName);
            Assert.Equal(44, foo.Val);
        }
        public class Foo
        {
            private int _val;
            public int Val
            {
                get => _val;
                set => _val = value;
            }
            public ref int Ref => ref _val;
            public ref readonly int RefReadOnly => ref _val;

        }
    }
}
