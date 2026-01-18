using Cysharp.Threading.Tasks;
using Sc.Common.UI;
using UnityEngine;

namespace Sc.Tests
{
    /// <summary>
    /// Navigation 시스템 테스트 시나리오 모음.
    /// 수동 테스트와 자동화 테스트에서 공유.
    /// </summary>
    public class NavigationTestScenarios
    {
        private readonly NavigationManager _navigation;
        private readonly RectTransform _screenContainer;
        private int _screenCounter;
        private int _popupCounter;

        public NavigationTestScenarios(NavigationManager navigation, RectTransform screenContainer)
        {
            _navigation = navigation;
            _screenContainer = screenContainer;
        }

        #region Helper Methods

        private SimpleTestScreen CreateScreen(string name = null)
        {
            _screenCounter++;
            var screenName = name ?? $"Screen_{_screenCounter}";
            return SimpleTestScreen.CreateInstance(_screenContainer, screenName);
        }

        private SimpleTestPopup CreatePopup(string name = null)
        {
            _popupCounter++;
            var popupName = name ?? $"Popup_{_popupCounter}";
            return SimpleTestPopup.CreateInstance(_screenContainer, popupName);
        }

        public void Reset()
        {
            _screenCounter = 0;
            _popupCounter = 0;
        }

        #endregion

        #region Scenario: Push Pop

        /// <summary>
        /// 시나리오: Screen 3개 Push → Pop All
        /// </summary>
        public async UniTask<TestResult> RunPushPopAllScenario()
        {
            Reset();

            // 3개 Screen 생성
            var screen1 = CreateScreen("Screen_A");
            var screen2 = CreateScreen("Screen_B");
            var screen3 = CreateScreen("Screen_C");

            // Push 3개
            var ctx1 = new SimpleTestScreen.Context.Builder(
                new SimpleTestScreenState { ScreenName = "A", Index = 1 }
            ).Build();
            await _navigation.PushAsync(ctx1);

            var ctx2 = new SimpleTestScreen.Context.Builder(
                new SimpleTestScreenState { ScreenName = "B", Index = 2 }
            ).Build();
            await _navigation.PushAsync(ctx2);

            var ctx3 = new SimpleTestScreen.Context.Builder(
                new SimpleTestScreenState { ScreenName = "C", Index = 3 }
            ).Build();
            await _navigation.PushAsync(ctx3);

            int countAfterPush = _navigation.ScreenCount;

            // Pop 3개
            await _navigation.PopAsync();
            await _navigation.PopAsync();
            // 마지막 Screen은 루트로 보호됨 (Pop 안됨)

            int countAfterPop = _navigation.ScreenCount;

            bool success = countAfterPush == 3 && countAfterPop == 1;

            return new TestResult
            {
                Success = success,
                Message = $"Push후={countAfterPush}, Pop후={countAfterPop} (예상: 3, 1)"
            };
        }

        #endregion

        #region Scenario: Visibility

        /// <summary>
        /// 시나리오: Screen 가시성 테스트
        /// Screen A Push → Screen B Push → A는 Hidden, B는 Visible
        /// </summary>
        public async UniTask<TestResult> RunVisibilityScenario()
        {
            Reset();

            var screenA = CreateScreen("VisibilityTest_A");
            var screenB = CreateScreen("VisibilityTest_B");

            // Screen A Push
            var ctxA = new SimpleTestScreen.Context.Builder(
                new SimpleTestScreenState { ScreenName = "A", Index = 1 }
            ).Build();
            await _navigation.PushAsync(ctxA);

            // Screen B Push
            var ctxB = new SimpleTestScreen.Context.Builder(
                new SimpleTestScreenState { ScreenName = "B", Index = 2 }
            ).Build();
            await _navigation.PushAsync(ctxB);

            // 가시성 확인
            var viewA = ctxA.View;
            var viewB = ctxB.View;

            // CanvasGroup.alpha로 가시성 확인
            var cgA = viewA?.GetComponent<CanvasGroup>();
            var cgB = viewB?.GetComponent<CanvasGroup>();

            bool aHidden = cgA == null || cgA.alpha < 0.5f || !viewA.gameObject.activeInHierarchy;
            bool bVisible = cgB != null && cgB.alpha > 0.5f && viewB.gameObject.activeInHierarchy;

            bool success = aHidden && bVisible;

            return new TestResult
            {
                Success = success,
                Message = $"A.Hidden={aHidden}, B.Visible={bVisible}"
            };
        }

        #endregion

        #region Scenario: Popup Stack

