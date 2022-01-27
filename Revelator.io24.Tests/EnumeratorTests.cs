using NUnit.Framework;
using Revelator.io24.Api;

namespace Revelator.io24.Tests
{
    internal class EnumeratorTests
    {
        [Test]
        public void Test1()
        {
            var kaka = "5543000106004b41680066005543000106004b4168006600";
            var enumerator = new EnumeratePackage(kaka);

            var header1 = enumerator.GetNext();
            Assert.NotNull(header1);
            Assert.False(enumerator.IsEnumerated());

            var header2 = enumerator.GetNext();
            Assert.NotNull(header2);
            Assert.True(enumerator.IsEnumerated());

            var header3 = enumerator.GetNext();
            Assert.Null(header3);
        }
    }
}
