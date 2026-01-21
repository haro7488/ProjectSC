using System.Collections.Generic;
using NUnit.Framework;
using Sc.Data;
using Sc.LocalServer;
using UnityEngine;

namespace Sc.Editor.Tests.LocalServer
{
    /// <summary>
    /// CharacterLevelUpHandler 단위 테스트.
    /// 캐릭터 레벨업 로직 검증.
    /// </summary>
    [TestFixture]
    public class CharacterLevelUpHandlerTests
    {
        private CharacterLevelUpHandler _handler;
        private CharacterDatabase _characterDb;
        private CharacterLevelDatabase _levelDb;
        private CharacterAscensionDatabase _ascensionDb;
        private ItemDatabase _itemDb;
        private UserSaveData _testUserData;
        private CharacterData _testCharacterData;
        private ItemData _expMaterial;

        private const string TEST_CHARACTER_ID = "char_test_001";
        private const string TEST_EXP_MATERIAL_ID = "item_exp_001";

        [SetUp]
        public void SetUp()
        {
            var timeService = new ServerTimeService();
            var validator = new ServerValidator(timeService);
            var rewardService = new RewardService();

            _handler = new CharacterLevelUpHandler(validator, rewardService);

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
                        new LevelRequirement(3, 300, 200),
                        new LevelRequirement(4, 600, 300),
                        new LevelRequirement(5, 1000, 400),
                        new LevelRequirement(10, 5000, 1000),
                        new LevelRequirement(20, 20000, 2000),
                        new LevelRequirement(30, 50000, 5000),
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
                            Materials = new List<RewardInfo> { RewardInfo.Item("item_asc_001", 5) },
                            GoldCost = 10000,
                            StatBonus = new CharacterStats(100, 10, 5, 5, 0.01f, 0.05f),
                            LevelCapIncrease = 10
                        }
                    }
                }
            };
            _ascensionDb.SetData(ascensionData);

            // 테스트용 경험치 재료 생성
            _expMaterial = ScriptableObject.CreateInstance<ItemData>();
            _expMaterial.Initialize(
                id: TEST_EXP_MATERIAL_ID,
                name: "경험치 물약",
                nameEn: "EXP Potion",
                type: ItemType.Consumable,
                rarity: Rarity.R,
                atkBonus: 0,
                defBonus: 0,
                hpBonus: 0,
                description: "경험치 제공",
                maxStackCount: 999,
                consumeCount: 1,
                cooldown: 0f,
                expValue: 100,
                goldCostPerUse: 50
            );
            _itemDb.Add(_expMaterial);

            _handler.SetDatabases(_characterDb, _levelDb, _ascensionDb, _itemDb);

            // 테스트 유저 데이터
            _testUserData = UserSaveData.CreateNew("test_uid", "TestUser");
            _testUserData.Currency = new UserCurrency { Gold = 100000, Gem = 1000, FreeGem = 500 };
            _testUserData.Characters = new List<OwnedCharacter>
            {
                new OwnedCharacter
                {
                    InstanceId = "inst_001",
                    CharacterId = TEST_CHARACTER_ID,
                    Level = 1,
                    Exp = 0,
                    Ascension = 0
                }
            };
            _testUserData.Items = new List<OwnedItem>
            {
                new OwnedItem
                {
                    ItemId = TEST_EXP_MATERIAL_ID,
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
            if (_expMaterial != null) Object.DestroyImmediate(_expMaterial);
        }

        #region Basic Level Up Tests

        [Test]
        public void Handle_ReturnsSuccess_WhenLevelUpValid()
        {
            var request = CharacterLevelUpRequest.Create(
                "inst_001",
                new Dictionary<string, int> { { TEST_EXP_MATERIAL_ID, 1 } }
            );

            var response = _handler.Handle(request, ref _testUserData);

            Assert.That(response.IsSuccess, Is.True);
            Assert.That(response.PreviousLevel, Is.EqualTo(1));
            Assert.That(response.NewLevel, Is.EqualTo(1)); // 100 exp로는 레벨업 못함
            Assert.That(response.NewExp, Is.EqualTo(100));
        }

        [Test]
        public void Handle_LevelsUp_WhenEnoughExp()
        {
            var request = CharacterLevelUpRequest.Create(
                "inst_001",
                new Dictionary<string, int> { { TEST_EXP_MATERIAL_ID, 5 } } // 500 exp
            );

            var response = _handler.Handle(request, ref _testUserData);

            Assert.That(response.IsSuccess, Is.True);
            Assert.That(response.NewLevel, Is.EqualTo(3)); // 500 exp -> level 3 (300 exp)
            Assert.That(response.NewExp, Is.EqualTo(500));
        }

        [Test]
        public void Handle_DeductsMaterials_WhenLevelUpValid()
        {
            var initialCount = _testUserData.FindItemById(TEST_EXP_MATERIAL_ID)?.Count ?? 0;
            var request = CharacterLevelUpRequest.Create(
                "inst_001",
                new Dictionary<string, int> { { TEST_EXP_MATERIAL_ID, 5 } }
            );

            _handler.Handle(request, ref _testUserData);

            var currentCount = _testUserData.FindItemById(TEST_EXP_MATERIAL_ID)?.Count ?? 0;
            Assert.That(currentCount, Is.EqualTo(initialCount - 5));
        }

        [Test]
        public void Handle_DeductsGold_WhenLevelUpValid()
        {
            var initialGold = _testUserData.Currency.Gold;
            var request = CharacterLevelUpRequest.Create(
                "inst_001",
                new Dictionary<string, int> { { TEST_EXP_MATERIAL_ID, 5 } } // 5 * 50 = 250 gold
            );

            _handler.Handle(request, ref _testUserData);

            Assert.That(_testUserData.Currency.Gold, Is.EqualTo(initialGold - 250));
        }

        [Test]
        public void Handle_UpdatesCharacterData_WhenLevelUpValid()
        {
            var request = CharacterLevelUpRequest.Create(
                "inst_001",
                new Dictionary<string, int> { { TEST_EXP_MATERIAL_ID, 10 } } // 1000 exp
            );

            _handler.Handle(request, ref _testUserData);

            var character = _testUserData.FindCharacterByInstanceId("inst_001");
            Assert.That(character.HasValue, Is.True);
            Assert.That(character.Value.Level, Is.EqualTo(5));
            Assert.That(character.Value.Exp, Is.EqualTo(1000));
        }

        #endregion

        #region Error Handling Tests

        [Test]
        public void Handle_ReturnsError_WhenCharacterNotFound()
        {
            var request = CharacterLevelUpRequest.Create(
                "non_existent",
                new Dictionary<string, int> { { TEST_EXP_MATERIAL_ID, 1 } }
            );

            var response = _handler.Handle(request, ref _testUserData);

            Assert.That(response.IsSuccess, Is.False);
            Assert.That(response.ErrorCode, Is.EqualTo(7001)); // ERROR_CHARACTER_NOT_FOUND
        }

        [Test]
        public void Handle_ReturnsError_WhenInsufficientMaterial()
        {
            var request = CharacterLevelUpRequest.Create(
                "inst_001",
                new Dictionary<string, int> { { TEST_EXP_MATERIAL_ID, 200 } } // 100개만 보유
            );

            var response = _handler.Handle(request, ref _testUserData);

            Assert.That(response.IsSuccess, Is.False);
            Assert.That(response.ErrorCode, Is.EqualTo(7003)); // ERROR_INSUFFICIENT_MATERIAL
        }

        [Test]
        public void Handle_ReturnsError_WhenInsufficientGold()
        {
            _testUserData.Currency = new UserCurrency { Gold = 10 }; // 부족한 골드
            var request = CharacterLevelUpRequest.Create(
                "inst_001",
                new Dictionary<string, int> { { TEST_EXP_MATERIAL_ID, 5 } } // 250 gold 필요
            );

            var response = _handler.Handle(request, ref _testUserData);

            Assert.That(response.IsSuccess, Is.False);
            Assert.That(response.ErrorCode, Is.EqualTo(7004)); // ERROR_INSUFFICIENT_GOLD
        }

        [Test]
        public void Handle_ReturnsError_WhenDatabaseNotSet()
        {
            _handler.SetDatabases(null, null, null, null);
            var request = CharacterLevelUpRequest.Create(
                "inst_001",
                new Dictionary<string, int> { { TEST_EXP_MATERIAL_ID, 1 } }
            );

            var response = _handler.Handle(request, ref _testUserData);

            Assert.That(response.IsSuccess, Is.False);
            Assert.That(response.ErrorCode, Is.EqualTo(9999)); // ERROR_DATABASE_NOT_SET
        }

        #endregion

        #region Level Cap Tests

        [Test]
        public void Handle_ReturnsError_WhenAtLevelCap()
        {
            // 레벨 캡에 도달한 캐릭터 설정 (돌파 0, 레벨캡 20)
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

            var request = CharacterLevelUpRequest.Create(
                "inst_001",
                new Dictionary<string, int> { { TEST_EXP_MATERIAL_ID, 1 } }
            );

            var response = _handler.Handle(request, ref _testUserData);

            Assert.That(response.IsSuccess, Is.False);
            Assert.That(response.ErrorCode, Is.EqualTo(7007)); // ERROR_LEVEL_CAP_REACHED
        }

        [Test]
        public void Handle_RespectsLevelCap_WhenAscension0()
        {
            // 레벨 19에서 많은 경험치 투입
            _testUserData.Characters = new List<OwnedCharacter>
            {
                new OwnedCharacter
                {
                    InstanceId = "inst_001",
                    CharacterId = TEST_CHARACTER_ID,
                    Level = 10,
                    Exp = 5000,
                    Ascension = 0
                }
            };

            var request = CharacterLevelUpRequest.Create(
                "inst_001",
                new Dictionary<string, int> { { TEST_EXP_MATERIAL_ID, 100 } } // 10000 exp
            );

            var response = _handler.Handle(request, ref _testUserData);

            Assert.That(response.IsSuccess, Is.True);
            Assert.That(response.NewLevel, Is.EqualTo(20)); // 캡에서 멈춤
        }

        #endregion

        #region Stats and Power Tests

        [Test]
        public void Handle_ReturnsPrevAndNewStats()
        {
            var request = CharacterLevelUpRequest.Create(
                "inst_001",
                new Dictionary<string, int> { { TEST_EXP_MATERIAL_ID, 10 } }
            );

            var response = _handler.Handle(request, ref _testUserData);

            Assert.That(response.IsSuccess, Is.True);
            Assert.That(response.PreviousStats, Is.Not.EqualTo(default(CharacterStats)));
            Assert.That(response.NewStats, Is.Not.EqualTo(default(CharacterStats)));
        }

        [Test]
        public void Handle_ReturnsPrevAndNewPower()
        {
            var request = CharacterLevelUpRequest.Create(
                "inst_001",
                new Dictionary<string, int> { { TEST_EXP_MATERIAL_ID, 10 } }
            );

            var response = _handler.Handle(request, ref _testUserData);

            Assert.That(response.IsSuccess, Is.True);
            Assert.That(response.PreviousPower, Is.GreaterThan(0));
            Assert.That(response.NewPower, Is.GreaterThan(0));
        }

        #endregion

        #region Delta Tests

        [Test]
        public void Handle_ReturnsDelta_WhenLevelUpValid()
        {
            var request = CharacterLevelUpRequest.Create(
                "inst_001",
                new Dictionary<string, int> { { TEST_EXP_MATERIAL_ID, 5 } }
            );

            var response = _handler.Handle(request, ref _testUserData);

            Assert.That(response.Delta, Is.Not.Null);
            Assert.That(response.Delta.HasChanges, Is.True);
            Assert.That(response.Delta.Currency.HasValue, Is.True);
        }

        #endregion
    }
}