        /// <summary>
        /// 시나리오: Screen + Popup 스택 테스트
        /// Screen Push → Popup Push → Popup Pop → Screen 유지
        /// </summary>
        public async UniTask<TestResult> RunPopupStackScenario()
        {
            Reset();

            var screen = CreateScreen("PopupTest_Screen");
            var popup = CreatePopup("PopupTest_Popup");

            // Screen Push
            var screenCtx = new SimpleTestScreen.Context.Builder(
                new SimpleTestScreenState { ScreenName = "Main", Index = 1 }
            ).Build();
            await _navigation.PushAsync(screenCtx);

            int countAfterScreen = _navigation.StackCount;
            int popupCountBefore = _navigation.PopupCount;

            // Popup Push
            var popupCtx = new SimpleTestPopup.Context.Builder(
                new SimpleTestPopupState { PopupName = "Test", Index = 1 }
            ).Build();
            await _navigation.PushAsync(popupCtx);

            int countAfterPopup = _navigation.StackCount;
            int popupCountAfter = _navigation.PopupCount;
            bool hasPopupOnTop = _navigation.HasPopupOnTop;

            // Popup Pop
            await _navigation.PopAsync();

            int countAfterPop = _navigation.StackCount;
            int screenCountFinal = _navigation.ScreenCount;

            bool success =
                countAfterScreen == 1 &&
                popupCountBefore == 0 &&
                countAfterPopup == 2 &&
                popupCountAfter == 1 &&
                hasPopupOnTop &&
                countAfterPop == 1 &&
                screenCountFinal == 1;

            return new TestResult
            {
                Success = success,
                Message = $"Screen후={countAfterScreen}, Popup후={countAfterPopup}, Pop후={countAfterPop}"
            };
        }

        #endregion

        #region Scenario: Back Navigation

        /// <summary>
        /// 시나리오: Back 동작 테스트
        /// </summary>
        public async UniTask<TestResult> RunBackNavigationScenario()
        {
            Reset();

            var screen1 = CreateScreen("Back_Screen1");
            var screen2 = CreateScreen("Back_Screen2");

            // Screen 1 Push
            var ctx1 = new SimpleTestScreen.Context.Builder(
                new SimpleTestScreenState { ScreenName = "Screen1", Index = 1 }
            ).Build();
            await _navigation.PushAsync(ctx1);

            // Screen 2 Push
            var ctx2 = new SimpleTestScreen.Context.Builder(
                new SimpleTestScreenState { ScreenName = "Screen2", Index = 2 }
            ).Build();
            await _navigation.PushAsync(ctx2);

            int countBefore = _navigation.ScreenCount;

            // Back 호출
            bool backResult = _navigation.Back();

            // 약간의 대기 (비동기 Pop 완료 대기)
            await UniTask.Delay(100);

            int countAfter = _navigation.ScreenCount;

            bool success = countBefore == 2 && backResult && countAfter == 1;

            return new TestResult
            {
                Success = success,
                Message = $"Back전={countBefore}, Back성공={backResult}, Back후={countAfter}"
            };
        }

        #endregion

        #region Scenario: Duplicate Screen

        /// <summary>
        /// 시나리오: 중복 Screen 제거 테스트
        /// Screen A → Screen B → Screen A 시 기존 A 제거
        /// </summary>
        public async UniTask<TestResult> RunDuplicateScreenScenario()
        {
            Reset();

            // Note: 현재 구현에서는 같은 타입의 Screen이 Push되면 기존 것이 제거됨
            // SimpleTestScreen은 한 타입이므로 항상 중복 제거됨

            var screen1 = CreateScreen("Dup_First");
            var screen2 = CreateScreen("Dup_Second");

            // 첫 번째 Screen Push
            var ctx1 = new SimpleTestScreen.Context.Builder(
                new SimpleTestScreenState { ScreenName = "First", Index = 1 }
            ).Build();
            await _navigation.PushAsync(ctx1);

            int countAfterFirst = _navigation.ScreenCount;

            // 같은 타입 Screen Push (중복 제거 발생)
            var ctx2 = new SimpleTestScreen.Context.Builder(
                new SimpleTestScreenState { ScreenName = "Second", Index = 2 }
            ).Build();
            await _navigation.PushAsync(ctx2);

            int countAfterSecond = _navigation.ScreenCount;

            // 중복 제거로 인해 Screen 수가 그대로 1개여야 함
            bool success = countAfterFirst == 1 && countAfterSecond == 1;

            return new TestResult
            {
                Success = success,
                Message = $"첫번째후={countAfterFirst}, 두번째후={countAfterSecond} (중복제거: 1,1 예상)"
            };
        }

        #endregion
    }
}
