using System.Collections.Generic;
using NUnit.Framework;
using Sc.Data;

namespace Sc.Editor.Tests.Data
{
    /// <summary>
    /// GachaHistoryRecord 단위 테스트.
    /// 히스토리 레코드 생성 및 카운트 계산 검증.
    /// </summary>
    [TestFixture]
    public class GachaHistoryRecordTests
    {
        #region Create Tests

        [Test]
        public void Create_GeneratesUniqueId()
        {
            var results = new List<GachaHistoryResultItem>();
            var record = GachaHistoryRecord.Create("pool_1", "테스트 풀", 1000, GachaPullType.Single, results);

            Assert.That(record.Id, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void Create_SetsPoolIdCorrectly()
        {
            var results = new List<GachaHistoryResultItem>();
            var record = GachaHistoryRecord.Create("pool_test", "테스트 풀", 1000, GachaPullType.Single, results);

            Assert.That(record.PoolId, Is.EqualTo("pool_test"));
        }

        [Test]
        public void Create_SetsPoolNameCorrectly()
        {
            var results = new List<GachaHistoryResultItem>();
            var record = GachaHistoryRecord.Create("pool_1", "아리아 픽업", 1000, GachaPullType.Single, results);

            Assert.That(record.PoolName, Is.EqualTo("아리아 픽업"));
        }

        [Test]
        public void Create_SetsTimestampCorrectly()
        {
            var results = new List<GachaHistoryResultItem>();
            var record = GachaHistoryRecord.Create("pool_1", "테스트 풀", 12345678, GachaPullType.Single, results);

            Assert.That(record.Timestamp, Is.EqualTo(12345678));
        }

        [Test]
        public void Create_SetsPullTypeCorrectly_Single()
        {
            var results = new List<GachaHistoryResultItem>();
            var record = GachaHistoryRecord.Create("pool_1", "테스트 풀", 1000, GachaPullType.Single, results);

            Assert.That(record.PullType, Is.EqualTo(GachaPullType.Single));
        }

        [Test]
        public void Create_SetsPullTypeCorrectly_Multi()
        {
            var results = new List<GachaHistoryResultItem>();
            var record = GachaHistoryRecord.Create("pool_1", "테스트 풀", 1000, GachaPullType.Multi, results);

            Assert.That(record.PullType, Is.EqualTo(GachaPullType.Multi));
        }

        #endregion

        #region Count Calculation Tests

        [Test]
        public void Create_CalculatesSSRCount_Correctly()
        {
            var results = new List<GachaHistoryResultItem>
            {
                new GachaHistoryResultItem { CharacterId = "char_001", Rarity = Rarity.SSR },
                new GachaHistoryResultItem { CharacterId = "char_002", Rarity = Rarity.SR },
                new GachaHistoryResultItem { CharacterId = "char_003", Rarity = Rarity.SSR },
                new GachaHistoryResultItem { CharacterId = "char_004", Rarity = Rarity.R }
            };

            var record = GachaHistoryRecord.Create("pool_1", "테스트 풀", 1000, GachaPullType.Multi, results);

            Assert.That(record.SSRCount, Is.EqualTo(2));
        }

        [Test]
        public void Create_CalculatesSRCount_Correctly()
        {
            var results = new List<GachaHistoryResultItem>
            {
                new GachaHistoryResultItem { CharacterId = "char_001", Rarity = Rarity.SR },
                new GachaHistoryResultItem { CharacterId = "char_002", Rarity = Rarity.SR },
                new GachaHistoryResultItem { CharacterId = "char_003", Rarity = Rarity.SR },
                new GachaHistoryResultItem { CharacterId = "char_004", Rarity = Rarity.R }
            };

            var record = GachaHistoryRecord.Create("pool_1", "테스트 풀", 1000, GachaPullType.Multi, results);

            Assert.That(record.SRCount, Is.EqualTo(3));
        }

        [Test]
        public void Create_CalculatesRCount_Correctly()
        {
            var results = new List<GachaHistoryResultItem>
            {
                new GachaHistoryResultItem { CharacterId = "char_001", Rarity = Rarity.R },
                new GachaHistoryResultItem { CharacterId = "char_002", Rarity = Rarity.R },
                new GachaHistoryResultItem { CharacterId = "char_003", Rarity = Rarity.R },
                new GachaHistoryResultItem { CharacterId = "char_004", Rarity = Rarity.R },
                new GachaHistoryResultItem { CharacterId = "char_005", Rarity = Rarity.R }
            };

            var record = GachaHistoryRecord.Create("pool_1", "테스트 풀", 1000, GachaPullType.Multi, results);

            Assert.That(record.RCount, Is.EqualTo(5));
        }

        [Test]
        public void Create_HandlesEmptyResults()
        {
            var results = new List<GachaHistoryResultItem>();
            var record = GachaHistoryRecord.Create("pool_1", "테스트 풀", 1000, GachaPullType.Single, results);

            Assert.That(record.SSRCount, Is.EqualTo(0));
            Assert.That(record.SRCount, Is.EqualTo(0));
            Assert.That(record.RCount, Is.EqualTo(0));
        }

        [Test]
        public void Create_HandlesNullResults()
        {
            var record = GachaHistoryRecord.Create("pool_1", "테스트 풀", 1000, GachaPullType.Single, null);

            Assert.That(record.SSRCount, Is.EqualTo(0));
            Assert.That(record.SRCount, Is.EqualTo(0));
            Assert.That(record.RCount, Is.EqualTo(0));
        }

        [Test]
        public void Create_CalculatesTypical10Pull_Correctly()
        {
            // 일반적인 10연차: SSR 0, SR 1, R 9
            var results = new List<GachaHistoryResultItem>
            {
                new GachaHistoryResultItem { CharacterId = "char_001", Rarity = Rarity.SR },
                new GachaHistoryResultItem { CharacterId = "char_002", Rarity = Rarity.R },
                new GachaHistoryResultItem { CharacterId = "char_003", Rarity = Rarity.R },
                new GachaHistoryResultItem { CharacterId = "char_004", Rarity = Rarity.R },
                new GachaHistoryResultItem { CharacterId = "char_005", Rarity = Rarity.R },
                new GachaHistoryResultItem { CharacterId = "char_006", Rarity = Rarity.R },
                new GachaHistoryResultItem { CharacterId = "char_007", Rarity = Rarity.R },
                new GachaHistoryResultItem { CharacterId = "char_008", Rarity = Rarity.R },
                new GachaHistoryResultItem { CharacterId = "char_009", Rarity = Rarity.R },
                new GachaHistoryResultItem { CharacterId = "char_010", Rarity = Rarity.R }
            };

            var record = GachaHistoryRecord.Create("pool_1", "테스트 풀", 1000, GachaPullType.Multi, results);

            Assert.That(record.SSRCount, Is.EqualTo(0));
            Assert.That(record.SRCount, Is.EqualTo(1));
            Assert.That(record.RCount, Is.EqualTo(9));
            Assert.That(record.Results.Count, Is.EqualTo(10));
        }

        #endregion

        #region GachaHistoryResultItem Tests

        [Test]
        public void GachaHistoryResultItem_StoresIsNew_Correctly()
        {
            var item = new GachaHistoryResultItem
            {
                CharacterId = "char_001",
                Rarity = Rarity.SSR,
                IsNew = true,
                IsPity = false
            };

            Assert.That(item.IsNew, Is.True);
        }

        [Test]
        public void GachaHistoryResultItem_StoresIsPity_Correctly()
        {
            var item = new GachaHistoryResultItem
            {
                CharacterId = "char_001",
                Rarity = Rarity.SSR,
                IsNew = false,
                IsPity = true
            };

            Assert.That(item.IsPity, Is.True);
        }

        #endregion
    }
}
