using System;

namespace Sc.Common
{
    /// <summary>
    /// 수학/확률 유틸리티
    /// </summary>
    public static class MathHelper
    {
        private static readonly Random _random = new();

        #region 확률/랜덤

        /// <summary>
        /// 확률 판정 (0.0~1.0)
        /// </summary>
        /// <param name="probability">확률 (0.0 = 0%, 1.0 = 100%)</param>
        /// <returns>확률 충족 시 true</returns>
        public static bool CheckProbability(float probability)
        {
            if (probability <= 0f) return false;
            if (probability >= 1f) return true;

            return (float)_random.NextDouble() < probability;
        }

        /// <summary>
        /// 정수 범위 랜덤 (min 포함, max 제외)
        /// </summary>
        /// <param name="min">최소값 (포함)</param>
        /// <param name="max">최대값 (제외)</param>
        /// <returns>랜덤 정수</returns>
        public static int RandomRange(int min, int max)
        {
            if (min >= max) return min;
            return _random.Next(min, max);
        }

        /// <summary>
        /// 실수 범위 랜덤
        /// </summary>
        /// <param name="min">최소값 (포함)</param>
        /// <param name="max">최대값 (포함)</param>
        /// <returns>랜덤 실수</returns>
        public static float RandomRange(float min, float max)
        {
            if (min >= max) return min;
            return min + (float)_random.NextDouble() * (max - min);
        }

        #endregion

        #region 값 변환

        /// <summary>
        /// 값을 범위 내로 제한
        /// </summary>
        /// <typeparam name="T">IComparable 타입</typeparam>
        /// <param name="value">값</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        /// <returns>제한된 값</returns>
        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0) return min;
            if (value.CompareTo(max) > 0) return max;
            return value;
        }

        /// <summary>
        /// 선형 보간
        /// </summary>
        /// <param name="a">시작값</param>
        /// <param name="b">끝값</param>
        /// <param name="t">보간 비율 (0.0~1.0)</param>
        /// <returns>보간된 값</returns>
        public static float Lerp(float a, float b, float t)
        {
            t = Clamp(t, 0f, 1f);
            return a + (b - a) * t;
        }

        /// <summary>
        /// 역선형 보간 (값의 비율 계산)
        /// </summary>
        /// <param name="a">시작값</param>
        /// <param name="b">끝값</param>
        /// <param name="value">현재값</param>
        /// <returns>비율 (0.0~1.0)</returns>
        public static float InverseLerp(float a, float b, float value)
        {
            if (Math.Abs(b - a) < float.Epsilon)
                return 0f;

            return Clamp((value - a) / (b - a), 0f, 1f);
        }

        /// <summary>
        /// 범위 재매핑
        /// </summary>
        /// <param name="value">현재값</param>
        /// <param name="fromMin">원본 최소값</param>
        /// <param name="fromMax">원본 최대값</param>
        /// <param name="toMin">대상 최소값</param>
        /// <param name="toMax">대상 최대값</param>
        /// <returns>재매핑된 값</returns>
        public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            float t = InverseLerp(fromMin, fromMax, value);
            return Lerp(toMin, toMax, t);
        }

        /// <summary>
        /// 퍼센트를 소수로 변환 (예: 50 -> 0.5)
        /// </summary>
        /// <param name="percent">퍼센트 값</param>
        /// <returns>소수 값</returns>
        public static float PercentToDecimal(float percent)
        {
            return percent / 100f;
        }

        #endregion
    }
}