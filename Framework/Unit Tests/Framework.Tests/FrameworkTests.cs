using Framework.Utils.Extensions.String;
using NUnit.Framework;

namespace Framework.Tests
{
    [TestFixture]
    public class FrameworkTests
    {
        [Test]
        public void TestAlphaCheckFails()
        {
            var alphaString = "abcd123";
            Assert.IsFalse(alphaString.IsAlpha());
        }

        [Test]
        public void TestAlphaCheckPasses()
        {
            var alphaString = "abcd";
            Assert.IsTrue(alphaString.IsAlpha());
        }

        [Test]
        public void TestAlphaNumericCheckFails()
        {
            var alphaString = "abcd123~~";
            Assert.IsFalse(alphaString.IsAlphaNumeric());
        }

        [Test]
        public void TestAlphaNumericCheckPasses()
        {
            var alphaString = "abcd123";
            Assert.IsTrue(alphaString.IsAlphaNumeric());
        }

        [Test]
        public void TestNumericCheckFails()
        {
            var alphaString = "abcd123";
            Assert.IsFalse(alphaString.IsNumeric());
        }

        [Test]
        public void TestNumericCheckPasses()
        {
            var alphaString = "1234";
            Assert.IsTrue(alphaString.IsNumeric());
        }
    }
}