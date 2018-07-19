using FastMember;
using Xunit;

namespace FastMemberTests
{
    public class Anons
    {
        [Fact]
        public void TestAnonTypeAccess()
        {
            var obj = new {A = 123, B = "def"};

            var accessor = ObjectAccessor.Create(obj);
            Assert.Equal(123, accessor["A"]);
            Assert.Equal("def", accessor["B"]);
        }
        [Fact]
        public void TestAnonCtor()
        {
            var obj = new {A = 123, B = "def"};

            var accessor = TypeAccessor.Create(obj.GetType());
            Assert.False(accessor.CreateNewSupported);
        }

        [Fact]
        public void TestPrivateTypeAccess()
        {
            var obj = new Private { A = 123, B = "def" };

            var accessor = ObjectAccessor.Create(obj);
            Assert.Equal(123, accessor["A"]);
            Assert.Equal("def", accessor["B"]);
        }

        [Fact]
        public void TestPrivateTypeCtor()
        {
            var accessor = TypeAccessor.Create(typeof (Private));
            Assert.True(accessor.CreateNewSupported);
            object obj = accessor.CreateNew();
            Assert.NotNull(obj);
            Assert.IsType<Private>(obj);
        }

        private sealed class Private
        {
            public int A { get; set; }
            public string B { get; set; }
        }
    }


}
