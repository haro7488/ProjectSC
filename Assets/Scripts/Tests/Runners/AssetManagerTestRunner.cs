using Sc.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Sc.Tests
{
    /// <summary>
    /// AssetManager 시스템 테스트 러너.
    /// 에디터 또는 런타임에서 AssetManager 테스트 실행.
    /// </summary>
    public class AssetManagerTestRunner : SystemTestRunner
    {
        private AssetManagerTestScenarios _scenarios;

        protected override string GetSystemName() => "AssetManager";

        protected override void OnSetup()
        {
            _scenarios = new AssetManagerTestScenarios();
        }

        protected override void OnTeardown()
        {
            _scenarios = null;
        }

        protected override void RegisterMockServices()
        {
            // AssetManager는 Singleton이므로 별도 서비스 등록 불필요
        }

        protected override void CreateControlPanel()
        {
            if (_testCanvas.Root == null) return;

            // 컨트롤 패널 생성
            _controlPanel = new GameObject("ControlPanel");
            _controlPanel.transform.SetParent(_testCanvas.ControlContainer, false);

            var panelRect = _controlPanel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 1);
            panelRect.anchorMax = new Vector2(0, 1);
            panelRect.pivot = new Vector2(0, 1);
            panelRect.anchoredPosition = new Vector2(20, -20);
            panelRect.sizeDelta = new Vector2(300, 350);

            var layout = _controlPanel.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 10;
            layout.padding = new RectOffset(10, 10, 10, 10);
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var bg = _controlPanel.AddComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.8f);

            // 버튼 생성
            CreateButton("Run All Tests", OnRunAllTests);
            CreateButton("Initialization", () => RunScenario("Initialization", _scenarios.RunInitializationScenario));
            CreateButton("Scope Creation", () => RunScenario("ScopeCreation", _scenarios.RunScopeCreationScenario));
            CreateButton("Scope Release", () => RunScenario("ScopeRelease", _scenarios.RunScopeReleaseScenario));
            CreateButton("Duplicate Scope", () => RunScenario("DuplicateScope", _scenarios.RunDuplicateScopeScenario));
            CreateButton("Cache Count", () => RunScenario("CacheCount", _scenarios.RunCacheCountScenario));
            CreateButton("Close", TeardownTest);
        }

        private void CreateButton(string text, UnityEngine.Events.UnityAction onClick)
        {
            var buttonGo = new GameObject(text);
            buttonGo.transform.SetParent(_controlPanel.transform, false);

            var buttonRect = buttonGo.AddComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(280, 40);

            var buttonImage = buttonGo.AddComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);

            var button = buttonGo.AddComponent<Button>();
            button.targetGraphic = buttonImage;
            button.onClick.AddListener(onClick);

            var textGo = new GameObject("Text");
            textGo.transform.SetParent(buttonGo.transform, false);

            var textRect = textGo.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;

            var textComponent = textGo.AddComponent<Text>();
            textComponent.text = text;
            textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            textComponent.fontSize = 16;
            textComponent.alignment = TextAnchor.MiddleCenter;
            textComponent.color = Color.white;
        }

        private void OnRunAllTests()
        {
            if (_scenarios == null)
            {
                Debug.LogError("[AssetManagerTest] 시나리오가 초기화되지 않았습니다.");
                return;
            }

            _scenarios.RunAllScenarios();
        }

        private void RunScenario(string name, System.Func<TestResult> scenarioFunc)
        {
            if (_scenarios == null)
            {
                Debug.LogError("[AssetManagerTest] 시나리오가 초기화되지 않았습니다.");
                return;
            }

            var result = scenarioFunc();
            LogResult(name, result);
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Test/AssetManager/Run All Tests")]
        public static void EditorRunAllTests()
        {
            // AssetManager 초기화 확인
            if (!AssetManager.HasInstance)
            {
                Debug.LogError("[AssetManagerTest] AssetManager 인스턴스가 필요합니다. 플레이 모드에서 실행하세요.");
                return;
            }

            if (!AssetManager.Instance.IsInitialized)
            {
                AssetManager.Instance.Initialize();
            }

            var scenarios = new AssetManagerTestScenarios();
            scenarios.RunAllScenarios();
        }

        [UnityEditor.MenuItem("Test/AssetManager/Initialization")]
        public static void EditorRunInitialization()
        {
            if (!AssetManager.HasInstance)
            {
                Debug.LogError("[AssetManagerTest] AssetManager 인스턴스가 필요합니다. 플레이 모드에서 실행하세요.");
                return;
            }

            var scenarios = new AssetManagerTestScenarios();
            var result = scenarios.RunInitializationScenario();
            Debug.Log($"[AssetManagerTest:Initialization] {result}");
        }

        [UnityEditor.MenuItem("Test/AssetManager/Scope Creation")]
        public static void EditorRunScopeCreation()
        {
            if (!AssetManager.HasInstance || !AssetManager.Instance.IsInitialized)
            {
                Debug.LogError("[AssetManagerTest] AssetManager가 초기화되지 않았습니다. 플레이 모드에서 실행하세요.");
                return;
            }

            var scenarios = new AssetManagerTestScenarios();
            var result = scenarios.RunScopeCreationScenario();
            Debug.Log($"[AssetManagerTest:ScopeCreation] {result}");
        }
#endif
    }
}
