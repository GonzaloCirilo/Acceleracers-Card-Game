using NUnit.Framework;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Tests
{
    [TestFixture]
    public class SPPTests
    {
        [Test]
        public void Addition_CombinesTwoSPPs()
        {
            var a = new SPP(1, 2, 3);
            var b = new SPP(4, 5, 6);
            var result = a + b;
            Assert.AreEqual(new SPP(5, 7, 9), result);
        }

        [Test]
        public void Subtraction_SubtractsTwoSPPs()
        {
            var a = new SPP(5, 7, 9);
            var b = new SPP(1, 2, 3);
            var result = a - b;
            Assert.AreEqual(new SPP(4, 5, 6), result);
        }

        [Test]
        public void Negation_NegatesAllValues()
        {
            var a = new SPP(1, 2, 3);
            var result = -a;
            Assert.AreEqual(new SPP(-1, -2, -3), result);
        }

        [Test]
        public void Zero_IsAllZeroes()
        {
            Assert.AreEqual(new SPP(0, 0, 0), SPP.Zero);
        }

        [Test]
        public void GetCategory_ReturnsCorrectValue()
        {
            var spp = new SPP(1, 2, 3);
            Assert.AreEqual(1, spp.GetCategory(SPPCategory.Speed));
            Assert.AreEqual(2, spp.GetCategory(SPPCategory.Power));
            Assert.AreEqual(3, spp.GetCategory(SPPCategory.Performance));
        }

        [Test]
        public void MeetsEscape_WhenMeetsOrExceeds_ReturnsTrue()
        {
            var spp = new SPP(5, 3, 7);
            Assert.IsTrue(spp.MeetsEscape(5, SPPCategory.Speed));
            Assert.IsTrue(spp.MeetsEscape(3, SPPCategory.Power));
            Assert.IsTrue(spp.MeetsEscape(6, SPPCategory.Performance));
        }

        [Test]
        public void MeetsEscape_WhenBelow_ReturnsFalse()
        {
            var spp = new SPP(5, 3, 7);
            Assert.IsFalse(spp.MeetsEscape(6, SPPCategory.Speed));
            Assert.IsFalse(spp.MeetsEscape(4, SPPCategory.Power));
        }

        [Test]
        public void AnyZeroOrBelow_WithZero_ReturnsTrue()
        {
            Assert.IsTrue(new SPP(0, 1, 1).AnyZeroOrBelow());
            Assert.IsTrue(new SPP(1, 0, 1).AnyZeroOrBelow());
            Assert.IsTrue(new SPP(1, 1, 0).AnyZeroOrBelow());
        }

        [Test]
        public void AnyZeroOrBelow_WithNegative_ReturnsTrue()
        {
            Assert.IsTrue(new SPP(-1, 5, 5).AnyZeroOrBelow());
        }

        [Test]
        public void AnyZeroOrBelow_AllPositive_ReturnsFalse()
        {
            Assert.IsFalse(new SPP(1, 1, 1).AnyZeroOrBelow());
        }

        [Test]
        public void Equality_SameValues_AreEqual()
        {
            var a = new SPP(1, 2, 3);
            var b = new SPP(1, 2, 3);
            Assert.AreEqual(a, b);
            Assert.IsTrue(a == b);
        }

        [Test]
        public void Equality_DifferentValues_AreNotEqual()
        {
            var a = new SPP(1, 2, 3);
            var b = new SPP(1, 2, 4);
            Assert.AreNotEqual(a, b);
            Assert.IsTrue(a != b);
        }

        [Test]
        public void HasWindow_PositiveValue_ReturnsTrue()
        {
            var spp = new SPP(3, 0, 2);
            Assert.IsTrue(spp.HasWindow(SPPCategory.Speed));
            Assert.IsFalse(spp.HasWindow(SPPCategory.Power));
            Assert.IsTrue(spp.HasWindow(SPPCategory.Performance));
        }

        [Test]
        public void ToString_FormatsCorrectly()
        {
            var spp = new SPP(1, 2, 3);
            Assert.AreEqual("SPP(1/2/3)", spp.ToString());
        }
    }
}
