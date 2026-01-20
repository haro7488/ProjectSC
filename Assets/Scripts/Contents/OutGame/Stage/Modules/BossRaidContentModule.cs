using Sc.Core;
using Sc.Data;
using Sc.Foundation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sc.Contents.Stage
{
    /// <summary>
    /// 보스 레이드 컨텐츠 모듈.
    /// 보스 HP 게이지, 내 기여도, 랭킹 버튼 표시.
    /// </summary>
    public class BossRaidContentModule : BaseStageContentModule
    {
        private Image _bossIcon;
        private TMP_Text _bossNameText;
        private Slider _bossHPSlider;
        private TMP_Text _bossHPText;
        private TMP_Text _contributionText;
        private Button _rankingButton;

        private StageCategoryData _categoryData;

        protected override string GetPrefabAddress()
        {
            // UI 프리팹이 준비되면 아래 주석 해제
            // return "UI/Stage/BossRaidModuleUI";
            return null;
        }

        protected override void OnInitialize()
        {
            Log.Debug("[BossRaidContentModule] OnInitialize", LogCategory.UI);

            // UI 컴포넌트 찾기 (프리팹 있을 경우)
            if (_rootInstance != null)
            {
                _bossIcon = FindTransform("BossIcon")?.GetComponent<Image>();
                _bossNameText = FindTransform("BossName")?.GetComponent<TMP_Text>();
                _bossHPSlider = FindComponent<Slider>();
                _bossHPText = FindTransform("BossHP")?.GetComponent<TMP_Text>();
                _contributionText = FindTransform("Contribution")?.GetComponent<TMP_Text>();
                _rankingButton = FindTransform("RankingButton")?.GetComponent<Button>();

                if (_rankingButton != null)
                {
                    _rankingButton.onClick.AddListener(OnRankingButtonClicked);
                }
            }

            // 카테고리 데이터 로드
            LoadCategoryData();

            // UI 업데이트
            UpdateUI();
        }

        protected override void OnRefreshInternal(string selectedStageId)
        {
            // 보스 HP, 기여도 갱신
            UpdateBossStatus();
        }

        protected override void OnStageSelectedInternal(StageData stageData)
        {
            // 선택된 스테이지 기반 보스 정보 갱신
            UpdateBossInfo(stageData);
        }

        protected override void OnCategoryIdChanged(string categoryId)
        {
            // 외부에서 카테고리 변경 시 (StageDashboard에서 다른 보스 선택)
            LoadCategoryData();
            UpdateUI();
        }

        protected override void OnReleaseInternal()
        {
            if (_rankingButton != null)
            {
                _rankingButton.onClick.RemoveListener(OnRankingButtonClicked);
            }

            _categoryData = null;
        }

        #region Private Methods

        private void LoadCategoryData()
        {
            if (string.IsNullOrEmpty(_categoryId))
            {
                Log.Debug("[BossRaidContentModule] No categoryId set", LogCategory.UI);
                _categoryData = null;
                return;
            }

            var categoryDb = DataManager.Instance?.GetDatabase<StageCategoryDatabase>();
            if (categoryDb == null)
            {
                Log.Warning("[BossRaidContentModule] StageCategoryDatabase not found", LogCategory.UI);
                _categoryData = null;
                return;
            }

            _categoryData = categoryDb.GetById(_categoryId);

            if (_categoryData == null)
            {
                Log.Warning($"[BossRaidContentModule] Category not found: {_categoryId}", LogCategory.UI);
            }
            else
            {
                Log.Debug($"[BossRaidContentModule] Loaded category: {_categoryData.Id}", LogCategory.UI);
            }
        }

        private void UpdateUI()
        {
            if (_categoryData == null)
            {
                ClearUI();
                return;
            }

            // 보스 아이콘
            if (_bossIcon != null && _categoryData.IconSprite != null)
            {
                _bossIcon.sprite = _categoryData.IconSprite;
                _bossIcon.gameObject.SetActive(true);
            }

            // 보스 이름
            if (_bossNameText != null)
            {
                _bossNameText.text = _categoryData.GetDisplayName();
            }

            // 보스 HP, 기여도 초기화
            UpdateBossStatus();
        }

        private void ClearUI()
        {
            if (_bossIcon != null)
            {
                _bossIcon.gameObject.SetActive(false);
            }

            if (_bossNameText != null)
            {
                _bossNameText.text = "";
            }

            if (_bossHPSlider != null)
            {
                _bossHPSlider.value = 1f;
            }

            if (_bossHPText != null)
            {
                _bossHPText.text = "";
            }

            if (_contributionText != null)
            {
                _contributionText.text = "";
            }
        }

        private void UpdateBossStatus()
        {
            // TODO: 서버에서 보스 레이드 상태 조회
            // 현재는 플레이스홀더 값 사용
            var currentHP = 75f;
            var maxHP = 100f;
            var myContribution = 1250;

            if (_bossHPSlider != null)
            {
                _bossHPSlider.value = currentHP / maxHP;
            }

            if (_bossHPText != null)
            {
                _bossHPText.text = $"HP: {currentHP:N0}/{maxHP:N0}";
            }

            if (_contributionText != null)
            {
                _contributionText.text = $"내 기여도: {myContribution:N0}";
            }
        }

        private void UpdateBossInfo(StageData stageData)
        {
            if (stageData == null) return;

            // 스테이지 기반 보스 정보 갱신 (필요 시)
            Log.Debug($"[BossRaidContentModule] Boss stage selected: {stageData.Id}", LogCategory.UI);
        }

        private void OnRankingButtonClicked()
        {
            Log.Debug("[BossRaidContentModule] Ranking button clicked", LogCategory.UI);
            // TODO: 랭킹 팝업 표시
        }

        #endregion
    }
}