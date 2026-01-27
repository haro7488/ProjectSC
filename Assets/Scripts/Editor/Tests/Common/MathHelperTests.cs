using System;
using NUnit.Framework;
using Sc.Common;

namespace Sc.Editor.Tests.Common
{
    /// <summary>
    /// MathHelper 단위 테스트
    /// </summary>
    [TestFixture]
    public class MathHelperTests
    {
        #region CheckProbability Tests

        [Test]
        public void CheckProbability_ZeroProbability_ReturnsFalse()
        {
            var result = MathHelper.CheckProbability(0f);

            Assert.That(result, Is.False);
        }

        [Test]
        public void CheckProbability_NegativeProbability_ReturnsFalse()
        {
            var result = MathHelper.CheckProbability(-0.5f);

            Assert.That(result, Is.False);
        }

        [Test]
        public void CheckProbability_OneProbability_ReturnsTrue()
        {
            var result = MathHelper.CheckProbability(1f);

            Assert.That(result, Is.True);
        }

        [Test]
        public void CheckProbability_GreaterThanOne_ReturnsTrue()
        {
            var result = MathHelper.CheckProbability(1.5f);

            Assert.That(result, Is.True);
        }

        [Test]
        public void CheckProbability_ValidProbability_ReturnsBool()
        {
            // 50% 확률로 여러번 실행하면 true/false 섞여야 함
            int trueCount = 0;
            int iterations = 1000;

            for (int i = 0; i < iterations; i++)
            {
                if (MathHelper.CheckProbability(0.5f))
                    trueCount++;
            }

            // 대략 30%~70% 범위 내에 있어야 함 (통계적으로)
            Assert.That(trueCount, Is.GreaterThan(iterations * 0.3));
            Assert.That(trueCount, Is.LessThan(iterations * 0.7));
        }

        #endregion

        #region RandomRange Tests

        [Test]
        public void RandomRange_Int_MinEqualsMax_ReturnsMin()
        {
            var result = MathHelper.RandomRange(5, 5);

            Assert.That(result, Is.EqualTo(5));
        }

        [Test]
        public void RandomRange_Int_MinGreaterThanMax_ReturnsMin()
        {
            var result = MathHelper.RandomRange(10, 5);

            Assert.That(result, Is.EqualTo(10));
        }

        [Test]
        public void RandomRange_Int_ReturnsValueInRange()
        {
            for (int i = 0; i < 100; i++)
            {
                var result = MathHelper.RandomRange(1, 10);

                Assert.That(result, Is.GreaterThanOrEqualTo(1));
                Assert.That(result, Is.LessThan(10)); // max 제외
            }
        }

        [Test]
        public void RandomRange_Float_MinEqualsMax_ReturnsMin()
        {
            var result = MathHelper.RandomRange(5f, 5f);

            Assert.That(result, Is.EqualTo(5f));
        }

        [Test]
        public void RandomRange_Float_ReturnsValueInRange()
        {
            for (int i = 0; i < 100; i++)
            {
                var result = MathHelper.RandomRange(0f, 1f);

                Assert.That(result, Is.GreaterThanOrEqualTo(0f));
                Assert.That(result, Is.LessThanOrEqualTo(1f));
            }
        }

        #endregion

        #region Clamp Tests

        [Test]
        public void Clamp_ValueBelowMin_ReturnsMin()
        {
            var result = MathHelper.Clamp(5, 10, 20);

            Assert.That(result, Is.EqualTo(10));
        }

        [Test]
        public void Clamp_ValueAboveMax_ReturnsMax()
        {
            var result = MathHelper.Clamp(25, 10, 20);

            Assert.That(result, Is.EqualTo(20));
        }

        [Test]
        public void Clamp_ValueInRange_ReturnsValue()
        {
            var result = MathHelper.Clamp(15, 10, 20);

            Assert.That(result, Is.EqualTo(15));
        }

