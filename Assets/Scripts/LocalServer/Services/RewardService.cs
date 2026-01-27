using System.Collections.Generic;
using Sc.Data;

namespace Sc.LocalServer
{
    /// <summary>
    /// 보상 서비스 (서버측)
    /// Delta 생성 및 보상 적용 로직
    /// </summary>
    public class RewardService
    {
        /// <summary>
        /// 재화 변경으로 Delta 생성
        /// </summary>
        public UserDataDelta CreateCurrencyDelta(UserCurrency currency)
        {
            return UserDataDelta.WithCurrency(currency);
        }

        /// <summary>
        /// 캐릭터 추가로 Delta 생성
        /// </summary>
        public UserDataDelta CreateCharacterDelta(
            UserCurrency currency,
            List<OwnedCharacter> addedCharacters,
            GachaPityData pityData = default)
        {
            return new UserDataDelta
            {
                Currency = currency,
                AddedCharacters = addedCharacters,
                GachaPity = pityData
            };
        }

        /// <summary>
        /// 보상 정보 배열로 Delta 생성
        /// </summary>
        public UserDataDelta CreateRewardDelta(
            RewardInfo[] rewards,
            ref UserSaveData userData)
        {
            var addedCharacters = new List<OwnedCharacter>();

            foreach (var reward in rewards)
            {
                ApplyReward(reward, ref userData, addedCharacters);
            }

            return new UserDataDelta
            {
                Currency = userData.Currency,
                AddedCharacters = addedCharacters.Count > 0 ? addedCharacters : null
            };
        }

        /// <summary>
        /// 개별 보상 적용
        /// </summary>
        private void ApplyReward(
            RewardInfo reward,
            ref UserSaveData userData,
            List<OwnedCharacter> addedCharacters)
        {
            switch (reward.Type)
            {
                case RewardType.Currency:
                    ApplyCurrencyReward(reward, ref userData);
                    break;

                case RewardType.Character:
                    var newChar = OwnedCharacter.Create(reward.ItemId);
                    userData.Characters.Add(newChar);
                    addedCharacters.Add(newChar);
                    break;

                case RewardType.Item:
                    // TODO[P1]: 인벤토리 시스템 연동
                    break;

                case RewardType.PlayerExp:
                    userData.Profile.Exp += reward.Amount;
                    // TODO[FUTURE]: 레벨업 체크
                    break;
            }
        }

        /// <summary>
        /// 재화 보상 적용
        /// </summary>
        private void ApplyCurrencyReward(RewardInfo reward, ref UserSaveData userData)
        {
            // CostType으로 변환 시도
            if (!System.Enum.TryParse<CostType>(reward.ItemId, out var costType))
                return;

            switch (costType)
            {
                case CostType.Gold:
                    userData.Currency.Gold += reward.Amount;
                    break;
                case CostType.Gem:
                    userData.Currency.FreeGem += reward.Amount;
                    break;
                case CostType.Stamina:
                    userData.Currency.Stamina += reward.Amount;
                    break;
                // 다른 재화 타입 추가...
            }
        }

        /// <summary>
        /// 재화 차감 (무료 재화 우선 사용)
        /// </summary>
        public bool DeductGem(ref UserCurrency currency, int amount)
        {
            if (currency.TotalGem < amount)
                return false;

            if (currency.FreeGem >= amount)
            {
                currency.FreeGem -= amount;
            }
            else
            {
                var remaining = amount - currency.FreeGem;
                currency.FreeGem = 0;
                currency.Gem -= remaining;
            }

            return true;
        }

        /// <summary>
        /// 골드 차감
        /// </summary>
        public bool DeductGold(ref UserCurrency currency, int amount)
        {
            if (currency.Gold < amount)
                return false;

            currency.Gold -= amount;
            return true;
        }
    }
}