using UnityEngine;
using UnityEngine.UI;

namespace Sc.Tests
{
    /// <summary>
    /// 테스트용 Canvas 계층 구조 생성
    /// </summary>
    public static class TestCanvasFactory
    {
        /// <summary>
        /// 테스트용 Canvas 계층 구조 생성
        /// </summary>
        public static TestCanvas Create(Transform parent)
        {
            // Canvas 생성
            var canvasGO = new GameObject("TestCanvas");
            canvasGO.transform.SetParent(parent);

            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000; // 최상위

            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            canvasGO.AddComponent<GraphicRaycaster>();

            // 컨테이너들 생성
            var screenContainer = CreateContainer(canvasGO.transform, "ScreenContainer", 0);
            var popupContainer = CreateContainer(canvasGO.transform, "PopupContainer", 100);
            var controlContainer = CreateContainer(canvasGO.transform, "ControlContainer", 200);

            return new TestCanvas
            {
                Root = canvasGO,
                Canvas = canvas,
                ScreenContainer = screenContainer,
                PopupContainer = popupContainer,
                ControlContainer = controlContainer
            };
        }

        private static RectTransform CreateContainer(Transform parent, string name, int sortingOrder)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var canvas = go.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = sortingOrder;

            go.AddComponent<GraphicRaycaster>();

            return rect;
        }
    }

    /// <summary>
    /// 테스트용 Canvas 계층 구조
    /// </summary>
    public struct TestCanvas
    {
        public GameObject Root;
        public Canvas Canvas;
        public RectTransform ScreenContainer;
        public RectTransform PopupContainer;
        public RectTransform ControlContainer;

        public void Destroy()
        {
            if (Root != null)
            {
                Object.DestroyImmediate(Root);
            }
        }
    }
}
