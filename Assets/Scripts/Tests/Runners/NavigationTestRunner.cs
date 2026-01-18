using UnityEngine;
using Sc.Common.UI;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Sc.Tests
{
    /// <summary>
    /// Navigation 시스템 테스트 러너.
    /// 에디터에서 SC Tools/System Tests/UI/Test Navigation 메뉴로 실행.
    /// </summary>
    public class NavigationTestRunner : SystemTestRunner
    {
        private NavigationManager _navigation;
        private NavigationTestScenarios _scenarios;
        private TextMeshProUGUI _resultText;
        private int _screenCounter;

        protected override string GetSystemName() => "Navigation";

        protected override void OnSetup()
        {
            // NavigationManager 생성
            var navGO = new GameObject("NavigationManager");
            navGO.transform.SetParent(_testRoot.transform);
            _navigation = navGO.AddComponent<NavigationManager>();

            // 시나리오 생성
            _scenarios = new NavigationTestScenarios(_navigation, _testCanvas.ScreenContainer);

            // 초기 Screen 생성 및 Push
            CreateAndPushInitialScreen().Forget();
        }

        private async UniTask CreateAndPushInitialScreen()
        {
            await UniTask.Yield(); // 한 프레임 대기

            var initialScreen = SimpleTestScreen.CreateInstance(
                _testCanvas.ScreenContainer, "InitialScreen");

            var ctx = new SimpleTestScreen.Context.Builder(
                new SimpleTestScreenState { ScreenName = "Initial", Index = 0 }
            ).Build();

            await _navigation.PushAsync(ctx);

            Debug.Log($"[NavigationTest] 초기 Screen Push 완료. StackCount={_navigation.StackCount}");
        }

        protected override void OnTeardown()
        {
            _navigation = null;
            _scenarios = null;
            _resultText = null;
        }

        protected override void CreateControlPanel()
        {
            var panel = TestUIBuilder.CreatePanel(
                _testCanvas.ControlContainer,
                "Navigation Test");

            // 기본 액션
            TestUIBuilder.AddLabel(panel, "Basic Actions:");
            TestUIBuilder.AddButton(panel, "Push Screen", OnPushScreenClicked);
            TestUIBuilder.AddButton(panel, "Push Popup", OnPushPopupClicked);
            TestUIBuilder.AddButton(panel, "Pop", OnPopClicked);
            TestUIBuilder.AddButton(panel, "Back", OnBackClicked);

            TestUIBuilder.AddSeparator(panel);

            // 시나리오
            TestUIBuilder.AddLabel(panel, "Test Scenarios:");
            TestUIBuilder.AddButton(panel, "Run: Push → Pop All", OnScenarioPushPopAll);
            TestUIBuilder.AddButton(panel, "Run: Visibility", OnScenarioVisibility);
            TestUIBuilder.AddButton(panel, "Run: Popup Stack", OnScenarioPopupStack);
            TestUIBuilder.AddButton(panel, "Run: Back Navigation", OnScenarioBackNav);
            TestUIBuilder.AddButton(panel, "Run: All Scenarios", OnRunAllScenarios);

            TestUIBuilder.AddSeparator(panel);

            // 결과 표시
            _resultText = TestUIBuilder.AddResultArea(panel, "결과가 여기에 표시됩니다...");

            TestUIBuilder.AddSeparator(panel);

            // 종료
            TestUIBuilder.AddButton(panel, "Exit Test", () => TeardownTest());

            _controlPanel = panel;
        }

        #region Basic Actions

        private void OnPushScreenClicked()
        {
            _screenCounter++;
            var screen = SimpleTestScreen.CreateInstance(
                _testCanvas.ScreenContainer, $"Screen_{_screenCounter}");

            var ctx = new SimpleTestScreen.Context.Builder(
                new SimpleTestScreenState { ScreenName = $"Screen", Index = _screenCounter }
            ).Build();

            _navigation.Push(ctx);
            UpdateStackInfo();
        }

        private void OnPushPopupClicked()
        {
            var popup = SimpleTestPopup.CreateInstance(
                _testCanvas.PopupContainer, $"Popup_{System.Guid.NewGuid().ToString()[..4]}");

            var ctx = new SimpleTestPopup.Context.Builder(
                new SimpleTestPopupState { PopupName = "TestPopup", Index = 1 }
            ).Build();

            _navigation.Push(ctx);
            UpdateStackInfo();
        }

        private void OnPopClicked()
        {
            _navigation.Pop();
            UpdateStackInfo();
        }

        private void OnBackClicked()
        {
            bool result = _navigation.Back();
            Debug.Log($"[NavigationTest] Back: {(result ? "성공" : "실패")}");
            UpdateStackInfo();
        }

        private void UpdateStackInfo()
        {
            if (_resultText != null)
            {
                _resultText.text = $"Stack: {_navigation.GetStackDebugString()}\n" +
                                  $"Screens: {_navigation.ScreenCount}, Popups: {_navigation.PopupCount}";
            }
        }

        #endregion

        #region Scenarios

        private async void OnScenarioPushPopAll()
        {
            SetResultText("Running: Push → Pop All...");
            var result = await _scenarios.RunPushPopAllScenario();
            LogResult("PushPopAll", result);
            SetResultText(result.ToString());
        }

        private async void OnScenarioVisibility()
        {
            SetResultText("Running: Visibility...");
            var result = await _scenarios.RunVisibilityScenario();
            LogResult("Visibility", result);
            SetResultText(result.ToString());
        }

        private async void OnScenarioPopupStack()
        {
            SetResultText("Running: Popup Stack...");
            var result = await _scenarios.RunPopupStackScenario();
            LogResult("PopupStack", result);
            SetResultText(result.ToString());
        }

        private async void OnScenarioBackNav()
        {
            SetResultText("Running: Back Navigation...");
            var result = await _scenarios.RunBackNavigationScenario();
            LogResult("BackNavigation", result);
            SetResultText(result.ToString());
        }

        private async void OnRunAllScenarios()
        {
            SetResultText("Running all scenarios...");

            int passed = 0;
            int failed = 0;
            var sb = new System.Text.StringBuilder();

            // PushPopAll
            var result1 = await _scenarios.RunPushPopAllScenario();
            LogResult("PushPopAll", result1);
            sb.AppendLine($"PushPopAll: {(result1.Success ? "PASS" : "FAIL")}");
            if (result1.Success) passed++;
            else failed++;

            await UniTask.Delay(200);

            // Visibility
            var result2 = await _scenarios.RunVisibilityScenario();
            LogResult("Visibility", result2);
            sb.AppendLine($"Visibility: {(result2.Success ? "PASS" : "FAIL")}");
            if (result2.Success) passed++;
            else failed++;

            await UniTask.Delay(200);

            // PopupStack
            var result3 = await _scenarios.RunPopupStackScenario();
            LogResult("PopupStack", result3);
            sb.AppendLine($"PopupStack: {(result3.Success ? "PASS" : "FAIL")}");
            if (result3.Success) passed++;
            else failed++;

            await UniTask.Delay(200);

            // BackNavigation
            var result4 = await _scenarios.RunBackNavigationScenario();
            LogResult("BackNavigation", result4);
            sb.AppendLine($"BackNavigation: {(result4.Success ? "PASS" : "FAIL")}");
            if (result4.Success) passed++;
            else failed++;

            sb.AppendLine($"\n총: {passed}/{passed + failed} 통과");

            SetResultText(sb.ToString());
        }

        private void SetResultText(string text)
        {
            if (_resultText != null)
            {
                _resultText.text = text;
            }
        }

        #endregion
    }
}
