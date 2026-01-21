using System.Collections.Generic;
using NUnit.Framework;
using Sc.Data;
using Sc.LocalServer;
using UnityEngine;

namespace Sc.Editor.Tests.LocalServer
{
    /// <summary>
    /// CharacterAscensionHandler 단위 테스트.
    /// 캐릭터 돌파 로직 검증.
    /// </summary>
    [TestFixture]
    public class CharacterAscensionHandlerTests
    {
        private CharacterAscensionHandler _handler;
        private CharacterDatabase _characterDb;
        private CharacterLevelDatabase _levelDb;
        private CharacterAscensionDatabase _ascensionDb;
        private ItemDatabase _itemDb;
        private UserSaveData _testUserData;
        private CharacterData _testCharacterData;
        private ItemData _ascensionMaterial;

        private const string TEST_CHARACTER_ID = "char_test_001";
        private const string TEST_ASCENSION_MATERIAL_ID = "item_asc_001";

        [SetUp]
        public void SetUp()
        {
            var timeService = new ServerTimeService();
            var validator = new ServerValidator(timeService);
            var rewardService = new RewardService();

            _handler = new CharacterAscensionHandler(validator, rewardService);

            // 데이터베이스 생성
            _characterDb = ScriptableObject.CreateInstance<CharacterDatabase>();
            _levelDb = ScriptableObject.CreateInstance<CharacterLevelDatabase>();
            _ascensionDb = ScriptableObject.CreateInstance<CharacterAscensionDatabase>();
            _itemDb = ScriptableObject.CreateInstance<ItemDatabase>();

            // 테스트용 캐릭터 데이터 생성
            _testCharacterData = ScriptableObject.CreateInstance<CharacterData>();
            _testCharacterData.Initialize(
                id: TEST_CHARACTER_ID,
                name: "테스트 캐릭터",
                nameEn: "Test Character",
                rarity: Rarity.SR,
                characterClass: CharacterClass.Warrior,
                element: Element.Fire,
                baseHp: 1000,
                baseAtk: 100,
                baseDef: 50,
                baseSpd: 100,
                critRate: 0.05f,
                critDamage: 1.5f,
                skillIds: new string[0],
                description: "테스트용 캐릭터"
            );
            _characterDb.Add(_testCharacterData);

            // 테스트용 레벨 데이터 생성
            var levelData = new List<CharacterLevelDatabase.RarityLevelData>
            {
                new CharacterLevelDatabase.RarityLevelData
                {
                    Rarity = Rarity.SR,
                    Requirements = new List<LevelRequirement>
                    {
                        new LevelRequirement(2, 100, 100),
                        new LevelRequirement(5, 1000, 400),
                        new LevelRequirement(10, 5000, 1000),
                        new LevelRequirement(20, 20000, 2000),
                        new LevelRequirement(30, 50000, 5000),
                        new LevelRequirement(40, 100000, 10000),
                    }
                }
            };
            _levelDb.SetData(levelData, 20);

            // 테스트용 돌파 데이터 생성
            var ascensionData = new List<CharacterAscensionDatabase.RarityAscensionData>
            {
                new CharacterAscensionDatabase.RarityAscensionData
                {
                    Rarity = Rarity.SR,
                    Requirements = new List<AscensionRequirement>
                    {
                        new AscensionRequirement
                        {
                            AscensionLevel = 0,
                            RequiredCharacterLevel = 20,
                            Materials = new List<RewardInfo> { RewardInfo.Item(TEST_ASCENSION_MATERIAL_ID, 5) },
                            GoldCost = 10000,
                            StatBonus = new CharacterStats(100, 10, 5, 5, 0.01f, 0.05f),
                            LevelCapIncrease = 10
                        },
                        new AscensionRequirement
                        {
                            AscensionLevel = 1,
                            RequiredCharacterLevel = 30,
                            Materials = new List<RewardInfo> { RewardInfo.Item(TEST_ASCENSION_MATERIAL_ID, 10) },
                            GoldCost = 20000,
                            StatBonus = new CharacterStats(200, 20, 10, 10, 0.02f, 0.1f),
                            LevelCapIncrease = 10
                        }
                    }
                }
            };
            _ascensionDb.SetData(ascensionData);

            // 테스트용 돌파 재료 생성
            _ascensionMaterial = ScriptableObject.CreateInstance<ItemData>();
            _ascensionMaterial.Initialize(
                id: TEST_ASCENSION_MATERIAL_ID,
                name: "돌파 재료",
                nameEn: "Ascension Material",
                type: ItemType.Material,
                rarity: Rarity.R,
                atkBonus: 0,
                defBonus: 0,
                hpBonus: 0,
                description: "돌파에 필요한 재료",
                maxStackCount: 999
            );
            _itemDb.Add(_ascensionMaterial);

            _handler.SetDatabases(_characterDb, _levelDb, _ascensionDb, _itemDb);

            // 테스트 유저 데이터 (돌파 조건 충족 상태)
            _testUserData = UserSaveData.CreateNew("test_uid", "TestUser");
            _testUserData.Currency = new UserCurrency { Gold = 100000, Gem = 1000, FreeGem = 500 };
            _testUserData.Characters = new List<OwnedCharacter>
            {
                new OwnedCharacter
                {
                    InstanceId = "inst_001",
                    CharacterId = TEST_CHARACTER_ID,
                    Level = 20,
                    Exp = 20000,
                    Ascension = 0
                }
            };
            _testUserData.Items = new List<OwnedItem>
            {
                new OwnedItem
                {
                    ItemId = TEST_ASCENSION_MATERIAL_ID,
                    Count = 100
                }
            };
        }

