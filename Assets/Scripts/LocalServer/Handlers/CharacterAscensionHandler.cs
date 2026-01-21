using Sc.Data;

namespace Sc.LocalServer
{
    /// <summary>
    /// 캐릭터 돌파 요청 처리
    /// </summary>
    public class CharacterAscensionHandler : IRequestHandler<CharacterAscensionRequest, CharacterAscensionResponse>
    {
        private readonly ServerValidator _validator;
        private readonly RewardService _rewardService;

        private CharacterDatabase _characterDb;
        private CharacterLevelDatabase _levelDb;
        private CharacterAscensionDatabase _ascensionDb;
        private ItemDatabase _itemDb;

        // 에러 코드
        private const int ERROR_CHARACTER_NOT_FOUND = 7001;
        private const int ERROR_INSUFFICIENT_MATERIAL = 7003;
        private const int ERROR_INSUFFICIENT_GOLD = 7004;
        private const int ERROR_LEVEL_REQUIREMENT_NOT_MET = 7005;
        private const int ERROR_MAX_ASCENSION = 7006;
        private const int ERROR_DATABASE_NOT_SET = 9999;

        public CharacterAscensionHandler(ServerValidator validator, RewardService rewardService)
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

        public CharacterAscensionResponse Handle(CharacterAscensionRequest request, ref UserSaveData userData)
        {
            // 0. 데이터베이스 확인
            if (_characterDb == null || _levelDb == null || _ascensionDb == null)
            {
                return CharacterAscensionResponse.Fail(ERROR_DATABASE_NOT_SET, "데이터베이스가 초기화되지 않았습니다.");
            }

            // 1. 캐릭터 조회
            var character = userData.FindCharacterByInstanceId(request.CharacterInstanceId);
            if (character == null)
            {
                return CharacterAscensionResponse.Fail(ERROR_CHARACTER_NOT_FOUND, "캐릭터를 찾을 수 없습니다.");
            }

            var charValue = character.Value;
            var charData = _characterDb.GetById(charValue.CharacterId);
            if (charData == null)
            {
                return CharacterAscensionResponse.Fail(ERROR_CHARACTER_NOT_FOUND, "캐릭터 데이터를 찾을 수 없습니다.");
            }

            // 2. 최대 돌파 확인
            int maxAscension = _ascensionDb.GetMaxAscension(charData.Rarity);
            if (charValue.Ascension >= maxAscension)
            {
                return CharacterAscensionResponse.Fail(ERROR_MAX_ASCENSION, "최대 돌파 단계에 도달했습니다.");
            }

            // 3. 돌파 요구사항 조회
            var requirement = _ascensionDb.GetRequirement(charData.Rarity, charValue.Ascension);
            if (!requirement.HasValue)
            {
                return CharacterAscensionResponse.Fail(ERROR_DATABASE_NOT_SET, "돌파 요구사항을 찾을 수 없습니다.");
            }

            var req = requirement.Value;

            // 4. 레벨 요구사항 확인
            if (charValue.Level < req.RequiredCharacterLevel)
            {
                return CharacterAscensionResponse.Fail(
                    ERROR_LEVEL_REQUIREMENT_NOT_MET,
                    $"레벨 {req.RequiredCharacterLevel} 이상이 필요합니다."
                );
            }

            // 5. 재료 검증
            foreach (var material in req.Materials)
            {
                if (material.Type == RewardType.Item)
                {
                    var item = userData.FindItemById(material.ItemId);
                    if (item == null || item.Value.Count < material.Amount)
                    {
                        return CharacterAscensionResponse.Fail(
                            ERROR_INSUFFICIENT_MATERIAL,
                            $"재료가 부족합니다: {material.ItemId}"
                        );
                    }
                }
            }

            // 6. 골드 검증
            if (!_validator.HasEnoughGold(userData.Currency, req.GoldCost))
            {
                return CharacterAscensionResponse.Fail(ERROR_INSUFFICIENT_GOLD, "골드가 부족합니다.");
            }

            // 7. 이전 상태 저장
            int prevAscension = charValue.Ascension;
            int prevLevelCap = _ascensionDb.GetLevelCap(charData.Rarity, charValue.Ascension, _levelDb.BaseLevelCap);
            var prevStats = PowerCalculator.CalculateStats(charValue, charData, _ascensionDb);
            int prevPower = PowerCalculator.Calculate(prevStats);

            // 8. 재료 차감
            foreach (var material in req.Materials)
            {
                if (material.Type == RewardType.Item)
                {
                    DeductItem(ref userData, material.ItemId, material.Amount);
                }
            }

            // 9. 골드 차감
            _rewardService.DeductGold(ref userData.Currency, req.GoldCost);

            // 10. 캐릭터 돌파 적용
            charValue.Ascension += 1;
            UpdateCharacter(ref userData, charValue);

            // 11. 새 상태 계산
            int newAscension = charValue.Ascension;
            int newLevelCap = _ascensionDb.GetLevelCap(charData.Rarity, charValue.Ascension, _levelDb.BaseLevelCap);
            var newStats = PowerCalculator.CalculateStats(charValue, charData, _ascensionDb);
            int newPower = PowerCalculator.Calculate(newStats);

            // 12. Delta 생성
            var delta = new UserDataDelta
            {
                Currency = userData.Currency
            };

            return CharacterAscensionResponse.Success(
                prevAscension, newAscension,
                prevLevelCap, newLevelCap,
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
