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

        public NavigationTestScenarios(NavigationManager navigation, RectTransform screenContainer)
        {
            _navigation = navigation;
            _screenContainer = screenContainer;
        }

        #region Scenario: Push Pop

        /// <summary>
        /// 시나리오: Screen 3개 Push → Pop All
        /// 서로 다른 Screen 타입 사용 (중복 제거 방지)
        /// </summary>
        public async UniTask<TestResult> RunPushPopAllScenario()
        {
            // Screen A Push
            SimpleTestScreenA.CreateInstance(_screenContainer, "Screen_A");
            var ctxA = new SimpleTestScreenA.Context.Builder(
                new SimpleTestScreenState { ScreenName = "A", Index = 1 }
            ).Build();
            await _navigation.PushAsync(ctxA);

            // Screen B Push
            SimpleTestScreenB.CreateInstance(_screenContainer, "Screen_B");
            var ctxB = new SimpleTestScreenB.Context.Builder(
                new SimpleTestScreenState { ScreenName = "B", Index = 2 }
            ).Build();
            await _navigation.PushAsync(ctxB);

            // Screen C Push
            SimpleTestScreenC.CreateInstance(_screenContainer, "Screen_C");
            var ctxC = new SimpleTestScreenC.Context.Builder(
                new SimpleTestScreenState { ScreenName = "C", Index = 3 }
            ).Build();
            await _navigation.PushAsync(ctxC);

            int countAfterPush = _navigation.ScreenCount;

            // Pop 2개 (마지막 1개는 루트로 보호)
            await _navigation.PopAsync();
            await _navigation.PopAsync();

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
            // Screen A Push
            SimpleTestScreenA.CreateInstance(_screenContainer, "VisibilityTest_A");
            var ctxA = new SimpleTestScreenA.Context.Builder(
                new SimpleTestScreenState { ScreenName = "A", Index = 1 }
            ).Build();
            await _navigation.PushAsync(ctxA);

            // Screen B Push
            SimpleTestScreenB.CreateInstance(_screenContainer, "VisibilityTest_B");
            var ctxB = new SimpleTestScreenB.Context.Builder(
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
            // Screen Push
            SimpleTestScreenA.CreateInstance(_screenContainer, "PopupTest_Screen");
            var screenCtx = new SimpleTestScreenA.Context.Builder(
                new SimpleTestScreenState { ScreenName = "Main", Index = 1 }
            ).Build();
            await _navigation.PushAsync(screenCtx);

            int countAfterScreen = _navigation.StackCount;
            int popupCountBefore = _navigation.PopupCount;

            // Popup Push
            SimpleTestPopup.CreateInstance(_screenContainer, "PopupTest_Popup");
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
        /// 서로 다른 Screen 타입 사용
        /// </summary>
        public async UniTask<TestResult> RunBackNavigationScenario()
        {
            // Screen A Push
            SimpleTestScreenA.CreateInstance(_screenContainer, "Back_Screen1");
            var ctx1 = new SimpleTestScreenA.Context.Builder(
                new SimpleTestScreenState { ScreenName = "Screen1", Index = 1 }
            ).Build();
            await _navigation.PushAsync(ctx1);

            // Screen B Push (다른 타입!)
            SimpleTestScreenB.CreateInstance(_screenContainer, "Back_Screen2");
            var ctx2 = new SimpleTestScreenB.Context.Builder(
                new SimpleTestScreenState { ScreenName = "Screen2", Index = 2 }
            ).Build();
            await _navigation.PushAsync(ctx2);

            int countBefore = _navigation.ScreenCount;

            // Back 동작 테스트: PopAsync 직접 호출하여 완료 대기
            // Back()은 fire-and-forget이므로 테스트에서는 PopAsync 사용
            bool canGoBack = _navigation.ScreenCount > 1 || _navigation.HasPopupOnTop;
            await _navigation.PopAsync();

            int countAfter = _navigation.ScreenCount;

            bool success = countBefore == 2 && canGoBack && countAfter == 1;

            return new TestResult
            {
                Success = success,
                Message = $"Back전={countBefore}, Back가능={canGoBack}, Back후={countAfter}"
            };
        }

        #endregion

        #region Scenario: Duplicate Screen

        /// <summary>
        /// 시나리오: 중복 Screen 제거 테스트
        /// 같은 타입 Screen Push 시 기존 것이 제거됨
        /// </summary>
        public async UniTask<TestResult> RunDuplicateScreenScenario()
        {
            // 첫 번째 Screen A Push
            SimpleTestScreenA.CreateInstance(_screenContainer, "Dup_First");
            var ctx1 = new SimpleTestScreenA.Context.Builder(
                new SimpleTestScreenState { ScreenName = "First", Index = 1 }
            ).Build();
            await _navigation.PushAsync(ctx1);

            int countAfterFirst = _navigation.ScreenCount;

            // 같은 타입 Screen A Push (중복 제거 발생)
            SimpleTestScreenA.CreateInstance(_screenContainer, "Dup_Second");
            var ctx2 = new SimpleTestScreenA.Context.Builder(
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
