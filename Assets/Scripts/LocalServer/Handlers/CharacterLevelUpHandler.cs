using System.Linq;
using Sc.Data;

namespace Sc.LocalServer
{
    /// <summary>
    /// 캐릭터 레벨업 요청 처리
    /// </summary>
    public class CharacterLevelUpHandler : IRequestHandler<CharacterLevelUpRequest, CharacterLevelUpResponse>
    {
        private readonly ServerValidator _validator;
        private readonly RewardService _rewardService;

        private CharacterDatabase _characterDb;
        private CharacterLevelDatabase _levelDb;
        private CharacterAscensionDatabase _ascensionDb;
        private ItemDatabase _itemDb;

        // 에러 코드
        private const int ERROR_CHARACTER_NOT_FOUND = 7001;
        private const int ERROR_MAX_LEVEL = 7002;
        private const int ERROR_INSUFFICIENT_MATERIAL = 7003;
        private const int ERROR_INSUFFICIENT_GOLD = 7004;
        private const int ERROR_LEVEL_CAP_REACHED = 7007;
        private const int ERROR_DATABASE_NOT_SET = 9999;

        public CharacterLevelUpHandler(ServerValidator validator, RewardService rewardService)
        {
            _validator = validator;
            _rewardService = rewardService;
        }

        public void SetDatabases(
            CharacterDatabase characterDb,
            CharacterLevelDatabase levelDb,
            CharacterAscensionDatabase ascensionDb,
            ItemDatabase itemDb)
        {
            _characterDb = characterDb;
            _levelDb = levelDb;
            _ascensionDb = ascensionDb;
            _itemDb = itemDb;
        }

        public CharacterLevelUpResponse Handle(CharacterLevelUpRequest request, ref UserSaveData userData)
        {
            // 0. 데이터베이스 확인
            if (_characterDb == null || _levelDb == null || _ascensionDb == null)
            {
                return CharacterLevelUpResponse.Fail(ERROR_DATABASE_NOT_SET, "데이터베이스가 초기화되지 않았습니다.");
            }

            // 1. 캐릭터 조회
            var character = userData.FindCharacterByInstanceId(request.CharacterInstanceId);
            if (character == null)
            {
                return CharacterLevelUpResponse.Fail(ERROR_CHARACTER_NOT_FOUND, "캐릭터를 찾을 수 없습니다.");
            }

            var charValue = character.Value;
            var charData = _characterDb.GetById(charValue.CharacterId);
            if (charData == null)
            {
                return CharacterLevelUpResponse.Fail(ERROR_CHARACTER_NOT_FOUND, "캐릭터 데이터를 찾을 수 없습니다.");
            }

            // 2. 레벨 상한 확인
            int levelCap = _ascensionDb.GetLevelCap(charData.Rarity, charValue.Ascension, _levelDb.BaseLevelCap);
            int maxLevel = _levelDb.GetMaxLevel(charData.Rarity);

            if (charValue.Level >= levelCap)
            {
                return CharacterLevelUpResponse.Fail(ERROR_LEVEL_CAP_REACHED, "레벨 상한에 도달했습니다. 돌파가 필요합니다.");
            }

            if (charValue.Level >= maxLevel)
            {
                return CharacterLevelUpResponse.Fail(ERROR_MAX_LEVEL, "최대 레벨에 도달했습니다.");
            }

            // 3. 재료 검증 및 경험치 계산
            long totalExpGain = 0;
            int totalGoldCost = 0;

            foreach (var (itemId, count) in request.MaterialUsage)
            {
                var item = userData.FindItemById(itemId);
                if (item == null || item.Value.Count < count)
                {
                    return CharacterLevelUpResponse.Fail(ERROR_INSUFFICIENT_MATERIAL, $"재료가 부족합니다: {itemId}");
                }

                // 아이템의 경험치 값 조회 (ItemDatabase에서)
                var itemData = _itemDb?.GetById(itemId);
                if (itemData != null)
                {
                    totalExpGain += itemData.ExpValue * count;
                    totalGoldCost += itemData.GoldCostPerUse * count;
                }
            }

            // 4. 골드 검증
            if (!_validator.HasEnoughGold(userData.Currency, totalGoldCost))
            {
                return CharacterLevelUpResponse.Fail(ERROR_INSUFFICIENT_GOLD, "골드가 부족합니다.");
            }

            // 5. 이전 상태 저장
            int prevLevel = charValue.Level;
            long prevExp = charValue.Exp;
            var prevStats = PowerCalculator.CalculateStats(charValue, charData, _ascensionDb);
            int prevPower = PowerCalculator.Calculate(prevStats);

            // 6. 경험치 적용 및 레벨 계산
            long newTotalExp = charValue.Exp + totalExpGain;
            int newLevel = _levelDb.CalculateLevelFromExp(charData.Rarity, newTotalExp, levelCap);

            // 7. 재료 차감
            foreach (var (itemId, count) in request.MaterialUsage)
            {
                DeductItem(ref userData, itemId, count);
            }

            // 8. 골드 차감
            _rewardService.DeductGold(ref userData.Currency, totalGoldCost);

            // 9. 캐릭터 업데이트
            charValue.Level = newLevel;
            charValue.Exp = newTotalExp;
            UpdateCharacter(ref userData, charValue);

            // 10. 새 스탯 계산
            var newStats = PowerCalculator.CalculateStats(charValue, charData, _ascensionDb);
            int newPower = PowerCalculator.Calculate(newStats);

            // 11. Delta 생성
            var delta = new UserDataDelta
            {
                Currency = userData.Currency
            };

            return CharacterLevelUpResponse.Success(
                prevLevel, newLevel,
                prevExp, newTotalExp,
                prevStats, newStats,
                prevPower, newPower,
                delta
            );
        }

        private void DeductItem(ref UserSaveData userData, string itemId, int amount)
        {
            for (int i = 0; i < userData.Items.Count; i++)
            {
                if (userData.Items[i].ItemId == itemId)
                {
                    var item = userData.Items[i];
                    item.Count -= amount;
                    if (item.Count <= 0)
                    {
                        userData.Items.RemoveAt(i);
                    }
                    else
                    {
                        userData.Items[i] = item;
                    }
                    return;
                }
            }
        }

        private void UpdateCharacter(ref UserSaveData userData, OwnedCharacter updated)
        {
            for (int i = 0; i < userData.Characters.Count; i++)
            {
                if (userData.Characters[i].InstanceId == updated.InstanceId)
                {
                    userData.Characters[i] = updated;
                    return;
                }
            }
        }
    }
}
