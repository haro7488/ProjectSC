using System.Collections.Generic;
using NUnit.Framework;
using Sc.Common;

namespace Sc.Editor.Tests.Common
{
    /// <summary>
    /// CollectionExtensions 단위 테스트
    /// </summary>
    [TestFixture]
    public class CollectionExtensionsTests
    {
        #region RandomPick Tests

        [Test]
        public void RandomPick_NullList_ReturnsDefault()
        {
            List<int> list = null;

            var result = list.RandomPick();

            Assert.That(result, Is.EqualTo(default(int)));
        }

        [Test]
        public void RandomPick_EmptyList_ReturnsDefault()
        {
            var list = new List<int>();

            var result = list.RandomPick();

            Assert.That(result, Is.EqualTo(default(int)));
        }

        [Test]
        public void RandomPick_SingleElement_ReturnsThatElement()
        {
            var list = new List<int> { 42 };

            var result = list.RandomPick();

            Assert.That(result, Is.EqualTo(42));
        }

        [Test]
        public void RandomPick_MultipleElements_ReturnsElementFromList()
        {
            var list = new List<int> { 1, 2, 3, 4, 5 };

            var result = list.RandomPick();

            Assert.That(list, Does.Contain(result));
        }

        #endregion

        #region WeightedRandomPick Tests