        [TearDown]
        public void TearDown()
        {
            if (_characterDb != null) Object.DestroyImmediate(_characterDb);
            if (_levelDb != null) Object.DestroyImmediate(_levelDb);
            if (_ascensionDb != null) Object.DestroyImmediate(_ascensionDb);
            if (_itemDb != null) Object.DestroyImmediate(_itemDb);
            if (_testCharacterData != null) Object.DestroyImmediate(_testCharacterData);
            if (_ascensionMaterial != null) Object.DestroyImmediate(_ascensionMaterial);
        }

        #region Basic Ascension Tests

        [Test]
        public void Handle_ReturnsSuccess_WhenAscensionValid()
        {
            var request = CharacterAscensionRequest.Create("inst_001");

            var response = _handler.Handle(request, ref _testUserData);

            Assert.That(response.IsSuccess, Is.True);
            Assert.That(response.PreviousAscension, Is.EqualTo(0));
            Assert.That(response.NewAscension, Is.EqualTo(1));
        }

        [Test]
        public void Handle_IncreasesLevelCap_WhenAscensionValid()
        {
            var request = CharacterAscensionRequest.Create("inst_001");

            var response = _handler.Handle(request, ref _testUserData);

            Assert.That(response.IsSuccess, Is.True);
            Assert.That(response.PreviousLevelCap, Is.EqualTo(20));
            Assert.That(response.NewLevelCap, Is.EqualTo(30)); // +10
        }

        [Test]
        public void Handle_DeductsMaterials_WhenAscensionValid()
        {
            var initialCount = _testUserData.FindItemById(TEST_ASCENSION_MATERIAL_ID)?.Count ?? 0;
            var request = CharacterAscensionRequest.Create("inst_001");

            _handler.Handle(request, ref _testUserData);

            var currentCount = _testUserData.FindItemById(TEST_ASCENSION_MATERIAL_ID)?.Count ?? 0;
            Assert.That(currentCount, Is.EqualTo(initialCount - 5)); // 1차 돌파 재료 5개
        }

