using System;
using System.Collections.Generic;

namespace Sc.Common
{
    /// <summary>
    /// 컬렉션 확장 메서드
    /// </summary>
    public static class CollectionExtensions
    {
        private static readonly Random _random = new();

        /// <summary>
        /// 컬렉션에서 랜덤 요소 반환
        /// </summary>
        /// <typeparam name="T">요소 타입</typeparam>
        /// <param name="list">대상 컬렉션</param>
        /// <returns>랜덤 요소, 빈 컬렉션이면 default</returns>
        public static T RandomPick<T>(this IList<T> list)
        {
            if (list == null || list.Count == 0)
                return default;

            return list[_random.Next(list.Count)];
        }

        /// <summary>
        /// 가중치 기반 랜덤 요소 반환
        /// </summary>
        /// <typeparam name="T">요소 타입</typeparam>
        /// <param name="list">대상 컬렉션</param>
        /// <param name="weightSelector">가중치 반환 함수</param>
        /// <returns>가중치 랜덤 요소, 빈 컬렉션이면 default</returns>
        public static T WeightedRandomPick<T>(this IList<T> list, Func<T, float> weightSelector)
        {
            if (list == null || list.Count == 0)
                return default;

            if (weightSelector == null)
                throw new ArgumentNullException(nameof(weightSelector));

            float totalWeight = 0f;
            foreach (var item in list)
            {
                totalWeight += weightSelector(item);
            }

            if (totalWeight <= 0f)
                return list.RandomPick();

            float randomValue = (float)_random.NextDouble() * totalWeight;
            float cumulative = 0f;

            foreach (var item in list)
            {
                cumulative += weightSelector(item);
                if (randomValue < cumulative)
                    return item;
            }

            return list[list.Count - 1];
        }

        /// <summary>
        /// Fisher-Yates 셔플
        /// </summary>
        /// <typeparam name="T">요소 타입</typeparam>
        /// <param name="list">대상 컬렉션</param>
        /// <returns>셔플된 컬렉션 (원본 수정)</returns>
        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            if (list == null || list.Count <= 1)
                return list;

            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }

            return list;
        }

        /// <summary>
        /// 안전한 인덱스 접근
        /// </summary>
        /// <typeparam name="T">요소 타입</typeparam>
        /// <param name="list">대상 컬렉션</param>
        /// <param name="index">인덱스</param>
        /// <param name="defaultValue">기본값</param>
        /// <returns>요소 또는 기본값</returns>
        public static T SafeGet<T>(this IList<T> list, int index, T defaultValue = default)
        {
            if (list == null || index < 0 || index >= list.Count)
                return defaultValue;

            return list[index];
        }

        /// <summary>
        /// null 또는 빈 컬렉션 확인
        /// </summary>
        /// <typeparam name="T">요소 타입</typeparam>
        /// <param name="collection">대상 컬렉션</param>
        /// <returns>null이거나 빈 경우 true</returns>
        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            return collection == null || collection.Count == 0;
        }

        /// <summary>
        /// 중복 없이 N개 랜덤 선택
        /// </summary>
        /// <typeparam name="T">요소 타입</typeparam>
        /// <param name="list">대상 컬렉션</param>
        /// <param name="count">선택 개수</param>
        /// <returns>선택된 요소 리스트</returns>
        public static List<T> RandomPickMultiple<T>(this IList<T> list, int count)
        {
            if (list == null || list.Count == 0 || count <= 0)
                return new List<T>();

            count = Math.Min(count, list.Count);

            var copy = new List<T>(list);
            copy.Shuffle();

            return copy.GetRange(0, count);
        }

        /// <summary>
        /// Dictionary 안전 접근
        /// </summary>
        /// <typeparam name="TKey">키 타입</typeparam>
        /// <typeparam name="TValue">값 타입</typeparam>
        /// <param name="dictionary">대상 Dictionary</param>
        /// <param name="key">키</param>
        /// <param name="defaultValue">기본값</param>
        /// <returns>값 또는 기본값</returns>
        public static TValue GetOrDefault<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key,
            TValue defaultValue = default)
        {
            if (dictionary == null || key == null)
                return defaultValue;

            return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
        }

        /// <summary>
        /// Dictionary에서 값 가져오기 (없으면 생성 후 반환)
        /// </summary>
        /// <typeparam name="TKey">키 타입</typeparam>
        /// <typeparam name="TValue">값 타입</typeparam>
        /// <param name="dictionary">대상 Dictionary</param>
        /// <param name="key">키</param>
        /// <param name="factory">값 생성 함수</param>
        /// <returns>기존 값 또는 새로 생성된 값</returns>
        public static TValue GetOrAdd<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key,
            Func<TValue> factory)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            if (!dictionary.TryGetValue(key, out var value))
            {
                value = factory();
                dictionary[key] = value;
            }

            return value;
        }
    }
}