        [Test]
        public void WeightedRandomPick_NullList_ReturnsDefault()
        {
            List<string> list = null;

            var result = list.WeightedRandomPick(s => 1f);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void WeightedRandomPick_EmptyList_ReturnsDefault()
        {
            var list = new List<string>();

            var result = list.WeightedRandomPick(s => 1f);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void WeightedRandomPick_SingleElement_ReturnsThatElement()
        {
            var list = new List<string> { "only" };

            var result = list.WeightedRandomPick(s => 1f);

            Assert.That(result, Is.EqualTo("only"));
        }

        [Test]
        public void WeightedRandomPick_ReturnsElementFromList()
        {
            var list = new List<string> { "a", "b", "c" };

            var result = list.WeightedRandomPick(s => 1f);

            Assert.That(list, Does.Contain(result));
        }

        [Test]
        public void WeightedRandomPick_ZeroWeights_FallsBackToRandomPick()
        {
            var list = new List<int> { 1, 2, 3 };

            var result = list.WeightedRandomPick(_ => 0f);

            Assert.That(list, Does.Contain(result));
        }

        #endregion

        #region Shuffle Tests

        [Test]
        public void Shuffle_NullList_ReturnsNull()
        {
            List<int> list = null;

            var result = list.Shuffle();

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Shuffle_EmptyList_ReturnsEmptyList()
        {
            var list = new List<int>();

            var result = list.Shuffle();

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Shuffle_SingleElement_ReturnsListWithSameElement()
        {
            var list = new List<int> { 1 };

            var result = list.Shuffle();

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0], Is.EqualTo(1));
        }

        [Test]
        public void Shuffle_PreservesAllElements()
        {
            var list = new List<int> { 1, 2, 3, 4, 5 };
            var original = new List<int>(list);

            list.Shuffle();

            Assert.That(list, Has.Count.EqualTo(original.Count));
            foreach (var item in original)
            {
                Assert.That(list, Does.Contain(item));
            }
        }

        #endregion

        #region SafeGet Tests

        [Test]
        public void SafeGet_NullList_ReturnsDefault()
        {
            List<int> list = null;

            var result = list.SafeGet(0);

            Assert.That(result, Is.EqualTo(default(int)));
        }

        [Test]
        public void SafeGet_NegativeIndex_ReturnsDefault()
        {
            var list = new List<int> { 1, 2, 3 };

            var result = list.SafeGet(-1);

            Assert.That(result, Is.EqualTo(default(int)));
        }

        [Test]
        public void SafeGet_IndexOutOfRange_ReturnsDefault()
        {
            var list = new List<int> { 1, 2, 3 };

            var result = list.SafeGet(10);

            Assert.That(result, Is.EqualTo(default(int)));
        }

        [Test]
        public void SafeGet_IndexOutOfRange_ReturnsCustomDefault()
        {
            var list = new List<int> { 1, 2, 3 };

            var result = list.SafeGet(10, -999);

            Assert.That(result, Is.EqualTo(-999));
        }

        [Test]
        public void SafeGet_ValidIndex_ReturnsElement()
        {
            var list = new List<int> { 10, 20, 30 };

            var result = list.SafeGet(1);

            Assert.That(result, Is.EqualTo(20));
        }

        #endregion

        #region IsNullOrEmpty Tests

        [Test]
        public void IsNullOrEmpty_NullCollection_ReturnsTrue()
        {
            List<int> list = null;

            var result = list.IsNullOrEmpty();

            Assert.That(result, Is.True);
        }

        [Test]
        public void IsNullOrEmpty_EmptyCollection_ReturnsTrue()
        {
            var list = new List<int>();

            var result = list.IsNullOrEmpty();

            Assert.That(result, Is.True);
        }

        [Test]
        public void IsNullOrEmpty_CollectionWithElements_ReturnsFalse()
        {
            var list = new List<int> { 1 };

            var result = list.IsNullOrEmpty();

            Assert.That(result, Is.False);
        }

        #endregion

        #region RandomPickMultiple Tests

        [Test]
        public void RandomPickMultiple_NullList_ReturnsEmptyList()
        {
            List<int> list = null;

            var result = list.RandomPickMultiple(3);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void RandomPickMultiple_EmptyList_ReturnsEmptyList()
        {
            var list = new List<int>();

            var result = list.RandomPickMultiple(3);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void RandomPickMultiple_ZeroCount_ReturnsEmptyList()
        {
            var list = new List<int> { 1, 2, 3 };

            var result = list.RandomPickMultiple(0);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void RandomPickMultiple_CountLargerThanList_ReturnsAllElements()
        {
            var list = new List<int> { 1, 2, 3 };

            var result = list.RandomPickMultiple(10);

            Assert.That(result, Has.Count.EqualTo(3));
        }

        [Test]
        public void RandomPickMultiple_ValidCount_ReturnsUniqueElements()
        {
            var list = new List<int> { 1, 2, 3, 4, 5 };

            var result = list.RandomPickMultiple(3);

            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(result, Is.Unique);
            foreach (var item in result)
            {
                Assert.That(list, Does.Contain(item));
            }
        }

        #endregion

        #region GetOrDefault Tests

        [Test]
        public void GetOrDefault_NullDictionary_ReturnsDefault()
        {
            Dictionary<string, int> dict = null;

            var result = dict.GetOrDefault("key");

            Assert.That(result, Is.EqualTo(default(int)));
        }

        [Test]
        public void GetOrDefault_KeyNotFound_ReturnsDefault()
        {
            var dict = new Dictionary<string, int> { { "a", 1 } };

            var result = dict.GetOrDefault("b");

            Assert.That(result, Is.EqualTo(default(int)));
        }

        [Test]
        public void GetOrDefault_KeyNotFound_ReturnsCustomDefault()
        {
            var dict = new Dictionary<string, int> { { "a", 1 } };

            var result = dict.GetOrDefault("b", -1);

            Assert.That(result, Is.EqualTo(-1));
        }

        [Test]
        public void GetOrDefault_KeyFound_ReturnsValue()
        {
            var dict = new Dictionary<string, int> { { "a", 42 } };

            var result = dict.GetOrDefault("a");

            Assert.That(result, Is.EqualTo(42));
        }

        #endregion

        #region GetOrAdd Tests

        [Test]
        public void GetOrAdd_KeyNotFound_AddsAndReturnsNewValue()
        {
            var dict = new Dictionary<string, int>();

            var result = dict.GetOrAdd("key", () => 100);

            Assert.That(result, Is.EqualTo(100));
            Assert.That(dict["key"], Is.EqualTo(100));
        }

        [Test]
        public void GetOrAdd_KeyFound_ReturnsExistingValue()
        {
            var dict = new Dictionary<string, int> { { "key", 50 } };

            var result = dict.GetOrAdd("key", () => 100);

            Assert.That(result, Is.EqualTo(50));
            Assert.That(dict["key"], Is.EqualTo(50));
        }

        [Test]
        public void GetOrAdd_KeyFound_FactoryNotCalled()
        {
            var dict = new Dictionary<string, int> { { "key", 50 } };
            var factoryCalled = false;

            dict.GetOrAdd("key", () =>
            {
                factoryCalled = true;
                return 100;
            });

            Assert.That(factoryCalled, Is.False);
        }

        #endregion
    }
}