        [Test]
        public void Handle_DeductsGold_WhenAscensionValid()
        {
            var initialGold = _testUserData.Currency.Gold;
            var request = CharacterAscensionRequest.Create("inst_001");

            _handler.Handle(request, ref _testUserData);

            Assert.That(_testUserData.Currency.Gold, Is.EqualTo(initialGold - 10000)); // 1차 돌파 골드 비용
        }

        [Test]
        public void Handle_UpdatesCharacterData_WhenAscensionValid()
        {
            var request = CharacterAscensionRequest.Create("inst_001");

            _handler.Handle(request, ref _testUserData);

            var character = _testUserData.FindCharacterByInstanceId("inst_001");
            Assert.That(character.HasValue, Is.True);
            Assert.That(character.Value.Ascension, Is.EqualTo(1));
        }

        #endregion

        #region Error Handling Tests

        [Test]
        public void Handle_ReturnsError_WhenCharacterNotFound()
        {
            var request = CharacterAscensionRequest.Create("non_existent");

            var response = _handler.Handle(request, ref _testUserData);

            Assert.That(response.IsSuccess, Is.False);
            Assert.That(response.ErrorCode, Is.EqualTo(7001)); // ERROR_CHARACTER_NOT_FOUND
        }

        [Test]
        public void Handle_ReturnsError_WhenLevelRequirementNotMet()
        {
            // 레벨 요구사항 미달 캐릭터
            _testUserData.Characters = new List<OwnedCharacter>
            {
                new OwnedCharacter
                {
                    InstanceId = "inst_001",
                    CharacterId = TEST_CHARACTER_ID,
                    Level = 10, // 20 필요
                    Exp = 5000,
                    Ascension = 0
                }
            };

            var request = CharacterAscensionRequest.Create("inst_001");

            var response = _handler.Handle(request, ref _testUserData);

            Assert.That(response.IsSuccess, Is.False);
            Assert.That(response.ErrorCode, Is.EqualTo(7005)); // ERROR_LEVEL_REQUIREMENT_NOT_MET
        }

        [Test]
        public void Handle_ReturnsError_WhenInsufficientMaterial()
        {
            _testUserData.Items = new List<OwnedItem>
            {
                new OwnedItem
                {
                    ItemId = TEST_ASCENSION_MATERIAL_ID,
                    Count = 2 // 5개 필요
                }
            };

            var request = CharacterAscensionRequest.Create("inst_001");

            var response = _handler.Handle(request, ref _testUserData);

            Assert.That(response.IsSuccess, Is.False);
            Assert.That(response.ErrorCode, Is.EqualTo(7003)); // ERROR_INSUFFICIENT_MATERIAL
        }

        [Test]
        public void Handle_ReturnsError_WhenInsufficientGold()
        {
            _testUserData.Currency = new UserCurrency { Gold = 1000 }; // 10000 필요

            var request = CharacterAscensionRequest.Create("inst_001");

            var response = _handler.Handle(request, ref _testUserData);

            Assert.That(response.IsSuccess, Is.False);
            Assert.That(response.ErrorCode, Is.EqualTo(7004)); // ERROR_INSUFFICIENT_GOLD
        }

        [Test]
        public void Handle_ReturnsError_WhenDatabaseNotSet()
        {
            _handler.SetDatabases(null, null, null, null);

            var request = CharacterAscensionRequest.Create("inst_001");

            var response = _handler.Handle(request, ref _testUserData);

            Assert.That(response.IsSuccess, Is.False);
            Assert.That(response.ErrorCode, Is.EqualTo(9999)); // ERROR_DATABASE_NOT_SET
        }

        [Test]
        public void Handle_ReturnsError_WhenMaxAscensionReached()
        {
            // 최대 돌파 도달 캐릭터 (2단계가 최대)
            _testUserData.Characters = new List<OwnedCharacter>
            {
                new OwnedCharacter
                {
                    InstanceId = "inst_001",
                    CharacterId = TEST_CHARACTER_ID,
                    Level = 40,
                    Exp = 100000,
                    Ascension = 2
                }
            };

            var request = CharacterAscensionRequest.Create("inst_001");

            var response = _handler.Handle(request, ref _testUserData);

            Assert.That(response.IsSuccess, Is.False);
            Assert.That(response.ErrorCode, Is.EqualTo(7006)); // ERROR_MAX_ASCENSION
        }

