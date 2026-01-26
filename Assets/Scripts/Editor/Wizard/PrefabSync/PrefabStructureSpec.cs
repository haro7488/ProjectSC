using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sc.Editor.Wizard.PrefabSync
{
    /// <summary>
    /// 프리팹 구조를 JSON으로 직렬화하기 위한 스펙 모델.
    /// AI가 읽고 빌더 코드를 생성/수정하는 데 사용됨.
    /// </summary>
    [Serializable]
    public class PrefabStructureSpec
    {
        public MetadataInfo metadata;
        public HierarchyNode hierarchy;
        public List<SerializedFieldMapping> serializedFields;
        public ThemeInfo theme;
    }

    #region Metadata

    [Serializable]
    public class MetadataInfo
    {
        public string prefabName;
        public string prefabPath;
        public string componentType;
        public string componentNamespace;
        public string generatedAt;
        public string builderPath;
    }

    #endregion

    #region Hierarchy

    [Serializable]
    public class HierarchyNode
    {
        public string name;
        public string path; // 루트부터의 전체 경로
        public bool active;
        public RectInfo rect;
        public List<ComponentInfo> components;
        public List<HierarchyNode> children;
    }

    [Serializable]
    public class RectInfo
    {
        public string anchor; // "Stretch", "TopLeft", "BottomRight", etc.
        public float[] anchorMin; // [x, y]
        public float[] anchorMax; // [x, y]
        public float[] pivot; // [x, y]
        public float[] sizeDelta; // [width, height]
        public float[] anchoredPosition; // [x, y]
        public float[] offsetMin; // [left, bottom]
        public float[] offsetMax; // [right, top] (negative for inset)
    }

    [Serializable]
    public class ComponentInfo
    {
        public string type;
        public string fullType; // 전체 네임스페이스 포함
        public bool isMainComponent; // Screen/Popup/Widget 등 주요 컴포넌트
        public Dictionary<string, object> properties;
    }

    #endregion

    #region SerializedField Mapping

    [Serializable]
    public class SerializedFieldMapping
    {
        public string fieldName;
        public string fieldType;
        public string targetPath; // 연결된 GameObject/Component 경로
        public string targetType; // 연결된 타입
        public bool isArray;
        public List<string> arrayPaths; // 배열인 경우 각 요소의 경로
    }

    #endregion

    #region Theme

    [Serializable]
    public class ThemeInfo
    {
        public Dictionary<string, string> colors; // name -> hex color
        public Dictionary<string, float> constants; // name -> value
        public Dictionary<string, string> fonts; // name -> font asset path
    }

    #endregion

    #region Helper Extensions

    public static class RectInfoExtensions
    {
        public static RectInfo FromRectTransform(RectTransform rect)
        {
            if (rect == null) return null;

            return new RectInfo
            {
                anchor = GetAnchorName(rect),
                anchorMin = new[] { rect.anchorMin.x, rect.anchorMin.y },
                anchorMax = new[] { rect.anchorMax.x, rect.anchorMax.y },
                pivot = new[] { rect.pivot.x, rect.pivot.y },
                sizeDelta = new[] { rect.sizeDelta.x, rect.sizeDelta.y },
                anchoredPosition = new[] { rect.anchoredPosition.x, rect.anchoredPosition.y },
                offsetMin = new[] { rect.offsetMin.x, rect.offsetMin.y },
                offsetMax = new[] { rect.offsetMax.x, rect.offsetMax.y }
            };
        }

        private static string GetAnchorName(RectTransform rect)
        {
            var minX = rect.anchorMin.x;
            var minY = rect.anchorMin.y;
            var maxX = rect.anchorMax.x;
            var maxY = rect.anchorMax.y;

            // Stretch
            if (Approximately(minX, 0) && Approximately(maxX, 1) &&
                Approximately(minY, 0) && Approximately(maxY, 1))
                return "Stretch";

            // Top Stretch
            if (Approximately(minX, 0) && Approximately(maxX, 1) &&
                Approximately(minY, 1) && Approximately(maxY, 1))
                return "TopStretch";

            // Bottom Stretch
            if (Approximately(minX, 0) && Approximately(maxX, 1) &&
                Approximately(minY, 0) && Approximately(maxY, 0))
                return "BottomStretch";

            // Left Stretch
            if (Approximately(minX, 0) && Approximately(maxX, 0) &&
                Approximately(minY, 0) && Approximately(maxY, 1))
                return "LeftStretch";

            // Right Stretch
            if (Approximately(minX, 1) && Approximately(maxX, 1) &&
                Approximately(minY, 0) && Approximately(maxY, 1))
                return "RightStretch";

            // Corners
            if (Approximately(minX, 0) && Approximately(maxX, 0) &&
                Approximately(minY, 1) && Approximately(maxY, 1))
                return "TopLeft";

            if (Approximately(minX, 1) && Approximately(maxX, 1) &&
                Approximately(minY, 1) && Approximately(maxY, 1))
                return "TopRight";

            if (Approximately(minX, 0) && Approximately(maxX, 0) &&
                Approximately(minY, 0) && Approximately(maxY, 0))
                return "BottomLeft";

            if (Approximately(minX, 1) && Approximately(maxX, 1) &&
                Approximately(minY, 0) && Approximately(maxY, 0))
                return "BottomRight";

            // Centers
            if (Approximately(minX, 0.5f) && Approximately(maxX, 0.5f) &&
                Approximately(minY, 0.5f) && Approximately(maxY, 0.5f))
                return "Center";

            if (Approximately(minX, 0.5f) && Approximately(maxX, 0.5f) &&
                Approximately(minY, 1) && Approximately(maxY, 1))
                return "TopCenter";

            if (Approximately(minX, 0.5f) && Approximately(maxX, 0.5f) &&
                Approximately(minY, 0) && Approximately(maxY, 0))
                return "BottomCenter";

            if (Approximately(minX, 0) && Approximately(maxX, 0) &&
                Approximately(minY, 0.5f) && Approximately(maxY, 0.5f))
                return "LeftCenter";

            if (Approximately(minX, 1) && Approximately(maxX, 1) &&
                Approximately(minY, 0.5f) && Approximately(maxY, 0.5f))
                return "RightCenter";

            // Custom anchors (percentage based)
            if (Approximately(minY, 0) && Approximately(maxY, 1))
            {
                // Horizontal partial stretch
                return $"HStretch({minX:F2}-{maxX:F2})";
            }

            if (Approximately(minX, 0) && Approximately(maxX, 1))
            {
                // Vertical partial stretch
                return $"VStretch({minY:F2}-{maxY:F2})";
            }

            return "Custom";
        }

        private static bool Approximately(float a, float b)
        {
            return Mathf.Abs(a - b) < 0.001f;
        }
    }

    #endregion
}