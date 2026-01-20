using Sc.Core;
using Sc.Data;
using Sc.Foundation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sc.Contents.Stage
{
    /// <summary>
    /// 경험치 던전 컨텐츠 모듈.
    /// 난이도 표시, 획득 경험치 미리보기.
    /// </summary>
    public class ExpDungeonContentModule : BaseStageContentModule
    {
        private Image _difficultyIcon;
        private TMP_Text _difficultyText;
        private TMP_Text _expPreviewText;
        private TMP_Text _descriptionText;

        private StageCategoryData _categoryData;

        protected override string GetPrefabAddress()
        {
            // UI 프리팹이 준비되면 아래 주석 해제
            // return "UI/Stage/ExpDungeonModuleUI";
            return null;
        }

        protected override void OnInitialize()
        {
            Log.Debug("[ExpDungeonContentModule] OnInitialize", LogCategory.UI);

            // UI 컴포넌트 찾기 (프리팹 있을 경우)
            if (_rootInstance != null)
            {
                _difficultyIcon = FindComponent<Image>();
                _difficultyText = FindTransform("DifficultyText")?.GetComponent<TMP_Text>();
                _expPreviewText = FindTransform("ExpPreview")?.GetComponent<TMP_Text>();
                _descriptionText = FindTransform("Description")?.GetComponent<TMP_Text>();
            }

            // 카테고리 데이터 로드
            LoadCategoryData();

            // UI 업데이트
            UpdateUI();
        }

        protected override void OnRefreshInternal(string selectedStageId)
        {
            // 필요 시 추가 갱신
        }

        protected override void OnStageSelectedInternal(StageData stageData)
        {
            // 선택된 스테이지 기반 경험치 미리보기
            UpdateExpPreview(stageData);
        }

        protected override void OnCategoryIdChanged(string categoryId)
        {
            // 외부에서 카테고리 변경 시 (StageDashboard에서 다른 난이도 선택)
            LoadCategoryData();
            UpdateUI();
        }

        protected override void OnReleaseInternal()
        {
            _categoryData = null;
        }

        #region Private Methods

        private void LoadCategoryData()
        {
            if (string.IsNullOrEmpty(_categoryId))
            {
                Log.Debug("[ExpDungeonContentModule] No categoryId set", LogCategory.UI);
                _categoryData = null;
                return;
            }

            var categoryDb = DataManager.Instance?.GetDatabase<StageCategoryDatabase>();
            if (categoryDb == null)
            {
                Log.Warning("[ExpDungeonContentModule] StageCategoryDatabase not found", LogCategory.UI);
                _categoryData = null;
                return;
            }

            _categoryData = categoryDb.GetById(_categoryId);

            if (_categoryData == null)
            {
                Log.Warning($"[ExpDungeonContentModule] Category not found: {_categoryId}", LogCategory.UI);
            }
            else
            {
                Log.Debug($"[ExpDungeonContentModule] Loaded category: {_categoryData.Id}, Difficulty: {_categoryData.Difficulty}", LogCategory.UI);
            }
        }

        private void UpdateUI()
        {
            if (_categoryData == null)
            {
                ClearUI();
                return;
            }

            // 난이도 아이콘
            if (_difficultyIcon != null && _categoryData.IconSprite != null)
            {
                _difficultyIcon.sprite = _categoryData.IconSprite;
                _difficultyIcon.gameObject.SetActive(true);
            }

            // 난이도 텍스트
            if (_difficultyText != null)
            {
                _difficultyText.text = GetDifficultyDisplayName(_categoryData.Difficulty);
            }

            // 설명
            if (_descriptionText != null && !string.IsNullOrEmpty(_categoryData.DescriptionKey))
            {
                _descriptionText.text = _categoryData.GetDisplayName();
            }

            // 경험치 미리보기 (초기값)
            UpdateExpPreview(null);
        }

        private void ClearUI()
        {
            if (_difficultyIcon != null)
            {
                _difficultyIcon.gameObject.SetActive(false);
            }

            if (_difficultyText != null)
            {
                _difficultyText.text = "";
            }

            if (_descriptionText != null)
            {
                _descriptionText.text = "";
            }

            if (_expPreviewText != null)
            {
                _expPreviewText.text = "";
            }
        }

        private void UpdateExpPreview(StageData stageData)
        {
            if (_expPreviewText == null) return;

            if (stageData != null)
            {
                _expPreviewText.text = $"획득 경험치: {stageData.RewardExp:N0}";
            }
            else if (_categoryData != null)
            {
                // 카테고리 기반 기본 경험치 표시
                var baseExp = GetBaseExpByDifficulty(_categoryData.Difficulty);
                _expPreviewText.text = $"기본 경험치: {baseExp:N0}~";
            }
            else
            {
                _expPreviewText.text = "";
            }
        }

        private string GetDifficultyDisplayName(Difficulty difficulty)
        {
            return difficulty switch
            {
                Difficulty.Easy => "쉬움",
                Difficulty.Normal => "보통",
                Difficulty.Hard => "어려움",
                _ => difficulty.ToString()
            };
        }

        private int GetBaseExpByDifficulty(Difficulty difficulty)
        {
            return difficulty switch
            {
                Difficulty.Easy => 100,
                Difficulty.Normal => 300,
                Difficulty.Hard => 600,
                _ => 100
            };
        }

        #endregion
    }
}