        #endregion

        #region Multi-Ascension Tests

        [Test]
        public void Handle_AllowsSecondAscension_WhenRequirementsMet()
        {
            // 2차 돌파 가능 상태
            _testUserData.Characters = new List<OwnedCharacter>
            {
                new OwnedCharacter
                {
                    InstanceId = "inst_001",
                    CharacterId = TEST_CHARACTER_ID,
                    Level = 30,
                    Exp = 50000,
                    Ascension = 1
                }
            };

            var request = CharacterAscensionRequest.Create("inst_001");

            var response = _handler.Handle(request, ref _testUserData);

            Assert.That(response.IsSuccess, Is.True);
            Assert.That(response.NewAscension, Is.EqualTo(2));
            Assert.That(response.NewLevelCap, Is.EqualTo(40)); // 20 + 10 + 10
        }

        [Test]
        public void Handle_DeductsCorrectMaterials_ForSecondAscension()
        {
            // 2차 돌파 상태
            _testUserData.Characters = new List<OwnedCharacter>
            {
                new OwnedCharacter
                {
                    InstanceId = "inst_001",
                    CharacterId = TEST_CHARACTER_ID,
                    Level = 30,
                    Exp = 50000,
                    Ascension = 1
                }
            };

            var initialCount = _testUserData.FindItemById(TEST_ASCENSION_MATERIAL_ID)?.Count ?? 0;
            var request = CharacterAscensionRequest.Create("inst_001");

            _handler.Handle(request, ref _testUserData);

            var currentCount = _testUserData.FindItemById(TEST_ASCENSION_MATERIAL_ID)?.Count ?? 0;
            Assert.That(currentCount, Is.EqualTo(initialCount - 10)); // 2차 돌파 재료 10개
        }

        #endregion

        #region Stats and Power Tests

        [Test]
        public void Handle_ReturnsPrevAndNewStats()
        {
            var request = CharacterAscensionRequest.Create("inst_001");

            var response = _handler.Handle(request, ref _testUserData);

            Assert.That(response.IsSuccess, Is.True);
            Assert.That(response.PreviousStats, Is.Not.EqualTo(default(CharacterStats)));
            Assert.That(response.NewStats, Is.Not.EqualTo(default(CharacterStats)));
        }

        [Test]
        public void Handle_IncreasesStats_WhenAscensionValid()
        {
            var request = CharacterAscensionRequest.Create("inst_001");

            var response = _handler.Handle(request, ref _testUserData);

            Assert.That(response.IsSuccess, Is.True);
            // 돌파로 스탯 보너스 적용됨
            Assert.That(response.NewStats.HP, Is.GreaterThan(response.PreviousStats.HP));
            Assert.That(response.NewStats.ATK, Is.GreaterThan(response.PreviousStats.ATK));
        }

        [Test]
        public void Handle_IncreasesPower_WhenAscensionValid()
        {
            var request = CharacterAscensionRequest.Create("inst_001");

            var response = _handler.Handle(request, ref _testUserData);

            Assert.That(response.IsSuccess, Is.True);
            Assert.That(response.NewPower, Is.GreaterThan(response.PreviousPower));
        }

        #endregion

        #region Delta Tests

        [Test]
        public void Handle_ReturnsDelta_WhenAscensionValid()
        {
            var request = CharacterAscensionRequest.Create("inst_001");

            var response = _handler.Handle(request, ref _testUserData);

            Assert.That(response.Delta, Is.Not.Null);
            Assert.That(response.Delta.HasChanges, Is.True);
            Assert.That(response.Delta.Currency.HasValue, Is.True);
        }

        #endregion
    }
}