        [Test]
        public void Clamp_Float_Works()
        {
            var result = MathHelper.Clamp(0.5f, 0f, 1f);

            Assert.That(result, Is.EqualTo(0.5f));
        }

        #endregion

        #region Lerp Tests

        [Test]
        public void Lerp_TZero_ReturnsA()
        {
            var result = MathHelper.Lerp(0f, 100f, 0f);

            Assert.That(result, Is.EqualTo(0f));
        }

        [Test]
        public void Lerp_TOne_ReturnsB()
        {
            var result = MathHelper.Lerp(0f, 100f, 1f);

            Assert.That(result, Is.EqualTo(100f));
        }

        [Test]
        public void Lerp_THalf_ReturnsMidpoint()
        {
            var result = MathHelper.Lerp(0f, 100f, 0.5f);

            Assert.That(result, Is.EqualTo(50f));
        }

        [Test]
        public void Lerp_TClamped_WhenAboveOne()
        {
            var result = MathHelper.Lerp(0f, 100f, 1.5f);

            Assert.That(result, Is.EqualTo(100f));
        }

        [Test]
        public void Lerp_TClamped_WhenBelowZero()
        {
            var result = MathHelper.Lerp(0f, 100f, -0.5f);

            Assert.That(result, Is.EqualTo(0f));
        }

        #endregion

        #region InverseLerp Tests

        [Test]
        public void InverseLerp_ValueEqualsA_ReturnsZero()
        {
            var result = MathHelper.InverseLerp(0f, 100f, 0f);

            Assert.That(result, Is.EqualTo(0f));
        }

        [Test]
        public void InverseLerp_ValueEqualsB_ReturnsOne()
        {
            var result = MathHelper.InverseLerp(0f, 100f, 100f);

            Assert.That(result, Is.EqualTo(1f));
        }

        [Test]
        public void InverseLerp_ValueMidpoint_ReturnsHalf()
        {
            var result = MathHelper.InverseLerp(0f, 100f, 50f);

            Assert.That(result, Is.EqualTo(0.5f));
        }

        [Test]
        public void InverseLerp_AEqualsB_ReturnsZero()
        {
            var result = MathHelper.InverseLerp(50f, 50f, 50f);

            Assert.That(result, Is.EqualTo(0f));
        }

        [Test]
        public void InverseLerp_ValueClamped_WhenAboveB()
        {
            var result = MathHelper.InverseLerp(0f, 100f, 150f);

            Assert.That(result, Is.EqualTo(1f));
        }

        #endregion

        #region Remap Tests

        [Test]
        public void Remap_MidpointValue_ReturnsMappedMidpoint()
        {
            // 0-100 범위의 50을 0-1 범위로 매핑하면 0.5
            var result = MathHelper.Remap(50f, 0f, 100f, 0f, 1f);

            Assert.That(result, Is.EqualTo(0.5f).Within(0.001f));
        }

        [Test]
        public void Remap_MinValue_ReturnsMappedMin()
        {
            var result = MathHelper.Remap(0f, 0f, 100f, 10f, 20f);

            Assert.That(result, Is.EqualTo(10f));
        }

        [Test]
        public void Remap_MaxValue_ReturnsMappedMax()
        {
            var result = MathHelper.Remap(100f, 0f, 100f, 10f, 20f);

            Assert.That(result, Is.EqualTo(20f));
        }

        #endregion

        #region PercentToDecimal Tests

        [Test]
        public void PercentToDecimal_Fifty_ReturnsHalf()
        {
            var result = MathHelper.PercentToDecimal(50f);

            Assert.That(result, Is.EqualTo(0.5f));
        }

        [Test]
        public void PercentToDecimal_Hundred_ReturnsOne()
        {
            var result = MathHelper.PercentToDecimal(100f);

            Assert.That(result, Is.EqualTo(1f));
        }

        [Test]
        public void PercentToDecimal_Zero_ReturnsZero()
        {
            var result = MathHelper.PercentToDecimal(0f);

            Assert.That(result, Is.EqualTo(0f));
        }

        #endregion
    }
}