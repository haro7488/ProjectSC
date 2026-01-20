using Sc.Core;
using Sc.Data;
using Sc.Foundation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sc.Contents.Stage
{
    /// <summary>
    /// 무한의 탑 컨텐츠 모듈.
    /// 현재 층, 최고 층, 보상 미리보기 표시.
    /// </summary>
    public class TowerContentModule : BaseStageContentModule
    {
        private TMP_Text _currentFloorText;
        private TMP_Text _highestFloorText;
        private TMP_Text _nextRewardText;
        private Image _rewardIcon;
        private Slider _progressSlider;

        protected override string GetPrefabAddress()
        {
            // UI 프리팹이 준비되면 아래 주석 해제
            // return "UI/Stage/TowerModuleUI";
            return null;
        }

        protected override void OnInitialize()
        {
            Log.Debug("[TowerContentModule] OnInitialize", LogCategory.UI);

            // UI 컴포넌트 찾기 (프리팹 있을 경우)
            if (_rootInstance != null)
            {
                _currentFloorText = FindTransform("CurrentFloor")?.GetComponent<TMP_Text>();
                _highestFloorText = FindTransform("HighestFloor")?.GetComponent<TMP_Text>();
                _nextRewardText = FindTransform("NextReward")?.GetComponent<TMP_Text>();
                _rewardIcon = FindTransform("RewardIcon")?.GetComponent<Image>();
                _progressSlider = FindComponent<Slider>();
            }

            // UI 업데이트
            UpdateUI();
        }

        protected override void OnRefreshInternal(string selectedStageId)
        {
            UpdateUI();
        }

        protected override void OnStageSelectedInternal(StageData stageData)
        {
            // 선택된 스테이지(층) 기반 보상 미리보기
            UpdateRewardPreview(stageData);
        }

        protected override void OnReleaseInternal()
        {
            // 특별한 해제 작업 없음
        }

        #region Private Methods

        private void UpdateUI()
        {
            var towerProgress = GetTowerProgress();

            // 현재 층
            if (_currentFloorText != null)
            {
                _currentFloorText.text = $"현재: {towerProgress.currentFloor}층";
            }

            // 최고 층
            if (_highestFloorText != null)
            {
                _highestFloorText.text = $"최고: {towerProgress.highestFloor}층";
            }

            // 진행도 슬라이더
            if (_progressSlider != null)
            {
                var nextMilestone = GetNextMilestone(towerProgress.highestFloor);
                var prevMilestone = GetPreviousMilestone(towerProgress.highestFloor);
                var range = nextMilestone - prevMilestone;
                var progress = towerProgress.highestFloor - prevMilestone;
                _progressSlider.value = range > 0 ? (float)progress / range : 0f;
            }

            // 다음 보상 미리보기
            UpdateNextRewardText(towerProgress.highestFloor);
        }

        private void UpdateRewardPreview(StageData stageData)
        {
            if (_nextRewardText == null) return;

            if (stageData != null)
            {
                // 선택된 층의 보상
                if (stageData.FirstClearRewards != null && stageData.FirstClearRewards.Count > 0)
                {
                    var reward = stageData.FirstClearRewards[0];
                    _nextRewardText.text = $"클리어 보상: {reward.Amount:N0}";
                }
                else
                {
                    _nextRewardText.text = $"클리어 보상: 골드 {stageData.RewardGold:N0}";
                }
            }
        }

        private void UpdateNextRewardText(int highestFloor)
        {
            if (_nextRewardText == null) return;

            var nextMilestone = GetNextMilestone(highestFloor);
            var floorsToGo = nextMilestone - highestFloor;

            if (floorsToGo > 0)
            {
                _nextRewardText.text = $"{nextMilestone}층 보상까지 {floorsToGo}층";
            }
            else
            {
                _nextRewardText.text = "모든 마일스톤 달성!";
            }
        }

        private (int currentFloor, int highestFloor) GetTowerProgress()
        {
            // TODO: 유저 데이터에서 탑 진행도 조회
            // 현재는 플레이스홀더 값 사용
            if (DataManager.Instance != null)
            {
                var progress = DataManager.Instance.StageProgress;
                // 탑 진행도가 별도로 저장되면 여기서 조회
                // 현재는 기본값 반환
            }

            return (currentFloor: 15, highestFloor: 14);
        }

        private int GetNextMilestone(int currentFloor)
        {
            // 10층 단위 마일스톤
            return ((currentFloor / 10) + 1) * 10;
        }

        private int GetPreviousMilestone(int currentFloor)
        {
            return (currentFloor / 10) * 10;
        }

        #endregion
    }
}
