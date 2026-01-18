using UnityEditor;
using UnityEngine;
using Sc.Tests;

namespace Sc.Editor.Tests
{
    /// <summary>
    /// 시스템 테스트 에디터 메뉴.
    /// SC Tools/System Tests 하위에 각 시스템별 테스트 메뉴 등록.
    /// </summary>
    public static class SystemTestMenu
    {
        private const string MenuRoot = "SC Tools/System Tests/";

        #region Navigation Tests

        [MenuItem(MenuRoot + "UI/Test Navigation", false, 100)]
        private static void TestNavigation()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[SystemTest] 테스트는 Play Mode에서만 실행 가능합니다.");
                EditorApplication.isPlaying = true;
                EditorApplication.playModeStateChanged += OnPlayModeForNavigation;
                return;
            }

            LaunchNavigationTest();
        }

        private static void OnPlayModeForNavigation(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                EditorApplication.playModeStateChanged -= OnPlayModeForNavigation;
                // 한 프레임 대기 후 테스트 시작
                EditorApplication.delayCall += LaunchNavigationTest;
            }
        }

        private static void LaunchNavigationTest()
        {
            var existing = Object.FindFirstObjectByType<NavigationTestRunner>();
            if (existing != null)
            {
                Debug.Log("[SystemTest] 기존 NavigationTestRunner 발견. 재시작합니다.");
                existing.TeardownTest();
                Object.DestroyImmediate(existing.gameObject);
            }

            var go = new GameObject("NavigationTestRunner");
            var runner = go.AddComponent<NavigationTestRunner>();
            runner.SetupTest();

            Debug.Log("[SystemTest] Navigation 테스트 시작됨. 컨트롤 패널로 테스트를 실행하세요.");
        }

        [MenuItem(MenuRoot + "UI/Test Navigation", true)]
        private static bool ValidateTestNavigation()
        {
            // 메뉴 항상 활성화
            return true;
        }

        #endregion

        #region Utility

        [MenuItem(MenuRoot + "Stop All Tests", false, 1000)]
        private static void StopAllTests()
        {
            var runners = Object.FindObjectsByType<SystemTestRunner>(FindObjectsSortMode.None);
            foreach (var runner in runners)
            {
                runner.TeardownTest();
                Object.DestroyImmediate(runner.gameObject);
            }

            Debug.Log($"[SystemTest] {runners.Length}개의 테스트 러너 정리 완료.");
        }

        [MenuItem(MenuRoot + "Stop All Tests", true)]
        private static bool ValidateStopAllTests()
        {
            return Application.isPlaying;
        }

        #endregion
    }
}
